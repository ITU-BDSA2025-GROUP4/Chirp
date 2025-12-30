namespace Chirp.Infrastructure.Services;

using System.Text;
using Chirp.Core.Entities;
using Chirp.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Chirp.Core.Utils;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _repository;
    private readonly UserManager<Author> _userManager;
    private readonly SignInManager<Author> _signInManager;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public AuthorService(
            UserManager<Author> userManager,
            SignInManager<Author> signInManager,
            IAuthorRepository repository,
            IEmailService emailService,
            IConfiguration configuration
            )
    {
        _repository = repository;
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _configuration = configuration;
    }

    //todo: should prob fix this / make it better
    // i use to generate a unique username if the username of the user when logging in with oauth is already in use
    public async Task<String> GenerateUniqueUsername(string baseUsername)
    {
        string username;
        int attempts = 0;
        do
        {
            var suffix = Guid.NewGuid().ToString("N")[..6]; // random 6-char hex
            username = $"{baseUsername}_{suffix}";
        } while (await _userManager.FindByNameAsync(username) != null && attempts < 10);

        return await _userManager.FindByNameAsync(username) == null ? username : throw new Exception("Unable to generate unique username");
    }

    public async Task<ChangePasswordStatus> ChangeAuthorPasswordAsync(ChangePasswordForm form, ClaimsPrincipal claim) {
        Optional<AuthorDTO> author = await GetLoggedInAuthor(claim);

        if(!author.HasValue)
        {
            return ChangePasswordStatus.NotLoggedIn;
        }

        if(form.NewPassword != form.NewPasswordConfirm)
        {
            return ChangePasswordStatus.PasswordsDoNotMatch;
        }

        Optional<Author> concreteAuthor =
            await _repository.GetConcreteAuthorAsync(author.Value().Email);

        if(!concreteAuthor.HasValue)
        {
            return ChangePasswordStatus.NotLoggedIn;
        }
        
        await _userManager.ChangePasswordAsync(
                concreteAuthor.Value(),
                form.PreviousPassword,
                form.NewPassword
        );

        return ChangePasswordStatus.Success;
    }

    public async Task<bool> UsingOAuth(ClaimsPrincipal claim)
    {
        Optional<AuthorDTO> author = await GetLoggedInAuthor(claim);

        if(!author.HasValue)
        {
            return false;
        }

        Optional<Author> concreteAuthor =
            await _repository.GetConcreteAuthorAsync(author.Value().Email);

        if(!concreteAuthor.HasValue)
        {
            return false;
        }

        // Users who use OAuth have no password
        return !await _userManager.HasPasswordAsync(concreteAuthor.Value());
    }

    // this is used for logging in a user or giving them feedback on what they can do
    // i.e. for example, if the it is possible to create a new author with the external information, we do it and log them in
    // if the email is taken, we return a enum member indicating that
    public async Task<ExternalLoginStatus> LoginOrGetOptionsAsync()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null) return ExternalLoginStatus.FailedToRetrieveLoginInfo;

        var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
        if (user != null)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return ExternalLoginStatus.LoggedIn;
        }

        var email = info.Principal.FindFirstValue(ClaimTypes.Email)!;
        var findEmail = await FindByEmailAsync(email);

        if (findEmail.HasValue)
        {
            return ExternalLoginStatus.EmailAlreadyInUseAccountMustBeLinked;
        }

        var username = info.Principal.FindFirstValue(ClaimTypes.Name) ?? info.ProviderKey;
        if ((await FindByNameAsync(username)).HasValue)
        {
            try
            {
                username = await GenerateUniqueUsername(username);
            }
            catch (Exception)
            {
                return ExternalLoginStatus.FailedToGenerateUniqueUsername;
            }
        }

        user = new Author
        {
            UserName = username,
            Email = email,
        };

        var createResult = await _userManager.CreateAsync(user);
        if (!createResult.Succeeded) return ExternalLoginStatus.FailedToCreateUser;

        await _userManager.AddLoginAsync(user, info);
        await _signInManager.SignInAsync(user, isPersistent: false);
        return ExternalLoginStatus.Success;
    }
    // this is unused at the moment
    // my plan is to allow users to link accounts to extenral stuff in their account settings
    // why? because it would be nice if a user could login in traditionally or thru github :)
    public async Task<ExternalLoginStatus> LinkExternal(Author user)
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null) return ExternalLoginStatus.FailedToRetrieveLoginInfo;

        await _userManager.AddLoginAsync(user, info);
        await _signInManager.SignInAsync(user, isPersistent: false);

        return ExternalLoginStatus.LinkedSuccessfully;
    }

    public async Task<(bool, string?)> RegisterAuthorAsync(RegisterViewModel model)
    {
        if (model.HasNull())
        {
            return (false, "All fields must be filled");
        }

        var findEmail = await FindByEmailAsync(model.Email);
        var findName = await FindByNameAsync(model.Username);
        if (findEmail.HasValue)
        {
            return (false, $"Email '{model.Email}' is already in use");
        }
        if (findName.HasValue)
        {
            return (false, $"Username '{model.Username}' is already in use");
        }
        if (model.Password != model.ConfirmPassword)
        {
            return (false, "Passwords do not match");
        }

        var user = new Author
        {
            UserName = model.Username,
            Email = model.Email,
            Cheeps = new List<Cheep>(),
        };

        IdentityResult result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return (false, "Password is too weak");

        // If email confirmation is setup, we can re-enable this
        //        var token = await GenerateEmailConfirmationTokenAsync(user);
        //
        //        var baseUrl =
        //            _configuration["AppSettings:BaseUrl"]
        //            ?? throw new InvalidOperationException("BaseUrl is not configured.");
        //        var confirmationLink = $"{baseUrl}/Account/confirmEmail?userId={user.Id}&token={token}";
        //
        //        await _emailService.SendRegistrationConfirmationEmailAsync(
        //            user.Email,
        //            user.UserName,
        //            confirmationLink
        //        );

        return (true, null);
    }

    public async Task<bool> DeleteAuthorAsync(AuthorDTO author) {
        await _repository.DeleteAuthor(author);
        return true;
    }

    public async Task<Microsoft.AspNetCore.Identity.SignInResult> LoginUserAsync(LoginViewModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
            return Microsoft.AspNetCore.Identity.SignInResult.Failed;

        // Not supported yet
        //        if (!await _userManager.IsEmailConfirmedAsync(user))
        //            return Microsoft.AspNetCore.Identity.SignInResult.NotAllowed;

        var result = await _signInManager.PasswordSignInAsync(
            user.UserName!,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false
        );

        return result;
    }

    public async Task<Optional<AuthorDTO>> GetLoggedInAuthor(ClaimsPrincipal principal)
    {
        bool isSignedIn = _signInManager.IsSignedIn(principal);
        if (!isSignedIn)
        {
            return Optional.Empty<AuthorDTO>();
        }
        else if (principal.Identity == null || principal.Identity.Name == null)
        {
            return Optional.Empty<AuthorDTO>();
        }

        string name = principal.Identity.Name;

        return await FindByNameAsync(name);
    }

    public async Task LogoutAuthorAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<IEnumerable<AuthorDTO>> GetAuthorsAsync()
    {
        return await _repository.ReadAll();
    }

    public async Task<Optional<AuthorDTO>> FindByEmailAsync(string email)
    {
        return await _repository.FindByEmailAsync(email);
    }

    public async Task<Optional<AuthorDTO>> FindByNameAsync(string name)
    {
        return await _repository.FindByNameAsync(name);
    }

    public async Task<Optional<AuthorDTO>> FindByIdAsync(int id)
    {
        return await _repository.FindByIdAsync(id);
    }

    public async Task Write()
    {
        await _repository.Write();
        return;
    }

    public async Task<IdentityResult> ConfirmEmailAsync(Guid userId, string token)
    {
        if (userId == Guid.Empty || string.IsNullOrEmpty(token))
            return IdentityResult.Failed(
                new IdentityError { Description = "Invalid token or user ID." }
            );

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found" });

        var decodedBytes = WebEncoders.Base64UrlDecode(token);
        var decodedToken = Encoding.UTF8.GetString(decodedBytes);

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        if (result.Succeeded)
        {
            var baseUrl =
                _configuration["AppSettings:BaseUrl"]
                ?? throw new InvalidOperationException("BaseUrl is not configured.");
            var loginLink = $"{baseUrl}/Account/Login";

            await _emailService.SendAccountCreatedEmailAsync(
                user.Email!,
                user.UserName!,
                loginLink
            );
        }
        return result;
    }

    public async Task SendEmailConfirmationAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            // Prevent user enumeration by not disclosing existence
            return;
        }
        if (await _userManager.IsEmailConfirmedAsync(user))
        {
            // Email already confirmed; no action needed
            return;
        }

        var token = await GenerateEmailConfirmationTokenAsync(user);
        var baseUrl =
            _configuration["AppSettings:BaseUrl"]
            ?? throw new InvalidOperationException("BaseUrl is not configured");
        var confirmationLink = $"{baseUrl}/Account/ConfirmEmail?userId={user.Id}&token={token}";

        await _emailService.SendResendConfirmationEmailAsync(
            user.Email!,
            user.UserName!,
            confirmationLink
        );
    }

    private async Task<String> GenerateEmailConfirmationTokenAsync(Author user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        return encodedToken;
    }

    AuthenticationProperties IAuthorService.ConfigureExternalAuthenticationProperties(string provider, string redirectUrl)
    {
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return properties;
    }
}
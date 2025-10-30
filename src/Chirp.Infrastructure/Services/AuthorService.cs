namespace Chirp.Infrastructure.Services;

using System.Text;
using Chirp.Core.Entities;
using Chirp.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Chirp.Core.Utils;
using Microsoft.Extensions.Configuration;

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

    public async Task<(bool, string?)> RegisterAuthorAsync(RegisterViewModel model)
    {
        if(model.HasNull())
        {
            return (false, "All fields must be filled");
        }

        var findEmail = await GetAuthorByEmail(model.Email);
        var findName = await GetAuthorByName(model.Username);
        if(findEmail.HasValue)
        {
            return (false, $"Email '{model.Email}' is already in use");
        }
        if(findName.HasValue)
        {
            return (false, $"Username '{model.Username}' is already in use");
        }
        if(model.Password != model.ConfirmPassword)
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

    public async Task<SignInResult> LoginUserAsync(LoginViewModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
            return SignInResult.Failed;

        // Not supported yet
//        if (!await _userManager.IsEmailConfirmedAsync(user))
//            return SignInResult.NotAllowed;

        var result = await _signInManager.PasswordSignInAsync(
            user.UserName!,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false
        );

        return result;
    }

    public async Task LogoutAuthorAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<IEnumerable<AuthorDTO>> GetAuthors()
    {
        return await _repository.ReadAll();
    }

    public async Task<Optional<AuthorDTO>> GetAuthorByEmail(string email)
    {
        return await _repository.FindAuthorByEmail(email);
    }

    public async Task<Optional<AuthorDTO>> GetAuthorByName(string name)
    {
        return await _repository.FindAuthorByName(name);
    }

    public async Task Write() {
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
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        return encodedToken;
    }

}
namespace Chirp.Core.Interfaces;

using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Chirp.Core.Utils;
using Chirp.Core.Entities;
using Microsoft.AspNetCore.Authentication;

public enum ExternalLoginStatus
{
    FailedToRetrieveLoginInfo,
    EmailAlreadyInUseAccountMustBeLinked,
    CanLinkOrCreate,
    UserNameAlreadyInUse,
    LoggedIn,
    FailedToGenerateUniqueUsername,
    FailedToCreateUser,
    LinkedSuccessfully,
    Success,
}

public interface IAuthorService
{
    public Task<IEnumerable<AuthorDTO>> GetAuthorsAsync();
    public Task<Optional<AuthorDTO>> FindByNameAsync(string name);
    public Task<Optional<AuthorDTO>> FindByEmailAsync(string email);
    public Task<Optional<AuthorDTO>> FindByIdAsync(int id);


    Task<Optional<AuthorDTO>> GetLoggedInAuthor(ClaimsPrincipal principal);
    Task<(bool, string?)> RegisterAuthorAsync(RegisterViewModel model);
    Task<IdentityResult> ConfirmEmailAsync(Guid userId, string token);
    Task<SignInResult> LoginUserAsync(LoginViewModel model);
    Task LogoutAuthorAsync();
    Task SendEmailConfirmationAsync(string email);

    Task<ExternalLoginStatus> LoginOrGetOptionsAsync();
    Task<ExternalLoginStatus> LinkExternal(Author user);
    public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl);
}
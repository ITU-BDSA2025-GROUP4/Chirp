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
    public Task<IEnumerable<AuthorDTO>> GetAuthors();
    public Task<Optional<AuthorDTO>> GetAuthorByName(string name);
    public Task<Optional<AuthorDTO>> GetAuthorByEmail(string email);

    Task<Optional<AuthorDTO>> GetLoggedInAuthor(ClaimsPrincipal principal);
    Task<(bool, string?)> RegisterAuthorAsync(RegisterViewModel model);
    public Task<bool> DeleteAuthorAsync(AuthorDTO author);
    Task<IdentityResult> ConfirmEmailAsync(Guid userId, string token);
    Task<SignInResult> LoginUserAsync(LoginViewModel model);
    Task LogoutAuthorAsync();
    Task SendEmailConfirmationAsync(string email);

    Task<ExternalLoginStatus> LoginOrGetOptionsAsync();
    Task<ExternalLoginStatus> LinkExternal(Author user);
    public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl);
}
using Microsoft.AspNetCore.Identity;
using Chirp.Core.Entities;
//TODO: Look into onion acitecture since I have messed up

namespace Chirp.Core.Interfaces;

public interface IAccountService
{
    Task<IdentityResult> RegisterUserAsync(RegisterViewModel model);
    Task<IdentityResult> ConfirmEmailAsync(Guid userId, string token);
    Task<SignInResult> LoginUserAsync(LoginViewModel model);
    Task LogoutUserAsync();
    Task SendEmailConfirmationAsync(string email);
}
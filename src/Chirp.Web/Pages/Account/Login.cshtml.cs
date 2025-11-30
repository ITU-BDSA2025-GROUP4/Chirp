using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Chirp.Core.Interfaces;
using Chirp.Core.Entities;
using Chirp.Core.Utils;
using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;

namespace Chirp.Razor.Pages;

public class LoginPageModel : PageModel
{
    private readonly IAuthorService _authorService;

    public LoginPageModel(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Optional<AuthorDTO> tmp = await _authorService.GetLoggedInAuthor(User);

        if (tmp.HasValue)
        {
            return Redirect("/");
        }

        return Page();
    }

    public IActionResult OnPostExternalLogin(string provider)
    {
        if (!OAuthEnabledStatus.IsOAuthEnabled)
        {
            TempData["message"] = "OAuth is not configured, missing Client ID and/or Client secret";
            return Page();
        }
        var redirectUrl = Url.Page("/Account/Login", pageHandler: "ExternalLoginCallback")!;
        var properties = _authorService.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> OnGetExternalLoginCallback()
    {
        var loginStatus = await _authorService.LoginOrGetOptionsAsync();


        switch (loginStatus)
        {
            case ExternalLoginStatus.FailedToRetrieveLoginInfo:
            case ExternalLoginStatus.FailedToGenerateUniqueUsername:
            case ExternalLoginStatus.FailedToCreateUser:
                TempData["message"] = "An error occured while signing in with OAuth. Please try again.";
                break;

            case ExternalLoginStatus.EmailAlreadyInUseAccountMustBeLinked:
                TempData["message"] = "Email already in use. Please sign into your account and link it with the external provider.";
                break;

            case ExternalLoginStatus.LoggedIn:
//                Console.WriteLine("Login success");
                return Redirect("/");

            default:
                break;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostLogin(LoginViewModel login)
    {
        // This case occurs when the password or email don't fit the requirements
        // set by the annotations on the password and email field.
        // I.e. the password is considered unsafe or the email is not valid.
        if (!ModelState.IsValid)
        {
            TempData["message"] = "Incorrect password or email";
            return Page();
        }

        var result = await _authorService.LoginUserAsync(login);
        if (result.Succeeded)
        {
//            Console.WriteLine("Login success");
            return Redirect("/");
        }

        if (result.IsNotAllowed)
            TempData["message"] = "Email is not confirmed yet.";
        else
            TempData["message"] = "Invalid login attampt.";

        //_logger.LogError(e, "Error during login for email: {Email}", model.Email);
        TempData["message"] = $"Incorrect password or email";
        return Page();
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Identity;

using Chirp.Core.Interfaces;
using Chirp.Infrastructure.Services;
using Chirp.Core.Entities;
using Chirp.Core.Utils;

namespace Chirp.Razor.Pages;

public class LoginPageModel : PageModel
{
    private readonly IAuthorService _authorService;

    public LoginPageModel(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    [HttpGet]
    public IActionResult OnGet()
    {
        Task<Optional<AuthorDTO>> tmp = _authorService.GetLoggedInAuthor(User);
        tmp.Wait();

        if(tmp.Result.HasValue)
        {
            return Redirect("/");
        }

        return Page();
    }

    [HttpPost]
    public IActionResult OnPostLogin(LoginViewModel login)
    {
        // This case occurs when the password or email don't fit the requirements
        // set by the annotations on the password and email field.
        // I.e. the password is considered unsafe or the email is not valid.
        if (!ModelState.IsValid)
        {
            TempData["message"] = "Incorrect password or email";
            return Page();
        }

        var result = _authorService.LoginUserAsync(login);
        result.Wait();

        if (result.Result.Succeeded)
        {
            Console.WriteLine("Login success");
            return Redirect("/");
        }

        if (result.Result.IsNotAllowed)
            TempData["message"] = "Email is not confirmed yet.";
        else
            TempData["message"] = "Invalid login attampt.";

        //_logger.LogError(e, "Error during login for email: {Email}", model.Email);
        TempData["message"] = $"Incorrect password or email";
        return Page();
    }
}
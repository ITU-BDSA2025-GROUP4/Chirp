using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Identity;
using Chirp.Infrastructure.Services;
using Chirp.Core.Interfaces;
using Chirp.Core.Entities;

namespace Chirp.Razor.Pages;

public class RegisterPageModel : PageModel
{
    private readonly IAuthorService _authorService;

    public RegisterPageModel(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    public IActionResult OnPostRegister(RegisterViewModel registerModel)
    {
        var task = _authorService.RegisterAuthorAsync(registerModel);
        task.Wait();

        var (success, msg) = task.Result;

        // User registration might fail if the email or username is already in
        // use. It can also fail that if the password and confirm password
        // don't match.
        //
        // In case of failure, we want to keep the fields populated so that the
        // user doesn't have to refill everything and only change the parts
        // that are problematic. 
        if(!success)
        {
            TempData["message"] = msg;
            TempData["username"] = registerModel.Username;
            TempData["email"] = registerModel.Email;
            return Page();
        }

        return Redirect("/");
    }
}
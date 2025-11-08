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

    [HttpPost]
    public IActionResult OnPostRegister(RegisterViewModel registerModel)
    {
        var task = _authorService.RegisterAuthorAsync(registerModel);
        task.Wait();

        var (success, msg) = task.Result;

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
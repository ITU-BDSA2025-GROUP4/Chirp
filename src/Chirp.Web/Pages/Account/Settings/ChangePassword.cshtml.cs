using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

using Chirp.Core.Interfaces;
using Chirp.Core.Entities;
using Chirp.Core.Utils;
using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;

namespace Chirp.Razor.Pages;

public class ChangePasswordPageModel : PageModel
{
    private readonly IAuthorService _authorService;

    public ChangePasswordPageModel(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    public async Task<IActionResult> OnGet()
    {
       Optional<AuthorDTO> tmp = await _authorService.GetLoggedInAuthor(User);

        if (!tmp.HasValue)
        {
            return Redirect("/Account/Login");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostChangePassword(ChangePasswordForm form)
    {
        Console.WriteLine("Changing password");
        Optional<AuthorDTO> user = await _authorService.GetLoggedInAuthor(User);
        if (!user.HasValue)
        {
            Console.WriteLine("Not loggied in");
            return Redirect("/Account/Login");
        }

        _authorService.ChangeAuthorPasswordAsync(form, User);

        return Redirect("/");
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Chirp.Core.Interfaces;
using Chirp.Core.Entities;
using Chirp.Core.Utils;
using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;

namespace Chirp.Razor.Pages;

public class SettingsPageModel : PageModel
{
    private readonly IAuthorService _authorService;

    public SettingsPageModel(IAuthorService authorService)
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

        TempData["username"] = tmp.Value().Name;
        TempData["email"] = tmp.Value().Email;

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAccount()
    {
        Optional<AuthorDTO> user = await _authorService.GetLoggedInAuthor(User);
        if (!user.HasValue)
        {
            return Redirect("/Account/Login");
        }

        await _authorService.LogoutAuthorAsync();
        AuthorDTO authorDTO = user.Value();
        await _authorService.DeleteAuthorAsync(authorDTO);

        return Redirect("/");
    }
}
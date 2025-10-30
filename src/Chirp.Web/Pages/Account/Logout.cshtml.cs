using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Chirp.Core.Interfaces;

namespace Chirp.Razor.Pages;

public class LogoutPageModel : PageModel
{
    private readonly IAuthorService _authorService;

    public LogoutPageModel(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    public IActionResult OnGet()
    {
        _authorService.LogoutAuthorAsync();
        return Redirect("/");
    }
}
using Chirp.Core.Application.Contracts;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Chirp.Core.Entities;
using Chirp.Core.Interfaces;
using Chirp.Core.Utils;

namespace Chirp.Razor.Pages;

[IgnoreAntiforgeryToken]
public class PublicModel : PageModel
{
    // Here for testing only, should be stored as secret in the future;
    private static readonly string APItoken = "eD[oiaj24_wda=/232)=_1EEdhue]3";

    private static readonly int _pageSize = 32;

    private readonly ICheepService _service;
    private readonly IAuthorService _authorService;
    public IEnumerable<CheepDTO> Cheeps { get; set; } = null!;

    public class CheepSubmitForm
    {
        [BindProperty]
        public string? Cheep { get; set; }

        public string? APItoken { get; set; } = null!;
    }

    public PublicModel(ICheepService service, IAuthorService authorService)
    {
        _service = service;
        _authorService = authorService;
    }

    public async Task OnGetAsync([FromQuery] int page = 1)
    {
        page = page > 1 ? page : 1;
        Cheeps = await _service.GetCheeps(page, _pageSize);
    }

    [HttpPost]
    public async Task<IActionResult> OnPostSubmit(CheepSubmitForm form)
    {
  
        if (string.IsNullOrWhiteSpace(form.Cheep))
        {
            TempData["message"] = "Cheep cannot be empty";
            return Redirect("/");
        }

        string name;
        if (form.APItoken != null && form.APItoken == APItoken)
        {
            name = "Jacqualine Gilcoine";
        }
        else
        {
            var logged = await _authorService.GetLoggedInAuthor(User);
            if (!logged.HasValue)
            {
                TempData["message"] = "Must be logged in to cheep";
                return Redirect("/");
            }
            name = logged.Value().Name;
        }

        var authorOpt = await _authorService.FindByNameAsync(name);
        if (!authorOpt.HasValue)
        {
            TempData["message"] = $"Username '{name}' not found";
            return Redirect("/");
        }
        var authorId = authorOpt.Value().Id;

        var request = new CreateCheepRequest(
            Text: form.Cheep!.Trim(),
            AuthorId: authorId
        );

        var result = await _service.PostCheepAsync(request); // AppResult<CheepDTO>

        if (!result.IsSuccess)
        {
            TempData["message"] = result.Message ?? "failed to create cheep";
            return Redirect("/");
        }

        return Redirect("/cheep");
    }
}
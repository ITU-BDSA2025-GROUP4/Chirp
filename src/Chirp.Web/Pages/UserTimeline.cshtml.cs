using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Chirp.Core.Entities;
using Chirp.Core.Interfaces;
using Chirp.Core.Utils;

namespace Chirp.Razor.Pages;

[IgnoreAntiforgeryToken]
public class UserTimelineModel : PageModel
{
    private static readonly int _pageSize = 32;

    private readonly ICheepService _service;
    private readonly IAuthorService _authorService;
    public IEnumerable<CheepDTO> Cheeps { get; set; } = null!;

    public UserTimelineModel(ICheepService service, IAuthorService authorService)
    {
        _service = service;
        _authorService = authorService;
    }

    public async Task OnGetAsync(string author, [FromQuery] int page = 1)
    {
        page = page > 1 ? page : 1;
        Cheeps = await _service.GetCheepsFromAuthor(author, page, _pageSize);
    }
}
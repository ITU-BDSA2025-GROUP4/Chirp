using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Chirp.Razor.Models;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private static readonly int _pageSize = 32;

    private readonly ICheepService _service;
    public IEnumerable<CheepViewModel> Cheeps { get; set; }

    public UserTimelineModel(ICheepService service)
    {
        _service = service;
    }

    public async Task OnGetAsync(string author, [FromQuery] int page = 1)
    {
        page = page > 1 ? page : 1;
        Cheeps = await _service.GetCheepsFromAuthor(author, page, _pageSize);
    }
}
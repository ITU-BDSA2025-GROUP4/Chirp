using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Chirp.Core.Entities;
using Chirp.Core.Interfaces;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private static readonly int _pageSize = 32;

    private readonly ICheepService _service;
    public IEnumerable<CheepDTO> Cheeps { get; set; }

    public PublicModel(ICheepService service)
    {
        _service = service;
    }

    public async Task OnGetAsync([FromQuery] int page = 1)
    {
        page = page > 1 ? page : 1;
        Cheeps = await _service.GetCheeps(page, _pageSize);
    }
}
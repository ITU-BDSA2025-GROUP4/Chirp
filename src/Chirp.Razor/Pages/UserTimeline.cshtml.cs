using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private static readonly int _pageSize = 32;

    private readonly ICheepService _service;
    public List<CheepViewModel> Cheeps { get; set; }

    public UserTimelineModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet(string author, [FromQuery] int page = 1)
    {
        page = page > 1 ? page : 1;
        Cheeps = _service.GetCheepsFromAuthor(author).Skip((page - 1) * _pageSize).Take(_pageSize).ToList();
        return Page();
    }
}
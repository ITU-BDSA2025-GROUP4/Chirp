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

    public async Task OnGetAsync([FromQuery] int page = 1, [FromQuery] string author = "")
    {
        page = page > 1 ? page : 1;
        TempData["currentPage"] = page;

        if(author == "")
        Cheeps = await _service.GetCheeps(page, _pageSize);
        else
        Cheeps = await _service.GetCheepsFromAuthor(author, page, _pageSize);
    }

    [HttpPost]
    public IActionResult OnPostSubmit(CheepSubmitForm form)
    {
        string name;
        if(form.APItoken != null && form.APItoken == APItoken)
        {
            name = "Jacqualine Gilcoine";
        }
        else
        {
            Task<Optional<AuthorDTO>> tmp = _authorService.GetLoggedInAuthor(User);
            tmp.Wait();

            if(!tmp.Result.HasValue)
            {
                TempData["message"] = "Must be logged in to cheep";
                return Redirect("/");
            }

            name = tmp.Result.Value().Name;
        }

        if(form.Cheep == null || form.Cheep.Trim() == "")
        {
            TempData["message"] = "Cheep cannot be empty";
            return Redirect("/");
        }
        string time = TimestampUtils.DateTimeTimeStampToDateTimeString(
                DateTime.Now
        );

        var cheepDTO = new CheepDTO(name, form.Cheep, time);

        bool wasAdded = _service.AddCheep(cheepDTO).Result;

        // This should never happen.
        // Unless if the database is on fire, then it will definitely happen
        if(!wasAdded) TempData["message"] = "Invalid user";

        return Redirect("/");
    }

    [HttpPost]
    public IActionResult OnPostPageHandle(string Page, string Author)
    {
        int page = 1;
        int.TryParse(Page, out page);
        if(Author == null || Author.Trim() == "")
        return Redirect("/?page="+page);
        else
        return Redirect("/?page="+page+"&author="+Author.Trim());
    }

    private IActionResult GoToPage(int page)
    {
        return Redirect("/?page="+page.ToString());
    }

    [HttpPost]
    public IActionResult OnPostGoToPage(string Page)
    {
        int pageNumber = 1;
        int.TryParse(Page, out pageNumber);
        return GoToPage(pageNumber);
    }

    [HttpPost]
    public IActionResult OnPostNext(string Page)
    {
        int pageNumber = 1;
        int.TryParse(Page, out pageNumber);
        pageNumber += 1;
        return GoToPage(pageNumber);
    }

    [HttpPost]
    public IActionResult OnPostPrevious(string Page)
    {
        int pageNumber = 1;
        int.TryParse(Page, out pageNumber);
        pageNumber -= 1;
        return GoToPage(pageNumber);
    }
        
}
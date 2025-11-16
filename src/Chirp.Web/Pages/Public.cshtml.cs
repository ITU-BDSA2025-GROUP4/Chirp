using System.ComponentModel.DataAnnotations;

using Chirp.Core.Application.Contracts;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Chirp.Core.Entities;
using Chirp.Core.Interfaces;
using Chirp.Core.Utils;

namespace Chirp.Razor.Pages;

//[IgnoreAntiforgeryToken]
public class PublicModel : PageModel
{
    // Here for testing only, should be stored as secret in the future;
    // Deprecated
    //private static readonly string APItoken = "eD[oiaj24_wda=/232)=_1EEdhue]3";

    private static readonly int _pageSize = 32;

    private readonly ICheepService _service;
    private readonly IAuthorService _authorService;
    public IEnumerable<CheepDTO> Cheeps { get; set; } = null!;
    [BindProperty] public CheepSubmitForm Form { get; set; } = new();

    public class CheepSubmitForm : IValidatableObject
    {
        [BindProperty]
        [StringLength(160, MinimumLength = 1, ErrorMessage = "Cheep length must be between 1 and 160")]
        public string? Cheep { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            Cheep = Cheep?.Trim();
            if (string.IsNullOrWhiteSpace(Cheep))
                yield return new ValidationResult("Cheep cannot be empty", new[] { nameof(Cheep) });
        }
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
        {
            TempData["timeline"] = author;
            Cheeps = await _service.GetCheepsFromAuthor(author, page, _pageSize);
        }
    }
    
    // BE AWARE OF BUG!
    // For an unknown reason, returning Page() causes this.Cheeps to be null,
    // which cases the Public.cshtml to throw an exception when it checks for Cheeps.
    // Redirecting back to index seems to medigate the issue, but it's worth looking into it.
    public async Task<IActionResult> OnPostSubmit(CheepSubmitForm form)
    {
        if (!ModelState.IsValid)
            return Redirect("/");

        string name;
        Task<Optional<AuthorDTO>> tmp = _authorService.GetLoggedInAuthor(User);
        tmp.Wait();

        if(!tmp.Result.HasValue)
        {
            TempData["message"] = "Must be logged in to cheep";
            return Redirect("/");
        }

        name = tmp.Result.Value().Name;

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

        return Redirect("/");
    }

    public IActionResult OnPostPageHandle(string Page, string Author)
    {
        int page = 1;
        int.TryParse(Page, out page);
        if(Author == null || Author.Trim() == "")
        return Redirect("/?page="+page);
        else
        return Redirect("/?page="+page+"&author="+Author.Trim());
    }
}
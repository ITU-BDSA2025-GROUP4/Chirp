using System.ComponentModel.DataAnnotations;

using Chirp.Core.Application.Contracts;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Chirp.Core.Entities;
using Chirp.Core.Interfaces;
using Chirp.Core.Utils;
using Microsoft.AspNetCore.Http.Extensions;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private static readonly int _pageSize = 32;

    private readonly ICheepService _service;
    private readonly IAuthorService _authorService;
    private readonly IFollowService _followService;

    public IEnumerable<CheepDTO> Cheeps { get; set; } = [];

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
                yield return new ValidationResult("Cheep cannot be empty", [nameof(Cheep)]);
        }
    }

    public PublicModel(ICheepService service, IAuthorService authorService, IFollowService followService)
    {
        _service = service;
        _authorService = authorService;
        _followService = followService;
    }

    public async Task OnGetAsync([FromQuery] int page = 1, [FromQuery] string author = "")
    {
        page = page > 1 ? page : 1;
        TempData["currentPage"] = page;


        Optional<AuthorDTO> currentAuthor = await _authorService.GetLoggedInAuthor(User);

        if (author == "")
            Cheeps = await _service.GetCheeps(page, _pageSize);
        else
        {
            TempData["timeline"] = author;

            if (currentAuthor.HasValue && currentAuthor.Value().Name == author)
            {
                Cheeps = await _service.GetCheepsWrittenByAuthorAndFollowedAuthors(currentAuthor.Value().Id, page, _pageSize);
            }
            else
            {
                Cheeps = await _service.GetCheepsFromAuthor(author, page, _pageSize);
            }
        }

    }

    public async Task<IActionResult> OnPostFollow(string author)
    {
        var currentAuthor = await _authorService.GetLoggedInAuthor(User);

        if (!currentAuthor.HasValue)
        {
            // todo: actually redirect properly at somepoint
            return Page();
        }

        var followee = await _authorService.FindByNameAsync(author);

        if (followee.HasValue)
        {
            var request = new FollowRequest(currentAuthor.Value().Id, followee.Value().Id);
            await _followService.FollowAuthorAsync(request);
            // todo: actually redirect properly at somepoint
            return Page();
        }

        // todo: actually redirect properly at somepoint
        return Page();
        return Redirect(Request.GetDisplayUrl());
    }

    public async Task<IActionResult> OnPostUnfollow(string author)
    {
        var currentAuthor = await _authorService.GetLoggedInAuthor(User);

        if (!currentAuthor.HasValue)
        {
            // todo: actually redirect properly at somepoint
            return Page();
        }

        var followee = await _authorService.FindByNameAsync(author);

        if (followee.HasValue)
        {
            var request = new FollowRequest(currentAuthor.Value().Id, followee.Value().Id);
            await _followService.UnfollowAuthorAsync(request);
            // todo: actually redirect properly at somepoint
            return Page();
        }

        // todo: actually redirect properly at somepoint
        return Page();
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

        if (!tmp.Result.HasValue)
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
        if (Author == null || Author.Trim() == "")
            return Redirect("/?page=" + page);
        else
            return Redirect("/?page=" + page + "&author=" + Author.Trim());
    }
}
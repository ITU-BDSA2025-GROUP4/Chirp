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
    private readonly IReplyService _replyService;
    private readonly IAuthorService _authorService;
    private readonly IFollowService _followService;

    public HashSet<string> FollowedAuthorNames { get; set; } = [];
    public IEnumerable<CheepDTO> Cheeps { get; set; } = [];
    public Dictionary<int, IEnumerable<ReplyDTO>> Replies { get; set; } = [];

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

    public PublicModel(ICheepService service, IReplyService replyService, IAuthorService authorService, IFollowService followService)
    {
        _service = service;
        _replyService = replyService;
        _authorService = authorService;
        _followService = followService;
    }

    // HTTP get handler for public and private(author) timelines
    public async Task OnGetAsync([FromQuery] int page = 1, [FromQuery] string author = "")
    {
        // Ensure page is always >= 1
        page = page > 1 ? page : 1;
        // Value is rendered on UI to display the current page number
        TempData["currentPage"] = page;

        var optionalAuthor = await _authorService.GetLoggedInAuthor(User);
        AuthorDTO? currentAuthor = optionalAuthor.HasValue ? optionalAuthor.Value() : null;

        if (currentAuthor != null)
        {
            FollowedAuthorNames = await _followService.GetFollowedAuthorNames(currentAuthor.Id);
        }

        // If author is an empty string, then the user has requested the public timeline rather than a author-specific timeline
        if (author == "")
            Cheeps = await _service.GetCheeps(page, _pageSize);
        else
        {
            // Used to display the name of the author whose timeline is being shown
            TempData["timeline"] = author;

            if (currentAuthor != null && currentAuthor.Name == author)
            {
                Cheeps = await _service.GetCheepsWrittenByAuthorAndFollowedAuthors(currentAuthor.Id, page, _pageSize);
            }
            else
            {
                Cheeps = await _service.GetCheepsFromAuthor(author, page, _pageSize);
            }
        }

        foreach(CheepDTO cheep in Cheeps)
        {
            Replies.Add(cheep.Id, await _replyService.GetReplies(cheep.Id));
        }
    }

    // Triggered when logged-in user clicks the follow button
    public async Task<IActionResult> OnPostFollow(string author, string returnUrl = "/")
    {
        var currentAuthor = await _authorService.GetLoggedInAuthor(User);

        // If the user isn't logged in, then simply ignore request
        if (!currentAuthor.HasValue)
        {
            return Redirect(returnUrl);
        }

        var followee = await _authorService.FindByNameAsync(author);

        if (followee.HasValue)
        {
            var request = new FollowRequest(currentAuthor.Value().Id, followee.Value().Id);
            await _followService.FollowAuthorAsync(request);
            // todo: actually redirect properly at somepoint
            return Redirect(returnUrl);
        }

        return Redirect(returnUrl);
    }

    // Triggered when logged-in user clicks the un-follow button
    public async Task<IActionResult> OnPostUnfollow(string author, string returnUrl = "/")
    {
        var currentAuthor = await _authorService.GetLoggedInAuthor(User);

        if (!currentAuthor.HasValue)
        {
            return Redirect(returnUrl);
        }

        var followee = await _authorService.FindByNameAsync(author);

        if (followee.HasValue)
        {
            var request = new FollowRequest(currentAuthor.Value().Id, followee.Value().Id);
            await _followService.UnfollowAuthorAsync(request);
            return Redirect(returnUrl);
        }

        return Redirect(returnUrl);
    }

    // BE AWARE OF BUG!
    // For an unknown reason, returning Page() causes this.Cheeps to be null,
    // which cases the Public.cshtml to throw an exception when it checks for Cheeps.
    // Redirecting back to index seems to medigate the issue, but it's worth looking into it.
    //
    // Called when user submits cheep
    public async Task<IActionResult> OnPostSubmit(CheepSubmitForm form, string returnUrl = "/")
    {
        if (!ModelState.IsValid)
            return Redirect(returnUrl);

        string name;
        Task<Optional<AuthorDTO>> tmp = _authorService.GetLoggedInAuthor(User);
        tmp.Wait();

        if (!tmp.Result.HasValue)
        {
            TempData["message"] = "Must be logged in to cheep";
            return Redirect(returnUrl);
        }

        name = tmp.Result.Value().Name;

        // This should in theory never occur, since we previously established
        // that hte user is already logged in; thus their account must exist
        var authorOpt = await _authorService.FindByNameAsync(name);
        if (!authorOpt.HasValue)
        {
            TempData["message"] = $"Username '{name}' not found";
            return Redirect(returnUrl);
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
            return Redirect(returnUrl);
        }

        return Redirect(returnUrl);
    }

    // Called when user writes reply to cheep
    public async Task<IActionResult> OnPostReply(string ReplyText, int CheepId, string Author, string returnUrl = "/")
    {
        Optional<AuthorDTO> currentUserMaybe = await _authorService.GetLoggedInAuthor(User);

        if(!currentUserMaybe.HasValue)
        {
            TempData["message"] = "Must be logged in to reply";
            return Redirect(returnUrl);
        }

        AuthorDTO currentUser = currentUserMaybe.Value();

        CreateReplyRequest reply = new(currentUser.Id, CheepId, ReplyText);
        await _replyService.PostReplyAsync(reply);

        return Redirect(returnUrl);
    }

    // Pagination handler
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
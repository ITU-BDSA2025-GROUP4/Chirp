using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Chirp.Core.Application.Contracts;
using Chirp.Core.Entities;
using Chirp.Core.Interfaces;
using Chirp.Core.Utils;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private static readonly int _pageSize = 32;

    private readonly ICheepService _service;
    private readonly IReplyService _replyService;
    private readonly IReCheepService _reCheepService;
    private readonly IAuthorService _authorService;
    private readonly IFollowService _followService;

    public HashSet<string> FollowedAuthorNames { get; set; } = [];
    public IEnumerable<CheepDTO> Cheeps { get; set; } = [];
    public IEnumerable<ReCheepDTO> ReCheeps { get; set; } = [];
    public IEnumerable<TimelineEntities> TimelineEntities { get; set; } = [];
    public Dictionary<int, IEnumerable<ReplyDTO>> Replies { get; set; } = [];

    [BindProperty]
    public CheepSubmitForm Form { get; set; } = new();

    public class CheepSubmitForm : IValidatableObject
    {
        [BindProperty]
        [StringLength(
            160,
            MinimumLength = 1,
            ErrorMessage = "Cheep length must be between 1 and 160"
        )]
        public string? Cheep { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            Cheep = Cheep?.Trim();
            if (string.IsNullOrWhiteSpace(Cheep))
                yield return new ValidationResult("Cheep cannot be empty", [nameof(Cheep)]);
        }
    }

    public PublicModel(
        ICheepService service,
        IReplyService replyService,
        IReCheepService reCheepService,
        IAuthorService authorService,
        IFollowService followService
    )
    {
        _service = service;
        _replyService = replyService;
        _reCheepService = reCheepService;
        _authorService = authorService;
        _followService = followService;
    }

    public async Task OnGetAsync([FromQuery] int page = 1, [FromQuery] string author = "")
    {
        page = page > 1 ? page : 1;
        TempData["currentPage"] = page;

        var optionalAuthor = await _authorService.GetLoggedInAuthor(User);
        AuthorDTO? currentAuthor = optionalAuthor.HasValue ? optionalAuthor.Value() : null;

        if (currentAuthor != null)
        {
            FollowedAuthorNames = await _followService.GetFollowedAuthorNames(currentAuthor.Id);
        }

        if (author == "")
        {
            Cheeps = await _service.GetCheeps(page, _pageSize);
            ReCheeps = await _reCheepService.ReadAll();
        }
        else
        {
            TempData["timeline"] = author;

            if (currentAuthor != null && currentAuthor.Name == author)
            {
                Cheeps = await _service.GetCheepsWrittenByAuthorAndFollowedAuthors(
                    currentAuthor.Id,
                    page,
                    _pageSize
                );
                ReCheeps = await _reCheepService.GetReCheeps(currentAuthor.Id);
            }
            else
            {
                Cheeps = await _service.GetCheepsFromAuthor(author, page, _pageSize);
                ReCheeps = await _reCheepService.GetReCheeps(author);
            }
        }

        try
        {
            var cheepEnumerator = Cheeps.GetEnumerator();
            var cheepNext = cheepEnumerator.Current;
            var reCheepEnumerator = ReCheeps.GetEnumerator();
            var reCheepNext = reCheepEnumerator?.Current;
            Debug.Write(Cheeps.ToString());
            //Debug.Write(ReCheeps.ToString());
            for (int i = 0; i < Cheeps.Count() && i < ReCheeps.Count(); i++)
            {
                DateTime cheepDate =
                    cheepNext != null ? DateTime.Parse(cheepNext.Timestamp) : DateTime.MaxValue;
                DateTime reCheepDate =
                    reCheepNext != null
                        ? DateTime.Parse(
                            Cheeps
                                .Where(c => c.Id == reCheepNext.CheepId)
                                .Select(c => c.Timestamp)
                                .First()
                        )
                        : DateTime.MaxValue;

                Debug.Write(cheepDate.ToString());
                Debug.Write(reCheepDate.ToString());
                var entity = new TimelineEntities();
                if (cheepDate <= reCheepDate)
                {
                    Debug.Write("ReCheep date heigher");
                    entity.Type = TimelineType.Cheep;
                    entity.Cheep = cheepNext!;
                    cheepEnumerator.MoveNext();
                    cheepNext = cheepEnumerator.Current;
                }
                else if (cheepDate > reCheepDate)
                {
                    Debug.Write("Cheep date heigher");
                    entity.Type = TimelineType.ReCheep;
                    entity.ReCheep = new Optional<ReCheepDTO>(reCheepNext!);
                    entity.Cheep = Cheeps
                        .Where(c => c.Id == reCheepNext!.CheepId)
                        .Select(c => new CheepDTO(c.Id, c.Author, c.Text, c.Timestamp))
                        .First()!;
                    reCheepEnumerator.MoveNext();
                    reCheepNext = reCheepEnumerator.Current;
                }

                TimelineEntities = TimelineEntities.Append(entity);

                if (cheepNext == null && reCheepNext == null)
                    break;
            }
        }
        catch
        {
            foreach (var cheep in Cheeps)
            {
                var entity = new TimelineEntities { Type = TimelineType.Cheep, Cheep = cheep };
                TimelineEntities = TimelineEntities.Append(entity);
            }
        }

        foreach (CheepDTO cheep in Cheeps)
        {
            Replies.Add(cheep.Id, await _replyService.GetReplies(cheep.Id));
        }
    }

    public async Task<IActionResult> OnPostFollow(string author, string returnUrl = "/")
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
            await _followService.FollowAuthorAsync(request);
            // todo: actually redirect properly at somepoint
            return Redirect(returnUrl);
        }

        return Redirect(returnUrl);
    }

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

        var authorOpt = await _authorService.FindByNameAsync(name);
        if (!authorOpt.HasValue)
        {
            TempData["message"] = $"Username '{name}' not found";
            return Redirect(returnUrl);
        }
        var authorId = authorOpt.Value().Id;

        var request = new CreateCheepRequest(Text: form.Cheep!.Trim(), AuthorId: authorId);

        var result = await _service.PostCheepAsync(request); // AppResult<CheepDTO>

        if (!result.IsSuccess)
        {
            TempData["message"] = result.Message ?? "failed to create cheep";
            return Redirect(returnUrl);
        }

        return Redirect(returnUrl);
    }

    public async Task<IActionResult> OnPostReply(
        string ReplyText,
        int CheepId,
        string Author,
        string returnUrl = "/"
    )
    {
        Optional<AuthorDTO> currentUserMaybe = await _authorService.GetLoggedInAuthor(User);

        if (!currentUserMaybe.HasValue)
        {
            TempData["message"] = "Must be logged in to reply";
            return Redirect(returnUrl);
        }

        AuthorDTO currentUser = currentUserMaybe.Value();

        Console.WriteLine("REPLY: " + ReplyText);
        Console.WriteLine("Author: " + Author);
        Console.WriteLine("CheepId: " + CheepId);
        Console.WriteLine("returnUrl: " + returnUrl);

        CreateReplyRequest reply = new(currentUser.Id, CheepId, ReplyText);
        await _replyService.PostReplyAsync(reply);

        return Redirect(returnUrl);
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

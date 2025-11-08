using Chirp.Core.Application.Contracts;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Chirp.Core.Entities;
using Chirp.Core.Interfaces;
using Chirp.Core.Utils;

namespace Chirp.Razor.Pages;

[IgnoreAntiforgeryToken]
public class CheepModel : PageModel
{
    public class CheepSubmitForm
    {
        [BindProperty] public string? Name { get; set; }
        [BindProperty] public string? Cheep { get; set; }
        [BindProperty] public int? AuthorId { get; set; }
    }

    private readonly ICheepService _cheeps;
    private readonly IAuthorService _authors;
      public CheepModel(ICheepService cheeps, IAuthorService authors)
    {
        _cheeps = cheeps;
        _authors = authors;
    }

    public void OnGet() {}

    public async Task<IActionResult> OnPostSubmitAsync(CheepSubmitForm form)
    {
        if (string.IsNullOrWhiteSpace(form.Cheep))
            return BadRequest("Cheep must be provided");

        int authorId;
        if (form.AuthorId is { } postedId)
        {
            authorId = postedId;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(form.Name))
                return BadRequest("Username must be provided");

            var author = await _authors.FindByNameAsync(form.Name);
            if (!author.HasValue)
                return BadRequest($"Username: '{form.Name}' not found");

            authorId = author.Value().Id;
        }

        var request = new CreateCheepRequest(Text: form.Cheep!, AuthorId: authorId);

        var result = await _cheeps.PostCheepAsync(request); // returns AppResult<CheepDTO>
        if (result.IsError)
            return BadRequest(result.Message ?? "failed to create cheep");

        return Redirect("/cheep");
    }
    
}
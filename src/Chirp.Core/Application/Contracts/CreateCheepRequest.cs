namespace Chirp.Core.Application.Contracts;

public sealed record CreateCheepRequest( int AuthorId,
    [System.ComponentModel.DataAnnotations.StringLength(280, MinimumLength = 1)]
    string Text
   );
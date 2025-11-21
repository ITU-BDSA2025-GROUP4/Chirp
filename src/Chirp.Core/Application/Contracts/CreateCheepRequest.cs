namespace Chirp.Core.Application.Contracts;

public sealed record CreateCheepRequest( int AuthorId,
    [System.ComponentModel.DataAnnotations.StringLength(160, MinimumLength = 1)]
    string Text
   );
namespace Chirp.Core.Application.Contracts;

public sealed record CreateReplyRequest( int AuthorId, int CheepId,
    [System.ComponentModel.DataAnnotations.StringLength(80, MinimumLength = 1)]
    string Text
   );
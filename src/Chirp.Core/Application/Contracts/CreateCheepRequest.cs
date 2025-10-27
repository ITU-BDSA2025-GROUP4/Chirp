namespace Chirp.Core.Application.Contracts;

public sealed record CreateCheepRequest(int AuthorId, string Text);
namespace Chirp.Core.Application.Contracts;

public sealed record DeleteCheepRequest(int CheepId, int RequestId, string? ETag);
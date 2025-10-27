namespace Chirp.Core.Application;

public record UpdateCheepRequest(int CheepId, int RequesterId, string Text, string ETag);
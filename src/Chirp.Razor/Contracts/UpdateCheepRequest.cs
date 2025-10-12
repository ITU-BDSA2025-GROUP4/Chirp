namespace Chirp.Razor.Contracts;

public sealed record UpdateCheepRequest(string Text, string ETagBase64);

namespace Chirp.Razor.Application.Contracts;

public sealed record UpdateCheepCommand(
    int Id,
    string Text,
    string ETagBase64
);
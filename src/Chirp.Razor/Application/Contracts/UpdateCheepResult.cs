namespace Chirp.Razor.Application.Contracts;

public sealed record UpdateCheepResult(
    UpdateOutcome Outcome,
    int? Id = null,
    string? Text = null,
    string? NewETagBase64 = null
);
namespace Chirp.Core.Application.Contracts;

public enum AppStatus
{
    Success, // maps to 200 / 201 / 204 depending on action
    NotFound, // maps to 404
    Conflict, // maps to 409 (ETag mismatch)
    ValidationError // maps to 400
}

public sealed record AppResult<T>(
    AppStatus Status,
    T? Value = default,
    string? ETag = null, // fresh ETag when relevant
    string? Message = null,
    IReadOnlyDictionary<string, string[]>? Errors = null
)
{
    public static AppResult<T> Ok(T value, string? etag = null)
        => new(AppStatus.Success, value, etag);

    public static AppResult<T> Created(T value, string? etag = null)
        => new(AppStatus.Success, value, etag);

    public static AppResult<T> Conflict(string? message = null)
        => new(AppStatus.Conflict, default, null, message);

    public static AppResult<T> NotFound(string? message = null)
        => new(AppStatus.NotFound, default, null, message);

    public static AppResult<T> Invalid(
        IReadOnlyDictionary<string, string[]> errors, string? message = null)
        => new(AppStatus.ValidationError, default, null, message, errors);
}
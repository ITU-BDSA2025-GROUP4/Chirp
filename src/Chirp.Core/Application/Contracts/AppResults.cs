namespace Chirp.Core.Application.Contracts
{
    public enum AppStatus
    {
        Ok,             // 200
        Created,        // 201
        NoContent,      // 204
        NotFound,       // 404
        Conflict,       // 409
        ValidationError // 400
    }

    public sealed record AppResult(
        AppStatus Status,
        string? ETag = null,
        string? Message = null,
        IReadOnlyDictionary<string, string[]>? Errors = null)
    {
        public static AppResult Ok(string? etag = null, string? message = null)
            => new(AppStatus.Ok, etag, message);

        public static AppResult Created(string? etag = null, string? message = null)
            => new(AppStatus.Created, etag, message);

        public static AppResult NoContent(string? etag = null)
            => new(AppStatus.NoContent, etag);

        public static AppResult NotFound(string? message = null)
            => new(AppStatus.NotFound, null, message);

        public static AppResult Conflict(string? message = null)
            => new(AppStatus.Conflict, null, message);

        public static AppResult Invalid(string? message = null,
            IReadOnlyDictionary<string, string[]>? errors = null)
            => new(AppStatus.ValidationError, null, message, errors);

        public bool IsSuccess =>
            Status is AppStatus.Ok or AppStatus.Created or AppStatus.NoContent;

        public bool IsError => !IsSuccess;

        public void ThrowIfError()
        {
            if (!IsError) return;

            throw new AppResultException(Status, Message ?? "An error occurred.", Errors);
        }
    }

    public sealed record AppResult<T>(
        AppStatus Status,
        T? Value = default,
        string? ETag = null,
        string? Message = null,
        IReadOnlyDictionary<string, string[]>? Errors = null)
    {
        public static AppResult<T> Ok(T value, string? etag = null)
            => new(AppStatus.Ok, value, etag);

        public static AppResult<T> Created(T value, string? etag = null)
            => new(AppStatus.Created, value, etag);

        public static AppResult<T> NoContent(string? etag = null)
            => new(AppStatus.NoContent, default, etag);

        public static AppResult<T> NotFound(string? message = null)
            => new(AppStatus.NotFound, default, null, message);

        public static AppResult<T> Conflict(string? message = null)
            => new(AppStatus.Conflict, default, null, message);

        public static AppResult<T> Invalid(string? message = null,
            IReadOnlyDictionary<string, string[]>? errors = null)
            => new(AppStatus.ValidationError, default, null, message, errors);

        public bool IsSuccess =>
            Status is AppStatus.Ok or AppStatus.Created or AppStatus.NoContent;

        public bool IsError => !IsSuccess;

        public T ThrowIfError()
        {
            if (IsSuccess && Value is not null)
                return Value;

            throw new AppResultException(Status, Message ?? "An error occurred.", Errors);
        }
    }

    public sealed class AppResultException(
        AppStatus status,
        string message,
        IReadOnlyDictionary<string, string[]>? errors = null)
        : Exception(message)
    {
        private AppStatus Status { get; } = status;
        private IReadOnlyDictionary<string, string[]>? Errors { get; } = errors;

        public override string ToString()
            => $"{base.ToString()}\nStatus: {Status}\nErrors: {FormatErrors()}";

        private string FormatErrors() =>
            Errors is null
                ? "none"
                : string.Join(", ",
                    Errors.Select(kv => $"{kv.Key}: [{string.Join(", ", kv.Value)}]"));
    }
}
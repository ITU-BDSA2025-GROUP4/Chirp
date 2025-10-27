﻿namespace Chirp.Core.Application.Contracts
{
    public enum AppStatus
    {
        Success,         // 200 / 201 / 204 
        NotFound,        // 404
        Conflict,        // 409 
        ValidationError  // 400
    }

    // For commands that do not return a body
    public sealed record AppResult(
        AppStatus Status,
        string? ETag = null,   
        string? Message = null,
        IReadOnlyDictionary<string, string[]>? Errors = null
    )
    {
       
        public static AppResult Ok(string? etag = null, string? message = null)
            => new(AppStatus.Success, etag, message);

      public static AppResult NoContent(string? etag = null)
            => new(AppStatus.Success, etag);

        public static AppResult NotFound(string? message = null)
            => new(AppStatus.NotFound, null, message);

        public static AppResult Conflict(string? message = null)
            => new(AppStatus.Conflict, null, message);

        public static AppResult Invalid(IReadOnlyDictionary<string, string[]> errors, string? message = null)
            => new(AppStatus.ValidationError, null, message, errors);
    }

    // For queries or commands that return a resource/view-model
    public sealed record AppResult<T>(
        AppStatus Status,
        T? Value = default,
        string? ETag = null,   
        string? Message = null,
        IReadOnlyDictionary<string, string[]>? Errors = null
    )
    {
        public static AppResult<T> Ok(T value, string? etag = null)
            => new(AppStatus.Success, value, etag);

        public static AppResult<T> Created(T value, string? etag = null)
            => new(AppStatus.Success, value, etag);

        public static AppResult<T> NotFound(string? message = null)
            => new(AppStatus.NotFound, default, null, message);

        public static AppResult<T> Conflict(string? message = null)
            => new(AppStatus.Conflict, default, null, message);

        public static AppResult<T> Invalid(IReadOnlyDictionary<string, string[]> errors, string? message = null)
            => new(AppStatus.ValidationError, default, null, message, errors);
    }
}

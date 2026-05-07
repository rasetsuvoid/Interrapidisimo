namespace Interrapidisimo.Application.Common.Models;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];
    public string? TraceId { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public static ApiResponse<T> Ok(T data, string? message = null, string? traceId = null) =>
        new() { Success = true, Data = data, Message = message, TraceId = traceId };

    public static ApiResponse<T> Fail(string error, string? traceId = null) =>
        new() { Success = false, Errors = [error], TraceId = traceId };

    public static ApiResponse<T> Fail(IEnumerable<string> errors, string? traceId = null) =>
        new() { Success = false, Errors = errors.ToList(), TraceId = traceId };
}

public class ApiResponse
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];
    public string? TraceId { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public static ApiResponse Ok(string? message = null, string? traceId = null) =>
        new() { Success = true, Message = message, TraceId = traceId };

    public static ApiResponse Fail(string error, string? traceId = null) =>
        new() { Success = false, Errors = [error], TraceId = traceId };

    public static ApiResponse Fail(IEnumerable<string> errors, string? traceId = null) =>
        new() { Success = false, Errors = errors.ToList(), TraceId = traceId };
}

namespace PayrollApi.Application.Common.Models;

/// <summary>
/// Standardized API response envelope used for ALL endpoints.
/// Success:  { success: true,  data: T,    meta: {...} }
/// Failure:  { success: false, errors: [], traceId: "" }
/// </summary>
public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }
    public ApiMeta? Meta { get; init; }
    public IEnumerable<ApiError>? Errors { get; init; }
    public string? TraceId { get; init; }

    public static ApiResponse<T> Ok(T data, string? message = null, ApiMeta? meta = null) =>
        new() { Success = true, Data = data, Message = message, Meta = meta };

    public static ApiResponse<T> Fail(string error, string? traceId = null) =>
        new() { Success = false, Errors = new[] { new ApiError("GENERAL", error) }, TraceId = traceId };

    public static ApiResponse<T> Fail(IEnumerable<ApiError> errors, string? traceId = null) =>
        new() { Success = false, Errors = errors, TraceId = traceId };
}

public sealed record ApiMeta(int Page, int PageSize, int TotalCount, int TotalPages);

public sealed record ApiError(string Code, string Message, string? Field = null);

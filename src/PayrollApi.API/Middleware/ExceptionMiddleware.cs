using System.Net;
using System.Text.Json;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.Common.Models;
using ValidationException = PayrollApi.Application.Common.Exceptions.ValidationException;

namespace PayrollApi.API.Middleware;

/// <summary>
/// Global exception middleware. Catches all unhandled exceptions and returns
/// a standardized RFC 7807-inspired error response.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;

        var (statusCode, response) = exception switch
        {
            NotFoundException ex => (
                HttpStatusCode.NotFound,
                ApiResponse<object>.Fail(ex.Message, traceId)),

            ConflictException ex => (
                HttpStatusCode.Conflict,
                ApiResponse<object>.Fail(ex.Message, traceId)),

            ForbiddenException ex => (
                HttpStatusCode.Forbidden,
                ApiResponse<object>.Fail(ex.Message, traceId)),

            ValidationException ex => (
                HttpStatusCode.UnprocessableEntity,
                ApiResponse<object>.Fail(
                    ex.Errors.SelectMany(e => e.Value.Select(msg => new ApiError(e.Key, msg, e.Key))),
                    traceId)),

            AppException ex => (
                HttpStatusCode.BadRequest,
                ApiResponse<object>.Fail(ex.Message, traceId)),

            _ => (
                HttpStatusCode.InternalServerError,
                ApiResponse<object>.Fail(
                    _env.IsDevelopment() ? exception.Message : "An unexpected error occurred. Please try again.",
                    traceId))
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", traceId);
        else
            _logger.LogWarning(exception, "Handled exception {ExceptionType}. TraceId: {TraceId}", exception.GetType().Name, traceId);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}

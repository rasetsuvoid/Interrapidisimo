using Interrapidisimo.Application.Common.Models;
using Interrapidisimo.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Interrapidisimo.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var traceId = context.TraceIdentifier;
            logger.LogError(ex, "Excepción no controlada. TraceId: {TraceId}", traceId);
            await HandleExceptionAsync(context, ex, traceId);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception, string traceId)
    {
        var (statusCode, message) = exception switch
        {
            BusinessRuleException => (HttpStatusCode.UnprocessableEntity, exception.Message),
            NotFoundException => (HttpStatusCode.NotFound, exception.Message),
            ConflictException => (HttpStatusCode.Conflict, exception.Message),
            Domain.Exceptions.ValidationException => (HttpStatusCode.BadRequest, exception.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Acceso no autorizado."),
            _ => (HttpStatusCode.InternalServerError, "Ocurrió un error inesperado. Intenta de nuevo más tarde.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse.Fail(message, traceId);
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}

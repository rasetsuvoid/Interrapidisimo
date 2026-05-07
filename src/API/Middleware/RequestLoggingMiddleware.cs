using System.Diagnostics;

namespace Interrapidisimo.API.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        var traceId = context.TraceIdentifier;

        logger.LogInformation(
            "HTTP {Method} {Path} started. TraceId: {TraceId}",
            context.Request.Method, context.Request.Path, traceId);

        await next(context);

        sw.Stop();
        logger.LogInformation(
            "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms. TraceId: {TraceId}",
            context.Request.Method, context.Request.Path,
            context.Response.StatusCode, sw.ElapsedMilliseconds, traceId);
    }
}

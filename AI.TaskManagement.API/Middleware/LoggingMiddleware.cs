using Serilog;

namespace AI.TaskManagement.API.Middleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;

    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Log.Information("HTTP {Method} {Path} started", context.Request.Method, context.Request.Path);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await _next(context);
        stopwatch.Stop();

        Log.Information("HTTP {Method} {Path} completed with status {StatusCode} in {ElapsedMilliseconds}ms",
            context.Request.Method, context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
    }
}

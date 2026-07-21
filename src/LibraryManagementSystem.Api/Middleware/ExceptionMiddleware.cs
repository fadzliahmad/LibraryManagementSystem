using System.Net;
using System.Text.Json;
using LibraryManagementSystem.Core.Interfaces;

namespace LibraryManagementSystem.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IAppLogService appLogService)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");

            // Log to database
            appLogService.LogError(
                message: "Unhandled exception in request pipeline",
                exception: ex,
                source: nameof(ExceptionMiddleware),
                requestPath: context.Request.Path,
                requestMethod: context.Request.Method
            );

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var payload = JsonSerializer.Serialize(new
            {
                error = "An unexpected error occurred. Please try again later."
            });

            await context.Response.WriteAsync(payload);
        }
    }
}

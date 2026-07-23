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

            // Handle specific exceptions gracefully
            var exceptionResponse = ExceptionHandlerService.HandleException(ex);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = GetStatusCode(exceptionResponse.ErrorCode);

            var payload = JsonSerializer.Serialize(exceptionResponse);

            await context.Response.WriteAsync(payload);
        }
    }

    private static int GetStatusCode(string errorCode)
    {
        return errorCode switch
        {
            "BOOK_HAS_ACTIVE_LOANS" => (int)HttpStatusCode.BadRequest, // 400
            "CONSTRAINT_VIOLATION" => (int)HttpStatusCode.BadRequest, // 400
            "DATABASE_UPDATE_ERROR" => (int)HttpStatusCode.BadRequest, // 400
            _ => (int)HttpStatusCode.InternalServerError // 500
        };
    }
}

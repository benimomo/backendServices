using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Constants;

namespace Shared.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred.");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = new ApiResponse<object>
            {
                Success = false,
                Message = CommonMessages.UnexpectedError,
                ErrorCode = "SERVER_ERROR"
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
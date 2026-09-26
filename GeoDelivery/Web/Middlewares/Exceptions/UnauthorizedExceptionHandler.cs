using Auth.Application.CustomExceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Web.Middlewares.Exceptions;

public class UnauthorizedExceptionHandler(ILogger<UnauthorizedExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not UnauthorizedException unauthorizedException)
        {
            return false;
        }

        logger.LogWarning("Unauthorized exception: {Message}", unauthorizedException.Message);

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

        var response = new
        {
            status = StatusCodes.Status401Unauthorized,
            title = "Unauthorized",
            message = unauthorizedException.Message
        };

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
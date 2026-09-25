using Auth.Application.CustomExceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Web.Middlewares.Exceptions;

public class UserCreateFailedExceptionHandler(
    ILogger<UserCreateFailedExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
       
        if (exception is not UserCreateFailedException userCreateFailedException)
        {
            return false;
        }

        logger.LogWarning("User create failed exception: {Message}", userCreateFailedException.Message);

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        var response = new
        {
            status = StatusCodes.Status400BadRequest,
            userCreateFailedException
        };

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true; 
        
    }
}
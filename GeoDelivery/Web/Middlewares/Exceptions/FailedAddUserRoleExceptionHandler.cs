using Auth.Application.CustomExceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Web.Middlewares.Exceptions;

public class FailedAddUserRoleExceptionHandler(
    ILogger<FailedAddUserRoleExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
       
        if (exception is not FailedAddUserRoleException failedAddUserRoleException)
        {
            return false;
        }

        logger.LogWarning("User create failed exception: {Message}", failedAddUserRoleException.Message);

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new
        {
            status = StatusCodes.Status500InternalServerError,
            failedAddUserRoleException
        };

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true; 
        
    }
}
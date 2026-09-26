using Auth.Application.CustomExceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Web.Middlewares.Exceptions;

public class EmailIsAlreadyInUseExceptionHandler(
    ILogger<EmailIsAlreadyInUseExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
       
        if (exception is not EmailIsAlreadyInUseException emailIsAlreadyInUseException)
        {
            return false;
        }

        logger.LogWarning("User create failed exception: {Message}", emailIsAlreadyInUseException.Message);

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        var response = new
        {
            status = StatusCodes.Status400BadRequest,
            emailIsAlreadyInUseException
        };

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true; 
        
    }
}
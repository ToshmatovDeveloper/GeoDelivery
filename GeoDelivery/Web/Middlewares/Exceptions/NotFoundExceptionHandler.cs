using Catalog.Application.CustomExceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Web.Middlewares.Exceptions;

public class NotFoundExceptionHandler(ILogger<NotFoundExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
       
        if (exception is not NotFoundException notFoundException)
        {
            return false;
        }

        logger.LogWarning("Not found exception: {Message}", notFoundException.Message);

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

        var response = new
        {
            status = StatusCodes.Status404NotFound,
            message = notFoundException.Message
        };

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true; 
        
    }
}
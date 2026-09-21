using Catalog.Application.CustomExceptions;
using FluentValidation;
using MediatR;

namespace Catalog.Application.Validation;

public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);

        var validationResult = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        
        var errors = validationResult
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .Select(f => new ValidationErrors(
                f.PropertyName,
                f.ErrorMessage))
            .ToList();

        if (errors.Count != 0)
        {
            var errorMessage = string.Join("; ", errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
            throw new BadRequestException(errorMessage);
        }
        
        return await next(cancellationToken);
    }
}
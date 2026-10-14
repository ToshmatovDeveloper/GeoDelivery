using FluentValidation;
using Geo.Application.Features.RedisGeo;

namespace Geo.Application.Validation.ValidatorsOfCommands;

public class RemoveCourierCommandValidator : AbstractValidator<RemoveCourierCommand>
{
    public RemoveCourierCommandValidator()
    {
        RuleFor(x => x.CourierId)
            .NotEmpty().WithMessage("Courier ID cannot be empty.");
    }
}
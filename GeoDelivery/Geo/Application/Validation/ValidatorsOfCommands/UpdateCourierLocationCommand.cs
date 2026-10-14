using FluentValidation;
using Geo.Application.Features.RedisGeo;

namespace Geo.Application.Validation.ValidatorsOfCommands;

public class UpdateCourierLocationCommandValidator : AbstractValidator<UpdateCourierLocationCommand>
{
    public UpdateCourierLocationCommandValidator()
    {
        RuleFor(x => x.CourierId)
            .NotEmpty().WithMessage("Courier ID cannot be empty.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude must be between -90 and 90 degrees.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude must be between -180 and 180 degrees.");
    }
}
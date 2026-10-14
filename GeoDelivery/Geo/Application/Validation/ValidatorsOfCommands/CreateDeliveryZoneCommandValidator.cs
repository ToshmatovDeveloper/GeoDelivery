using FluentValidation;
using Geo.Application.Features.PostGisGeo;

namespace Geo.Application.Validation.ValidatorsOfCommands;

public class CreateDeliveryZoneCommandValidator : AbstractValidator<CreateDeliveryZoneCommand>
{
    public CreateDeliveryZoneCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Zone name is required.")
            .MaximumLength(100).WithMessage("Zone name must not exceed 100 characters.");

        RuleFor(x => x.Points)
            .NotNull().WithMessage("Points list is required.")
            .Must(points => points != null && points.Count >= 3)
            .WithMessage("A delivery zone polygon must contain at least 3 points.");

        RuleForEach(x => x.Points).ChildRules(point =>
        {
            point.RuleFor(p => p.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be between -90 and 90 degrees.");

            point.RuleFor(p => p.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be between -180 and 180 degrees.");
        });
    }
}
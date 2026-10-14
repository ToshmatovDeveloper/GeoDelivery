using FluentValidation;
using Geo.Application.Features.PostGisGeo;

namespace Geo.Application.Validation.QueriesValidators;

public class CheckPointInZoneQueryValidator : AbstractValidator<CheckPointInZoneQuery>
{
    public CheckPointInZoneQueryValidator()
    {
        RuleFor(x => x.ZoneId)
            .NotEmpty().WithMessage("Zone ID cannot be empty.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude must be between -90 and 90 degrees.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude must be between -180 and 180 degrees.");
    }
}
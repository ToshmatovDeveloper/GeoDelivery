using FluentValidation;
using Geo.Application.Features.PostGisGeo;

namespace Geo.Application.Validation.QueriesValidators;

public class CalculateDistanceQueryValidator : AbstractValidator<CalculateDistanceQuery>
{
    public CalculateDistanceQueryValidator()
    {
        RuleFor(x => x.Latitude1)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude 1 must be between -90 and 90 degrees.");

        RuleFor(x => x.Longitude1)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude 1 must be between -180 and 180 degrees.");

        RuleFor(x => x.Latitude2)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude 2 must be between -90 and 90 degrees.");

        RuleFor(x => x.Longitude2)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude 2 must be between -180 and 180 degrees.");
    }
}
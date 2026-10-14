using FluentValidation;
using Geo.Application.Features.RedisGeo;

namespace Geo.Application.Validation.QueriesValidators;

public class GetNearbyCouriersQueryValidator : AbstractValidator<GetNearbyCouriersQuery>
{
    public GetNearbyCouriersQueryValidator()
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude must be between -90 and 90 degrees.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude must be between -180 and 180 degrees.");

        RuleFor(x => x.RadiusInKm)
            .GreaterThan(0)
            .WithMessage("Search radius must be greater than 0 km.");
    }
}
using FluentValidation;
using Geo.Application.Features.PostGisGeo;

namespace Geo.Application.Validation.QueriesValidators;

public class GetNearestDeliveryZoneQueryValidator : AbstractValidator<GetNearestDeliveryZoneQuery>
{
    public GetNearestDeliveryZoneQueryValidator()
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude must be between -90 and 90 degrees.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude must be between -180 and 180 degrees.");
    }
}
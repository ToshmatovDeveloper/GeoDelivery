using Catalog.Application.Features.Restaurant.Command;
using FluentValidation;

namespace Catalog.Application.Validation.Restaurant;

public class DeleteRestaurantCommandValidator : AbstractValidator<DeleteRestaurantCommand>
{
    public DeleteRestaurantCommandValidator()
    {
        RuleFor(command => command.RestaurantId)
            .NotEmpty()
            .WithMessage("RestaurantId is required.");
    }
}
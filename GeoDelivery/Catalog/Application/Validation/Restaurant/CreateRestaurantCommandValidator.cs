using Catalog.Application.Features.Restaurant.Command;
using FluentValidation;

namespace Catalog.Application.Validation.Restaurant;

public class CreateRestaurantCommandValidator : AbstractValidator<CreateRestaurantCommand>
{
    public CreateRestaurantCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage("Restaurant name is required.")
            .MaximumLength(150)
            .WithMessage("Restaurant name must not exceed 150 characters.");

        RuleFor(command => command.Description)
            .MaximumLength(500)
            .WithMessage("Restaurant description must not exceed 500 characters.");
    }
}
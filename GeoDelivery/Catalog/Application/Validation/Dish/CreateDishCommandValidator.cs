using Catalog.Application.Features.Dish.Command;
using FluentValidation;

namespace Catalog.Application.Validation.Dish;

public class CreateDishCommandValidator : AbstractValidator<CreateDishCommand>
{
    public CreateDishCommandValidator()
    {
        RuleFor(command => command.RestaurantId)
            .NotEmpty()
            .WithMessage("RestaurantId is required.");

        RuleFor(command => command.CategoryId)
            .NotEmpty()
            .WithMessage("CategoryId is required.");

        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage("Dish name is required.")
            .MaximumLength(150)
            .WithMessage("Dish name must not exceed 150 characters.");

        RuleFor(command => command.Price)
            .GreaterThan(0)
            .WithMessage("Dish price must be greater than zero.");
    }
}
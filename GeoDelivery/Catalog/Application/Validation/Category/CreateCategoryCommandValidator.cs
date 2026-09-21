using Catalog.Application.Features.Category.Command;
using FluentValidation;

namespace Catalog.Application.Validation.Category;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(command => command.RestaurantId)
            .NotEmpty()
            .WithMessage("RestaurantId is required.");

        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage("Category name is required.")
            .MaximumLength(100)
            .WithMessage("Category name must not exceed 100 characters.");
    }
}
using Catalog.Application.Features.Category.Command;
using FluentValidation;

namespace Catalog.Application.Validation.Category;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(command => command.CategoryId)
            .NotEmpty()
            .WithMessage("CategoryId is required.");

        RuleFor(command => command.RestaurantId)
            .NotEmpty()
            .WithMessage("RestaurantId is required.");
    }
}
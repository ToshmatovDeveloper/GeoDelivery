using Catalog.Application.Features.Dish.Command;
using FluentValidation;

namespace Catalog.Application.Validation.Dish;

public class MakeDishUnavailableCommandValidator : AbstractValidator<MakeDishUnavailableCommand>
{
    public MakeDishUnavailableCommandValidator()
    {
        RuleFor(command => command.DishId)
            .NotEmpty()
            .WithMessage("DishId is required.");
    }
}
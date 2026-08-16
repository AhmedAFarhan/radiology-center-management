using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Application.Localization;

namespace RadiologyCenter.BuildingBlocks.Application.Validation;

public abstract class EntityIdQueryValidatorBase<T> : AbstractValidator<T> where T : IEntityIdQuery
{
    protected EntityIdQueryValidatorBase() => RuleFor(x => x.Id)
        .NotEmpty()
        .WithErrorCode(ErrorCodes.Shared.IdRequired);
}

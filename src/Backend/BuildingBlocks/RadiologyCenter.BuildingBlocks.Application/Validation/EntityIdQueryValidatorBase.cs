using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.BuildingBlocks.Application.Validation;

public abstract class EntityIdQueryValidatorBase<T> : AbstractValidator<T> where T : IEntityIdQuery
{
    protected EntityIdQueryValidatorBase(string idRequiredCode = "Shared.IdRequired") =>
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithErrorCode(idRequiredCode);
}

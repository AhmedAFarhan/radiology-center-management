using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Catalog.Domain.Enumerations;

namespace RadiologyCenter.Catalog.Application.Commands.Common;

public abstract class ExaminationTypeValidatorBase<T> : AbstractValidator<T>
    where T : IExaminationTypeFields
{
    protected ExaminationTypeValidatorBase()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Modality).NotEmpty().IsEnumerationMember<Modality, T>("Modality");
        RuleFor(x => x.BodyPart).NotEmpty().MaximumLength(200);
        RuleFor(x => x.StandardDurationMinutes).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
    }
}
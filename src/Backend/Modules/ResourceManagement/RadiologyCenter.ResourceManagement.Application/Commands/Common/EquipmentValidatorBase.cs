using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;

namespace RadiologyCenter.ResourceManagement.Application.Commands.Common;

public abstract class EquipmentValidatorBase<T> : AbstractValidator<T> where T : IEquipmentFields
{
    protected EquipmentValidatorBase()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Modality).NotEmpty().IsEnumerationMember<EquipmentModality, T>("Modality");
        RuleFor(x => x.SerialNumber).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.SerialNumber));
    }
}

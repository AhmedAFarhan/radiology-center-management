using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;
using ErrorCodes = RadiologyCenter.ResourceManagement.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Commands.Common;

public abstract class EquipmentValidatorBase<T> : AbstractValidator<T> where T : IEquipmentFields
{
    protected EquipmentValidatorBase()
    {
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.EquipmentNameRequired).MaximumLength(200).WithErrorCode(ErrorCodes.EquipmentNameTooLong);
        RuleFor(x => x.Modality).NotEmpty().WithErrorCode(ErrorCodes.ModalityRequired).IsEnumerationMember<EquipmentModality, T>("Modality");
        RuleFor(x => x.SerialNumber).MaximumLength(100).WithErrorCode(ErrorCodes.SerialNumberTooLong).When(x => !string.IsNullOrWhiteSpace(x.SerialNumber));
    }
}

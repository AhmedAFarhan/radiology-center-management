using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Commands.Common;

public abstract class EquipmentValidatorBase<T> : AbstractValidator<T> where T : IEquipmentFields
{
    protected EquipmentValidatorBase()
    {
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired).MaximumLength(200).WithErrorCode(ErrorCodes.Shared.TextTooLong);
        RuleFor(x => x.Modality).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired).IsEnumerationMember<EquipmentModality, T>("Modality");
        RuleFor(x => x.SerialNumber).MaximumLength(100).WithErrorCode(ErrorCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.SerialNumber));
    }
}

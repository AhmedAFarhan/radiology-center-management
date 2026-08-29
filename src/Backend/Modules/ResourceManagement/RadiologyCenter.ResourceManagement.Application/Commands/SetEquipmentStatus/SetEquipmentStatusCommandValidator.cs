using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;
using ErrorCodes = RadiologyCenter.ResourceManagement.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Commands.SetEquipmentStatus;

public class SetEquipmentStatusCommandValidator : AbstractValidator<SetEquipmentStatusCommand>
{
    public SetEquipmentStatusCommandValidator()
    {
        RuleFor(x => x.EquipmentId).NotEmpty().WithErrorCode(ErrorCodes.EquipmentIdRequired);
        RuleFor(x => x.Status).NotEmpty().WithErrorCode(ErrorCodes.StatusRequired).IsEnumerationMember<EquipmentStatus, SetEquipmentStatusCommand>("Status");
    }
}

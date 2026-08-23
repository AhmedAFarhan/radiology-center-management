using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Commands.SetEquipmentStatus;

public class SetEquipmentStatusCommandValidator : AbstractValidator<SetEquipmentStatusCommand>
{
    public SetEquipmentStatusCommandValidator()
    {
        RuleFor(x => x.EquipmentId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.Status).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).IsEnumerationMember<EquipmentStatus, SetEquipmentStatusCommand>("Status");
    }
}

using FluentValidation;
using RadiologyCenter.ResourceManagement.Application.Commands.Common;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateEquipment;

public class UpdateEquipmentCommandValidator : EquipmentValidatorBase<UpdateEquipmentCommand>
{
    public UpdateEquipmentCommandValidator()
    {
        RuleFor(x => x.EquipmentId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}

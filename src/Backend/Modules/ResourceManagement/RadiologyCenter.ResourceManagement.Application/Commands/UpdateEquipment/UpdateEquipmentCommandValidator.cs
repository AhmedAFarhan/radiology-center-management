using FluentValidation;
using RadiologyCenter.ResourceManagement.Application.Commands.Common;
using ErrorCodes = RadiologyCenter.ResourceManagement.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateEquipment;

public class UpdateEquipmentCommandValidator : EquipmentValidatorBase<UpdateEquipmentCommand>
{
    public UpdateEquipmentCommandValidator()
    {
        RuleFor(x => x.EquipmentId).NotEmpty().WithErrorCode(ErrorCodes.EquipmentIdRequired);
    }
}

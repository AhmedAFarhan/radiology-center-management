using FluentValidation;
using RadiologyCenter.ResourceManagement.Application.Commands.Common;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateWorkShift;

public class UpdateWorkShiftCommandValidator : WorkShiftValidatorBase<UpdateWorkShiftCommand>
{
    public UpdateWorkShiftCommandValidator()
    {
        RuleFor(x => x.WorkShiftId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}

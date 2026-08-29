using FluentValidation;
using RadiologyCenter.ResourceManagement.Application.Commands.Common;
using ErrorCodes = RadiologyCenter.ResourceManagement.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateLeave;

public class UpdateLeaveCommandValidator : LeaveValidatorBase<UpdateLeaveCommand>
{
    public UpdateLeaveCommandValidator()
    {
        RuleFor(x => x.LeaveId).NotEmpty().WithErrorCode(ErrorCodes.LeaveIdRequired);
    }
}

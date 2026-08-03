using FluentValidation;
using RadiologyCenter.ResourceManagement.Application.Commands.Common;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateLeave;

public class UpdateLeaveCommandValidator : LeaveValidatorBase<UpdateLeaveCommand>
{
    public UpdateLeaveCommandValidator()
    {
        RuleFor(x => x.LeaveId).NotEmpty();
    }
}

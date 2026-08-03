using FluentValidation;
using RadiologyCenter.ResourceManagement.Application.Commands.Common;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateWorkShift;

public class UpdateWorkShiftCommandValidator : WorkShiftValidatorBase<UpdateWorkShiftCommand>
{
    public UpdateWorkShiftCommandValidator()
    {
        RuleFor(x => x.WorkShiftId).NotEmpty();
    }
}

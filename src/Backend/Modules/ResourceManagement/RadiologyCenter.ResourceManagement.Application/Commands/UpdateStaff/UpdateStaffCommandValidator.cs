using FluentValidation;
using RadiologyCenter.ResourceManagement.Application.Commands.Common;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateStaff;

public class UpdateStaffCommandValidator : StaffValidatorBase<UpdateStaffCommand>
{
    public UpdateStaffCommandValidator()
    {
        RuleFor(x => x.StaffId).NotEmpty();
    }
}

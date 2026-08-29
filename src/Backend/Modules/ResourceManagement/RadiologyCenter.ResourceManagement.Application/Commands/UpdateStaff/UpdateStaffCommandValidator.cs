using FluentValidation;
using RadiologyCenter.ResourceManagement.Application.Commands.Common;
using ErrorCodes = RadiologyCenter.ResourceManagement.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateStaff;

public class UpdateStaffCommandValidator : StaffValidatorBase<UpdateStaffCommand>
{
    public UpdateStaffCommandValidator()
    {
        RuleFor(x => x.StaffId).NotEmpty().WithErrorCode(ErrorCodes.StaffIdRequired);
    }
}

using FluentValidation;
using RadiologyCenter.ResourceManagement.Application.Commands.Common;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateStaff;

public class UpdateStaffCommandValidator : StaffValidatorBase<UpdateStaffCommand>
{
    public UpdateStaffCommandValidator()
    {
        RuleFor(x => x.StaffId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}

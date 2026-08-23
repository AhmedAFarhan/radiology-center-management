using FluentValidation;
using RadiologyCenter.ResourceManagement.Application.Localization;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Commands.Common;

public abstract class LeaveValidatorBase<T> : AbstractValidator<T> where T : ILeaveFields
{
    protected LeaveValidatorBase()
    {
        RuleFor(x => x.StaffId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.LeaveType).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).IsEnumerationMember<LeaveType, T>("Leave type");
        RuleFor(x => x.StartDate).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired);
        RuleFor(x => x.EndDate).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate).WithErrorCode(ErrorCodes.LeaveEndOnOrAfterStart);
        RuleFor(x => x.Reason).MaximumLength(500).WithErrorCode(SharedCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.Reason));
    }
}

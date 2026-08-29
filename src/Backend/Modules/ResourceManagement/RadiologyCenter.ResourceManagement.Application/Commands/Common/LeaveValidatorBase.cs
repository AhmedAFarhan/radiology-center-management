using FluentValidation;
using RadiologyCenter.ResourceManagement.Application.Localization;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;
using ErrorCodes = RadiologyCenter.ResourceManagement.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Commands.Common;

public abstract class LeaveValidatorBase<T> : AbstractValidator<T> where T : ILeaveFields
{
    protected LeaveValidatorBase()
    {
        RuleFor(x => x.StaffId).NotEmpty().WithErrorCode(ErrorCodes.StaffIdRequired);
        RuleFor(x => x.LeaveType).NotEmpty().WithErrorCode(ErrorCodes.LeaveTypeRequired).IsEnumerationMember<LeaveType, T>("Leave type");
        RuleFor(x => x.StartDate).NotEmpty().WithErrorCode(ErrorCodes.StartDateRequired);
        RuleFor(x => x.EndDate).NotEmpty().WithErrorCode(ErrorCodes.EndDateRequired);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate).WithErrorCode(ErrorCodes.LeaveEndOnOrAfterStart);
        RuleFor(x => x.Reason).MaximumLength(500).WithErrorCode(ErrorCodes.ReasonTooLong).When(x => !string.IsNullOrWhiteSpace(x.Reason));
    }
}

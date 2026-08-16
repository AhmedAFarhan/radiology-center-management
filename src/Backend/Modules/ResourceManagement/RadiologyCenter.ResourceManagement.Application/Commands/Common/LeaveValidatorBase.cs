using FluentValidation;
using RadiologyCenter.ResourceManagement.Application.Localization;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;

namespace RadiologyCenter.ResourceManagement.Application.Commands.Common;

public abstract class LeaveValidatorBase<T> : AbstractValidator<T> where T : ILeaveFields
{
    protected LeaveValidatorBase()
    {
        RuleFor(x => x.StaffId).NotEmpty();
        RuleFor(x => x.LeaveType).NotEmpty().IsEnumerationMember<LeaveType, T>("Leave type");
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty();
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate).WithErrorCode(ErrorCodes.LeaveEndOnOrAfterStart);
        RuleFor(x => x.Reason).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Reason));
    }
}

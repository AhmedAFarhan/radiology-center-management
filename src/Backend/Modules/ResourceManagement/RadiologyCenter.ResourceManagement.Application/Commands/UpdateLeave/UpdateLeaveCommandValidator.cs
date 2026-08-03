using FluentValidation;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateLeave;

public class UpdateLeaveCommandValidator : AbstractValidator<UpdateLeaveCommand>
{
    public UpdateLeaveCommandValidator()
    {
        RuleFor(x => x.LeaveId).NotEmpty();
        RuleFor(x => x.StaffId).NotEmpty();
        RuleFor(x => x.LeaveType).NotEmpty().Must(IsValidLeaveType)
            .WithMessage("Leave type must be one of: Annual, Sick, Unpaid, Maternity, Other.");
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty();
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be on or after start date.");
        RuleFor(x => x.Reason).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Reason));
    }

    private static bool IsValidLeaveType(string leaveType) =>
        LeaveType.GetAll<LeaveType>().Any(t => t.Name.Equals(leaveType, StringComparison.OrdinalIgnoreCase));
}

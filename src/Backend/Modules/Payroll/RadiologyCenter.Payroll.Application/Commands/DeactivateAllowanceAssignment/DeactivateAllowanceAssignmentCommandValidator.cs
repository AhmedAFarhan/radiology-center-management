using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.DeactivateAllowanceAssignment;

public class DeactivateAllowanceAssignmentCommandValidator : AbstractValidator<DeactivateAllowanceAssignmentCommand>
{
    public DeactivateAllowanceAssignmentCommandValidator() => RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.AllowanceAssignmentIdRequired);
}
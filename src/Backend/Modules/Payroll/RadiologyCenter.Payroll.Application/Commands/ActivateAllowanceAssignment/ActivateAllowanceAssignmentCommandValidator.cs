using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.ActivateAllowanceAssignment;

public class ActivateAllowanceAssignmentCommandValidator : AbstractValidator<ActivateAllowanceAssignmentCommand>
{
    public ActivateAllowanceAssignmentCommandValidator() => RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.AllowanceAssignmentIdRequired);
}
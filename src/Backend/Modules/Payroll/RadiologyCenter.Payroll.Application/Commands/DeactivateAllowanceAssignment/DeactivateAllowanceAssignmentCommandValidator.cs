using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Commands.DeactivateAllowanceAssignment;

public class DeactivateAllowanceAssignmentCommandValidator : AbstractValidator<DeactivateAllowanceAssignmentCommand>
{
    public DeactivateAllowanceAssignmentCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}
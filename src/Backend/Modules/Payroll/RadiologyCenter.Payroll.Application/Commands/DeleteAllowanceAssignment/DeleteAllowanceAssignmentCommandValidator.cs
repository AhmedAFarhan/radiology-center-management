using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Commands.DeleteAllowanceAssignment;

public class DeleteAllowanceAssignmentCommandValidator : AbstractValidator<DeleteAllowanceAssignmentCommand>
{
    public DeleteAllowanceAssignmentCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}
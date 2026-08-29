using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.DeleteAllowanceAssignment;

public class DeleteAllowanceAssignmentCommandValidator : AbstractValidator<DeleteAllowanceAssignmentCommand>
{
    public DeleteAllowanceAssignmentCommandValidator() => RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.AllowanceAssignmentIdRequired);
}
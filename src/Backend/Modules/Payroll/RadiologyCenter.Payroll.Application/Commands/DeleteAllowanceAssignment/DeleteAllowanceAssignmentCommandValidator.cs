using FluentValidation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.DeleteAllowanceAssignment;

public class DeleteAllowanceAssignmentCommandValidator : AbstractValidator<DeleteAllowanceAssignmentCommand>
{
    public DeleteAllowanceAssignmentCommandValidator() => RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired);
}
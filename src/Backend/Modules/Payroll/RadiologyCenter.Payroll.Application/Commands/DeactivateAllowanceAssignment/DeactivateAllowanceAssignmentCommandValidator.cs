using FluentValidation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.DeactivateAllowanceAssignment;

public class DeactivateAllowanceAssignmentCommandValidator : AbstractValidator<DeactivateAllowanceAssignmentCommand>
{
    public DeactivateAllowanceAssignmentCommandValidator() => RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired);
}
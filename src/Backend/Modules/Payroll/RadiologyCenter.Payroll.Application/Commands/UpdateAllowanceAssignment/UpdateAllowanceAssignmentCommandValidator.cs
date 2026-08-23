using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Domain.Enumerations;
using RadiologyCenter.Payroll.Application.Localization;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.UpdateAllowanceAssignment;

public class UpdateAllowanceAssignmentCommandValidator : AbstractValidator<UpdateAllowanceAssignmentCommand>
{
    public UpdateAllowanceAssignmentCommandValidator()
    {
        RuleFor(x => x.AllowanceAssignmentId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(100).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0).WithErrorCode(SharedCodes.Shared.CannotBeNegative);
        RuleFor(x => x.EffectiveDate).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired);
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.EffectiveDate)
            .WithErrorCode(ErrorCodes.EndDateOnOrAfterEffectiveDate)
            .When(x => x.EndDate.HasValue);
        RuleFor(x => x.Frequency).IsEnumerationMemberOrEmpty<Frequency, UpdateAllowanceAssignmentCommand>("Frequency");
    }
}
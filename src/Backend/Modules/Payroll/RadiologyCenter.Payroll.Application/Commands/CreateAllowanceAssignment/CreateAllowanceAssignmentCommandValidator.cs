using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Domain.Enumerations;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.CreateAllowanceAssignment;

public class CreateAllowanceAssignmentCommandValidator : AbstractValidator<CreateAllowanceAssignmentCommand>
{
    public CreateAllowanceAssignmentCommandValidator()
    {
        RuleFor(x => x.StaffId).NotEmpty().WithErrorCode(ErrorCodes.StaffIdRequired);
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.NameRequired).MaximumLength(100).WithErrorCode(ErrorCodes.NameTooLong);
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.AmountCannotBeNegative);
        RuleFor(x => x.EffectiveDate).NotEmpty().WithErrorCode(ErrorCodes.EffectiveDateRequired);
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.EffectiveDate)
            .WithErrorCode(ErrorCodes.EndDateOnOrAfterEffectiveDate)
            .When(x => x.EndDate.HasValue);
        RuleFor(x => x.Frequency).IsEnumerationMemberOrEmpty<Frequency, CreateAllowanceAssignmentCommand>("Frequency");
    }
}
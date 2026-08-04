using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Application.Commands.CreateAllowanceAssignment;

public class CreateAllowanceAssignmentCommandValidator : AbstractValidator<CreateAllowanceAssignmentCommand>
{
    public CreateAllowanceAssignmentCommandValidator()
    {
        RuleFor(x => x.StaffId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.EffectiveDate).NotEmpty();
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.EffectiveDate)
            .When(x => x.EndDate.HasValue);
        RuleFor(x => x.Frequency).IsEnumerationMemberOrEmpty<Frequency, CreateAllowanceAssignmentCommand>("Frequency");
    }
}
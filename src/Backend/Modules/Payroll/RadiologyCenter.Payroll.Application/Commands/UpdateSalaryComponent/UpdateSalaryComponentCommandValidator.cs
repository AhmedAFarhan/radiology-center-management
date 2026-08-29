using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Domain.Enumerations;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.UpdateSalaryComponent;

public class UpdateSalaryComponentCommandValidator : AbstractValidator<UpdateSalaryComponentCommand>
{
    public UpdateSalaryComponentCommandValidator()
    {
        RuleFor(x => x.SalaryComponentId).NotEmpty().WithErrorCode(ErrorCodes.SalaryComponentIdRequired);
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.NameRequired).MaximumLength(100).WithErrorCode(ErrorCodes.NameTooLong);
        RuleFor(x => x.Kind).NotEmpty().WithErrorCode(ErrorCodes.KindRequired).IsEnumerationMember<ComponentKind, UpdateSalaryComponentCommand>("Kind");
        RuleFor(x => x.DefaultValue).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.DefaultValueCannotBeNegative);
        RuleFor(x => x.Frequency).IsEnumerationMemberOrEmpty<Frequency, UpdateSalaryComponentCommand>("Frequency");
    }
}
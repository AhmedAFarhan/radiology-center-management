using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Domain.Enumerations;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.CreateSalaryComponent;

public class CreateSalaryComponentCommandValidator : AbstractValidator<CreateSalaryComponentCommand>
{
    public CreateSalaryComponentCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.NameRequired).MaximumLength(100).WithErrorCode(ErrorCodes.NameTooLong);
        RuleFor(x => x.Kind).NotEmpty().WithErrorCode(ErrorCodes.KindRequired).IsEnumerationMember<ComponentKind, CreateSalaryComponentCommand>("Kind");
        RuleFor(x => x.DefaultValue).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.DefaultValueCannotBeNegative);
        RuleFor(x => x.Frequency).IsEnumerationMemberOrEmpty<Frequency, CreateSalaryComponentCommand>("Frequency");
    }
}
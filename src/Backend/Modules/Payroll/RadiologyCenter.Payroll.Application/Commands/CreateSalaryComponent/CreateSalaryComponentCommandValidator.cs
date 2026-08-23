using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Domain.Enumerations;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.CreateSalaryComponent;

public class CreateSalaryComponentCommandValidator : AbstractValidator<CreateSalaryComponentCommand>
{
    public CreateSalaryComponentCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired).MaximumLength(100).WithErrorCode(ErrorCodes.Shared.TextTooLong);
        RuleFor(x => x.Kind).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired).IsEnumerationMember<ComponentKind, CreateSalaryComponentCommand>("Kind");
        RuleFor(x => x.DefaultValue).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.Shared.CannotBeNegative);
        RuleFor(x => x.Frequency).IsEnumerationMemberOrEmpty<Frequency, CreateSalaryComponentCommand>("Frequency");
    }
}
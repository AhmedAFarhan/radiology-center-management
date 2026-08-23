using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Domain.Enumerations;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.UpdateSalaryComponent;

public class UpdateSalaryComponentCommandValidator : AbstractValidator<UpdateSalaryComponentCommand>
{
    public UpdateSalaryComponentCommandValidator()
    {
        RuleFor(x => x.SalaryComponentId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(100).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.Kind).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).IsEnumerationMember<ComponentKind, UpdateSalaryComponentCommand>("Kind");
        RuleFor(x => x.DefaultValue).GreaterThanOrEqualTo(0).WithErrorCode(SharedCodes.Shared.CannotBeNegative);
        RuleFor(x => x.Frequency).IsEnumerationMemberOrEmpty<Frequency, UpdateSalaryComponentCommand>("Frequency");
    }
}
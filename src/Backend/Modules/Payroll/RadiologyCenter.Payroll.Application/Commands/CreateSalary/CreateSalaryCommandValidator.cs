using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Domain.Enumerations;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.CreateSalary;

public class CreateSalaryCommandValidator : AbstractValidator<CreateSalaryCommand>
{
    public CreateSalaryCommandValidator()
    {
        RuleFor(x => x.StaffId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.BaseSalary).GreaterThanOrEqualTo(0).WithErrorCode(SharedCodes.Shared.CannotBeNegative);
        RuleFor(x => x.SalaryType).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).IsEnumerationMember<SalaryType, CreateSalaryCommand>("SalaryType");
        RuleFor(x => x.EffectiveDate).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired);
    }
}
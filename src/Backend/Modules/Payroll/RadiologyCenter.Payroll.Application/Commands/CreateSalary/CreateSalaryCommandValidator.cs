using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Domain.Enumerations;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.CreateSalary;

public class CreateSalaryCommandValidator : AbstractValidator<CreateSalaryCommand>
{
    public CreateSalaryCommandValidator()
    {
        RuleFor(x => x.StaffId).NotEmpty().WithErrorCode(ErrorCodes.StaffIdRequired);
        RuleFor(x => x.BaseSalary).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.AmountCannotBeNegative);
        RuleFor(x => x.SalaryType).NotEmpty().WithErrorCode(ErrorCodes.SalaryTypeRequired).IsEnumerationMember<SalaryType, CreateSalaryCommand>("SalaryType");
        RuleFor(x => x.EffectiveDate).NotEmpty().WithErrorCode(ErrorCodes.EffectiveDateRequired);
    }
}
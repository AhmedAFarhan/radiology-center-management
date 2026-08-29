using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Domain.Enumerations;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.UpdateSalary;

public class UpdateSalaryCommandValidator : AbstractValidator<UpdateSalaryCommand>
{
    public UpdateSalaryCommandValidator()
    {
        RuleFor(x => x.SalaryId).NotEmpty().WithErrorCode(ErrorCodes.SalaryIdRequired);
        RuleFor(x => x.BaseSalary).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.AmountCannotBeNegative);
        RuleFor(x => x.SalaryType).NotEmpty().WithErrorCode(ErrorCodes.SalaryTypeRequired).IsEnumerationMember<SalaryType, UpdateSalaryCommand>("SalaryType");
        RuleFor(x => x.EffectiveDate).NotEmpty().WithErrorCode(ErrorCodes.EffectiveDateRequired);
    }
}
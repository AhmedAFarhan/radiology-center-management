using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Application.Commands.UpdateSalary;

public class UpdateSalaryCommandValidator : AbstractValidator<UpdateSalaryCommand>
{
    public UpdateSalaryCommandValidator()
    {
        RuleFor(x => x.SalaryId).NotEmpty();
        RuleFor(x => x.BaseSalary).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SalaryType).NotEmpty().IsEnumerationMember<SalaryType, UpdateSalaryCommand>("SalaryType");
        RuleFor(x => x.EffectiveDate).NotEmpty();
    }
}
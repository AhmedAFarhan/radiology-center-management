using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Application.Commands.CreateSalary;

public class CreateSalaryCommandValidator : AbstractValidator<CreateSalaryCommand>
{
    public CreateSalaryCommandValidator()
    {
        RuleFor(x => x.StaffId).NotEmpty();
        RuleFor(x => x.BaseSalary).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SalaryType).NotEmpty().IsEnumerationMember<SalaryType, CreateSalaryCommand>("SalaryType");
        RuleFor(x => x.EffectiveDate).NotEmpty();
    }
}
using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Domain.Enumerations;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.UpdateSalary;

public class UpdateSalaryCommandValidator : AbstractValidator<UpdateSalaryCommand>
{
    public UpdateSalaryCommandValidator()
    {
        RuleFor(x => x.SalaryId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.BaseSalary).GreaterThanOrEqualTo(0).WithErrorCode(SharedCodes.Shared.CannotBeNegative);
        RuleFor(x => x.SalaryType).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).IsEnumerationMember<SalaryType, UpdateSalaryCommand>("SalaryType");
        RuleFor(x => x.EffectiveDate).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired);
    }
}
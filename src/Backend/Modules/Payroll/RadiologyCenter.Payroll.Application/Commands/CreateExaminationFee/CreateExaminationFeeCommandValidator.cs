using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Domain.Enumerations;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.CreateExaminationFee;

public class CreateExaminationFeeCommandValidator : AbstractValidator<CreateExaminationFeeCommand>
{
    public CreateExaminationFeeCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationTypeIdRequired);
        RuleFor(x => x.Role).NotEmpty().WithErrorCode(ErrorCodes.RoleRequired).IsEnumerationMember<ExamFeeRole, CreateExaminationFeeCommand>("Role");
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.AmountCannotBeNegative);
        RuleFor(x => x.Amount).LessThanOrEqualTo(100).WithErrorCode(ErrorCodes.PercentageAmountMax).When(x => x.IsPercentage);
    }
}
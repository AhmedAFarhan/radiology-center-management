using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Domain.Enumerations;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.UpdateExaminationFee;

public class UpdateExaminationFeeCommandValidator : AbstractValidator<UpdateExaminationFeeCommand>
{
    public UpdateExaminationFeeCommandValidator()
    {
        RuleFor(x => x.ExaminationFeeId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationFeeIdRequired);
        RuleFor(x => x.Role).NotEmpty().WithErrorCode(ErrorCodes.RoleRequired).IsEnumerationMember<ExamFeeRole, UpdateExaminationFeeCommand>("Role");
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.AmountCannotBeNegative);
        RuleFor(x => x.Amount).LessThanOrEqualTo(100).WithErrorCode(ErrorCodes.PercentageAmountMax).When(x => x.IsPercentage);
    }
}
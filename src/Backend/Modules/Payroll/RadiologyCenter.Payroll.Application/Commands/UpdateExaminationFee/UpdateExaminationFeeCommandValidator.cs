using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Application.Commands.UpdateExaminationFee;

public class UpdateExaminationFeeCommandValidator : AbstractValidator<UpdateExaminationFeeCommand>
{
    public UpdateExaminationFeeCommandValidator()
    {
        RuleFor(x => x.ExaminationFeeId).NotEmpty();
        RuleFor(x => x.Role).NotEmpty().IsEnumerationMember<ExamFeeRole, UpdateExaminationFeeCommand>("Role");
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Amount).LessThanOrEqualTo(100).When(x => x.IsPercentage);
    }
}
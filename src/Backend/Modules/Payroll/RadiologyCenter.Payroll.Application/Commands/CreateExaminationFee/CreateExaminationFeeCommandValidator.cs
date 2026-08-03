using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Application.Commands.CreateExaminationFee;

public class CreateExaminationFeeCommandValidator : AbstractValidator<CreateExaminationFeeCommand>
{
    public CreateExaminationFeeCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty();
        RuleFor(x => x.Role).NotEmpty().IsEnumerationMember<ExamFeeRole, CreateExaminationFeeCommand>("Role");
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Amount).LessThanOrEqualTo(100).When(x => x.IsPercentage);
    }
}
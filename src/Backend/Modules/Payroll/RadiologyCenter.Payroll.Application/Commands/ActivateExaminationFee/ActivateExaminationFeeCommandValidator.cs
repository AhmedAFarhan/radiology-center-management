using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Commands.ActivateExaminationFee;

public class ActivateExaminationFeeCommandValidator : AbstractValidator<ActivateExaminationFeeCommand>
{
    public ActivateExaminationFeeCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}
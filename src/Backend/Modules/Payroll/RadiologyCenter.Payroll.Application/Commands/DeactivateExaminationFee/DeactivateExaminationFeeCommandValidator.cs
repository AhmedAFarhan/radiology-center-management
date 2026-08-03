using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Commands.DeactivateExaminationFee;

public class DeactivateExaminationFeeCommandValidator : AbstractValidator<DeactivateExaminationFeeCommand>
{
    public DeactivateExaminationFeeCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}
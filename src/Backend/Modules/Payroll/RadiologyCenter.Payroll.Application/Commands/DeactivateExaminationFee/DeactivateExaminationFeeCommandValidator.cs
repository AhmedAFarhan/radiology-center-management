using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.DeactivateExaminationFee;

public class DeactivateExaminationFeeCommandValidator : AbstractValidator<DeactivateExaminationFeeCommand>
{
    public DeactivateExaminationFeeCommandValidator() => RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.ExaminationFeeIdRequired);
}
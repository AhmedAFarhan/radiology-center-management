using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.DeleteExaminationFee;

public class DeleteExaminationFeeCommandValidator : AbstractValidator<DeleteExaminationFeeCommand>
{
    public DeleteExaminationFeeCommandValidator() => RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.ExaminationFeeIdRequired);
}
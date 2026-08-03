using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Commands.DeleteExaminationFee;

public class DeleteExaminationFeeCommandValidator : AbstractValidator<DeleteExaminationFeeCommand>
{
    public DeleteExaminationFeeCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}
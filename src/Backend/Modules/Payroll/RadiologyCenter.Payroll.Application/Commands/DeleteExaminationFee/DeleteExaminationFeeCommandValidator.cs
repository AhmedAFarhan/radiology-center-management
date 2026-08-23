using FluentValidation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.DeleteExaminationFee;

public class DeleteExaminationFeeCommandValidator : AbstractValidator<DeleteExaminationFeeCommand>
{
    public DeleteExaminationFeeCommandValidator() => RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired);
}
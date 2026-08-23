using FluentValidation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.ActivateExaminationFee;

public class ActivateExaminationFeeCommandValidator : AbstractValidator<ActivateExaminationFeeCommand>
{
    public ActivateExaminationFeeCommandValidator() => RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired);
}
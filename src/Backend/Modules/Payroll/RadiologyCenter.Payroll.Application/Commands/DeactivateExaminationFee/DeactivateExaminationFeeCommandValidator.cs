using FluentValidation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.DeactivateExaminationFee;

public class DeactivateExaminationFeeCommandValidator : AbstractValidator<DeactivateExaminationFeeCommand>
{
    public DeactivateExaminationFeeCommandValidator() => RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired);
}
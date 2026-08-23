using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Domain.Enumerations;
using RadiologyCenter.Payroll.Application.Localization;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.CreateExaminationFee;

public class CreateExaminationFeeCommandValidator : AbstractValidator<CreateExaminationFeeCommand>
{
    public CreateExaminationFeeCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.Role).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).IsEnumerationMember<ExamFeeRole, CreateExaminationFeeCommand>("Role");
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0).WithErrorCode(SharedCodes.Shared.CannotBeNegative);
        RuleFor(x => x.Amount).LessThanOrEqualTo(100).WithErrorCode(ErrorCodes.PercentageAmountMax).When(x => x.IsPercentage);
    }
}
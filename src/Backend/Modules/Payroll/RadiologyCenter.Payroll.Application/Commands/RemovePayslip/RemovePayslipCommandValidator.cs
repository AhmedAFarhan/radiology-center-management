using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.RemovePayslip;

public class RemovePayslipCommandValidator : AbstractValidator<RemovePayslipCommand>
{
    public RemovePayslipCommandValidator()
    {
        RuleFor(x => x.PayRunId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.StaffId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}
using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.AddPayslip;

public class AddPayslipCommandValidator : AbstractValidator<AddPayslipCommand>
{
    public AddPayslipCommandValidator()
    {
        RuleFor(x => x.PayRunId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.StaffId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}
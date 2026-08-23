using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.ApprovePayRun;

public class ApprovePayRunCommandValidator : AbstractValidator<ApprovePayRunCommand>
{
    public ApprovePayRunCommandValidator()
    {
        RuleFor(x => x.PayRunId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}
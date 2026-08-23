using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.RejectPayRun;

public class RejectPayRunCommandValidator : AbstractValidator<RejectPayRunCommand>
{
    public RejectPayRunCommandValidator()
    {
        RuleFor(x => x.PayRunId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}
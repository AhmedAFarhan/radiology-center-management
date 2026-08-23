using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.RestartPayRun;

public class RestartPayRunCommandValidator : AbstractValidator<RestartPayRunCommand>
{
    public RestartPayRunCommandValidator()
    {
        RuleFor(x => x.PayRunId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}
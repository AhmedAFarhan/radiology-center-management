using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.ComputePayRun;

public class ComputePayRunCommandValidator : AbstractValidator<ComputePayRunCommand>
{
    public ComputePayRunCommandValidator()
    {
        RuleFor(x => x.PayRunId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}
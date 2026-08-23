using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.DeletePayRun;

public class DeletePayRunCommandValidator : AbstractValidator<DeletePayRunCommand>
{
    public DeletePayRunCommandValidator()
    {
        RuleFor(x => x.PayRunId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}
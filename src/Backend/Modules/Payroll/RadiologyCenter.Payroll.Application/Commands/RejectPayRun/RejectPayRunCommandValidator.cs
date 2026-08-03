using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Commands.RejectPayRun;

public class RejectPayRunCommandValidator : AbstractValidator<RejectPayRunCommand>
{
    public RejectPayRunCommandValidator()
    {
        RuleFor(x => x.PayRunId).NotEmpty();
    }
}
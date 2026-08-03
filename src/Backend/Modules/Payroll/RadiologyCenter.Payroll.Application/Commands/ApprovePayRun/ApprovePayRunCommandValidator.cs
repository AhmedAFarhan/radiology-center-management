using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Commands.ApprovePayRun;

public class ApprovePayRunCommandValidator : AbstractValidator<ApprovePayRunCommand>
{
    public ApprovePayRunCommandValidator()
    {
        RuleFor(x => x.PayRunId).NotEmpty();
    }
}
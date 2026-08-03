using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Commands.ComputePayRun;

public class ComputePayRunCommandValidator : AbstractValidator<ComputePayRunCommand>
{
    public ComputePayRunCommandValidator()
    {
        RuleFor(x => x.PayRunId).NotEmpty();
    }
}
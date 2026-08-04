using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Commands.RestartPayRun;

public class RestartPayRunCommandValidator : AbstractValidator<RestartPayRunCommand>
{
    public RestartPayRunCommandValidator()
    {
        RuleFor(x => x.PayRunId).NotEmpty();
    }
}
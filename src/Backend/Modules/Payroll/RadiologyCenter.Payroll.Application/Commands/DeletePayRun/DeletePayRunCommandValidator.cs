using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Commands.DeletePayRun;

public class DeletePayRunCommandValidator : AbstractValidator<DeletePayRunCommand>
{
    public DeletePayRunCommandValidator()
    {
        RuleFor(x => x.PayRunId).NotEmpty();
    }
}
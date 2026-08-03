using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Commands.CreatePayRun;

public class CreatePayRunCommandValidator : AbstractValidator<CreatePayRunCommand>
{
    public CreatePayRunCommandValidator()
    {
        RuleFor(x => x.RunFrom).NotEqual(default(DateTime));
        RuleFor(x => x.RunTo).NotEqual(default(DateTime));
        RuleFor(x => x.RunTo).GreaterThanOrEqualTo(x => x.RunFrom).When(x => x.RunFrom != default(DateTime));
    }
}
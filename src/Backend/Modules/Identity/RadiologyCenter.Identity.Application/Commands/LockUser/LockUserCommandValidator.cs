using FluentValidation;

namespace RadiologyCenter.Identity.Application.Commands.LockUser;

public class LockUserCommandValidator : AbstractValidator<LockUserCommand>
{
    public LockUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.LockoutEnd).GreaterThan(DateTimeOffset.UtcNow);
    }
}

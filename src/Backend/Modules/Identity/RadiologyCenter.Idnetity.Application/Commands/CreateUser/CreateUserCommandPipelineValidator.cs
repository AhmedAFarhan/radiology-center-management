using FluentValidation;
using RadiologyCenter.Idnetity.Application.Abstractions;

namespace RadiologyCenter.Idnetity.Application.Commands.CreateUser;

public class CreateUserCommandPipelineValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandPipelineValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.Email).MustAsync(async (email, ct) =>
            !await userRepository.ExistsByEmailAsync(email, ct))
            .WithMessage("Email is already registered.");
        RuleFor(x => x.UserName).MustAsync(async (userName, ct) =>
            !await userRepository.ExistsByUserNameAsync(userName, ct))
            .WithMessage("Username is already taken.");
    }
}

using FluentValidation;
using Microsoft.AspNetCore.Identity;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Idnetity.Application.Abstractions;
using RadiologyCenter.Idnetity.Domain.Entities;
using RadiologyCenter.Idnetity.Domain.Events;

namespace RadiologyCenter.Idnetity.Application.Commands.Users;

public record CreateUserCommand(string UserName, string Email, string FirstName, string LastName, string Password);

public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(100);
    }
}

public static class CreateUserHandler
{
    public static async Task<Result> HandleAsync(
        CreateUserCommand command,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher,
        CancellationToken ct)
    {
        var emailExists = await userRepository.ExistsByEmailAsync(command.Email, ct);
        if (emailExists)
            return Result.Failure(Error.Conflict($"Email '{command.Email}' is already registered."));

        var userNameExists = await userRepository.ExistsByUserNameAsync(command.UserName, ct);
        if (userNameExists)
            return Result.Failure(Error.Conflict($"Username '{command.UserName}' is already taken."));

        var user = User.Create(command.UserName, command.Email, command.FirstName, command.LastName);
        user.SetPasswordHash(passwordHasher.HashPassword(user, command.Password));

        await userRepository.AddAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

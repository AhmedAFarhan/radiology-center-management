using Microsoft.AspNetCore.Identity;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Idnetity.Application.Abstractions;

namespace RadiologyCenter.Idnetity.Application.Commands.CreateUser;

public static class CreateUserCommandHandler
{
    public static async Task<Result> HandleAsync(
        CreateUserCommand command,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher,
        CancellationToken ct)
    {
        var user = User.Create(command.UserName, command.Email, command.FirstName, command.LastName);
        user.SetPasswordHash(passwordHasher.HashPassword(user, command.Password));
        await userRepository.AddAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}

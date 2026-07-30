using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Idnetity.Application.Abstractions;

namespace RadiologyCenter.Idnetity.Application.Commands.Users;

public record RemoveRoleFromUserCommand(Guid UserId, Guid RoleId);

public class RemoveRoleFromUserValidator : AbstractValidator<RemoveRoleFromUserCommand>
{
    public RemoveRoleFromUserValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.RoleId).NotEmpty();
    }
}

public static class RemoveRoleFromUserHandler
{
    public static async Task<Result> HandleAsync(
        RemoveRoleFromUserCommand command,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, ct);
        if (user is null)
            return Result.Failure(Error.NotFound(nameof(User), command.UserId));

        var role = await roleRepository.GetByIdAsync(command.RoleId, ct);
        if (role is null)
            return Result.Failure(Error.NotFound(nameof(Role), command.RoleId));

        user.RemoveRole(role);
        await userRepository.UpdateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

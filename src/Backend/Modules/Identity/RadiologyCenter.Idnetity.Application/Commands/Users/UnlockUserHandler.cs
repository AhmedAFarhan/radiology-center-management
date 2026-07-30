using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Idnetity.Application.Abstractions;

namespace RadiologyCenter.Idnetity.Application.Commands.Users;

public record UnlockUserCommand(Guid UserId);

public class UnlockUserValidator : AbstractValidator<UnlockUserCommand>
{
    public UnlockUserValidator() => RuleFor(x => x.UserId).NotEmpty();
}

public static class UnlockUserHandler
{
    public static async Task<Result> HandleAsync(
        UnlockUserCommand command,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, ct);
        if (user is null)
            return Result.Failure(Error.NotFound(nameof(User), command.UserId));

        user.Unlock();
        await userRepository.UpdateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

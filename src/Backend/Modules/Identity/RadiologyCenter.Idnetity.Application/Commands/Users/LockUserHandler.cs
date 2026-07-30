using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Idnetity.Application.Abstractions;

namespace RadiologyCenter.Idnetity.Application.Commands.Users;

public record LockUserCommand(Guid UserId, DateTimeOffset LockoutEnd);

public class LockUserValidator : AbstractValidator<LockUserCommand>
{
    public LockUserValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.LockoutEnd).GreaterThan(DateTimeOffset.UtcNow);
    }
}

public static class LockUserHandler
{
    public static async Task<Result> HandleAsync(
        LockUserCommand command,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, ct);
        if (user is null)
            return Result.Failure(Error.NotFound(nameof(User), command.UserId));

        user.Lock(command.LockoutEnd);
        await userRepository.UpdateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

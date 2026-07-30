using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Idnetity.Application.Abstractions;
using RadiologyCenter.Idnetity.Domain.Entities;

namespace RadiologyCenter.Idnetity.Application.Commands.Roles;

public record CreateRoleCommand(string Name, string? Description, bool IsSystem = false);

public class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public static class CreateRoleHandler
{
    public static async Task<Result> HandleAsync(
        CreateRoleCommand command,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var nameExists = await roleRepository.ExistsByNameAsync(command.Name, ct);
        if (nameExists)
            return Result.Failure(Error.Conflict($"Role '{command.Name}' already exists."));

        var role = Role.Create(command.Name, command.Description, command.IsSystem);
        await roleRepository.AddAsync(role, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

namespace RadiologyCenter.Identity.Application.Commands.CreateUser;

public record CreateUserCommand(
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Password,
    IReadOnlyList<Guid> RoleIds) : ICommand;

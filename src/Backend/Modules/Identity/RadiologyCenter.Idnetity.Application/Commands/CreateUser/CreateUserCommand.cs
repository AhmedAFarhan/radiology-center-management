namespace RadiologyCenter.Idnetity.Application.Commands.CreateUser;

public record CreateUserCommand(string UserName, string Email, string FirstName, string LastName, string Password);

namespace RadiologyCenter.Idnetity.Application.Commands.UpdateUserProfile;

public record UpdateUserProfileCommand(Guid UserId, string FirstName, string LastName, string? PhoneNumber);

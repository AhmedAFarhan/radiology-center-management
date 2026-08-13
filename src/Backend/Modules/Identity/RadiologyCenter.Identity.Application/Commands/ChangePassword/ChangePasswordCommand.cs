namespace RadiologyCenter.Identity.Application.Commands.ChangePassword;

public record ChangePasswordCommand(string CurrentPassword, string NewPassword) : ICommand;
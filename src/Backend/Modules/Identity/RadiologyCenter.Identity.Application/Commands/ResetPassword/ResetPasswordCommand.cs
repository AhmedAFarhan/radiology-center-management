namespace RadiologyCenter.Identity.Application.Commands.ResetPassword;

public record ResetPasswordCommand(Guid UserId, string NewPassword) : ICommand;
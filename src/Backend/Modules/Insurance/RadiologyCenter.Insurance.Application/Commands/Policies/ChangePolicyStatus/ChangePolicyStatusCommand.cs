namespace RadiologyCenter.Insurance.Application.Commands.Policies.ChangePolicyStatus;

public enum PolicyAction
{
    Deactivate,
    Reactivate,
    Expire
}

public record ChangePolicyStatusCommand(Guid PolicyId, PolicyAction Action) : ICommand;
namespace RadiologyCenter.Identity.Application.Abstractions;

public interface IIdentityAccessProvisioner
{
    Task ProvisionAsync(Guid userId, string password, CancellationToken ct);
}
public sealed class NoIdentityAccessProvisioner : IIdentityAccessProvisioner
{
    public Task ProvisionAsync(Guid userId,string password,CancellationToken ct) => Task.CompletedTask;
}

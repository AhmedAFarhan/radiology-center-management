namespace RadiologyCenter.Identity.Infrastructure.Persistence.Seed;

public sealed class AdministratorSeedOptions
{
    public string AdminUserName { get; set; } = "admin";
    public string AdminPassword { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = "admin@radiologycenter.local";
    public string AdminPhone { get; set; } = "01000000000";
}

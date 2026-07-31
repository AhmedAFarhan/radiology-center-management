namespace RadiologyCenter.Identity.Application.Settings;

public class AccountLockoutOptions
{
    public int MaxFailedAccessAttempts { get; init; } = 5;
    public int LockoutDurationMinutes { get; init; } = 5;
}

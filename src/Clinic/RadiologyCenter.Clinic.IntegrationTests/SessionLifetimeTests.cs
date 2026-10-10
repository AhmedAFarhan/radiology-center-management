using RadiologyCenter.Clinic.Security;
using Xunit;

public sealed class SessionLifetimeTests
{
    [Fact]
    public async Task ClosingWaitsForAllOperationsAndRejectsNewOperations()
    {
        var guard = new ClinicSessionGuard();
        using var first = guard.EnterOperation();
        using var second = guard.EnterOperation();
        Assert.NotNull(first);
        Assert.NotNull(second);
        var drain = guard.StopAndDrainAsync();
        Assert.Null(guard.EnterOperation());
        Assert.False(drain.IsCompleted);
        first.Dispose();
        first.Dispose(); // A duplicate disposal must not release another operation.
        Assert.False(drain.IsCompleted);
        second.Dispose();
        await drain.WaitAsync(TimeSpan.FromSeconds(2));
        Assert.Null(guard.EnterOperation());
    }

    [Fact]
    public async Task ClosingAnIdleSessionCompletesWithoutWaiting()
    {
        var guard = new ClinicSessionGuard();
        await guard.StopAndDrainAsync().WaitAsync(TimeSpan.FromSeconds(2));
        Assert.Null(guard.EnterOperation());
        await guard.StopAndDrainAsync().WaitAsync(TimeSpan.FromSeconds(2));
    }
}

using Microsoft.EntityFrameworkCore;

namespace RadiologyCenter.IntegrationTests.Shared;

public static class DatabaseCleanup
{
    public static async Task ResetAsync(DbContext dbContext)
    {
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }
}

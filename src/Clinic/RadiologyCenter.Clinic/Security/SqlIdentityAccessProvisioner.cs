using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RadiologyCenter.Identity.Application.Abstractions;
using RadiologyCenter.Identity.Infrastructure.Persistence;

namespace RadiologyCenter.Clinic.Security;

public sealed class SqlIdentityAccessProvisioner(IdentityDbContext context) : IIdentityAccessProvisioner
{
    public async Task ProvisionAsync(Guid userId,string password,CancellationToken ct)
    {
        await using var command=context.Database.GetDbConnection().CreateCommand();
        command.Transaction=context.Database.CurrentTransaction?.GetDbTransaction()
            ?? throw new InvalidOperationException("Database identities must be provisioned with the account transaction.");
        command.CommandText="Security.ProvisionDesktopUser"; command.CommandType=System.Data.CommandType.StoredProcedure;
        var id=command.CreateParameter(); id.ParameterName="@UserId"; id.Value=userId; command.Parameters.Add(id);
        var secret=command.CreateParameter(); secret.ParameterName="@Password"; secret.Value=password; secret.Size=128; command.Parameters.Add(secret);
        await command.ExecuteNonQueryAsync(ct);
    }
}

using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.BuildingBlocks.Domain.Localization;

namespace RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;

public sealed class SqlTransactionLock : ITransactionLock
{
    public async Task AcquireAsync(IUnitOfWorkTransaction transaction, IEnumerable<string> resources, CancellationToken ct = default)
    {
        var sqlTransaction = transaction.DbTransaction ?? throw new InvalidOperationException("A database transaction is required.");
        foreach (var resource in resources.Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal))
        {
            ct.ThrowIfCancellationRequested();
            await using var command = sqlTransaction.Connection!.CreateCommand();
            command.Transaction = sqlTransaction;
            command.CommandText = "DECLARE @result int; EXEC @result=sys.sp_getapplock @Resource=@resource,@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=10000; SELECT @result;";
            var parameter = command.CreateParameter();
            parameter.ParameterName = "@resource";
            parameter.Value = "RadiologyCenter:" + resource;
            command.Parameters.Add(parameter);
            var result = Convert.ToInt32(await command.ExecuteScalarAsync(ct));
            if (result < 0)
                throw new ConcurrencyException(MessageCodes.Shared.ConcurrencyConflict, "The resource is being updated. Refresh and try again.");
        }
    }
}

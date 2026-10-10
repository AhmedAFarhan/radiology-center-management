using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Domain.Entities;

namespace RadiologyCenter.Clinic.Services;

/// <summary>Persists domain facts with the originating transaction. No background callbacks are required.</summary>
public sealed class SqlDomainEventJournal : IDomainEventDispatcher
{
    public async Task DispatchAsync(IAggregateRoot aggregate, DbContext context, CancellationToken ct = default)
    {
        var events = aggregate.DomainEvents.ToArray();
        if (events.Length == 0) return;
        var transaction = context.Database.CurrentTransaction
            ?? throw new InvalidOperationException("Domain events require the business transaction.");
        foreach (var e in events)
        {
            await using var command = context.Database.GetDbConnection().CreateCommand();
            command.Transaction = transaction.GetDbTransaction();
            command.CommandText = "INSERT INTO [System].[DesktopEvents](EventType,Payload) VALUES (@type,@payload)";
            var type = command.CreateParameter(); type.ParameterName = "@type";
            type.DbType = DbType.String; type.Size = 200; type.Value = e.GetType().Name; command.Parameters.Add(type);
            var payload = command.CreateParameter(); payload.ParameterName = "@payload";
            payload.DbType = DbType.String; payload.Value = JsonSerializer.Serialize(e, e.GetType()); command.Parameters.Add(payload);
            await command.ExecuteNonQueryAsync(ct);
        }
        aggregate.ClearDomainEvents();
    }
    public Task FlushAsync(CancellationToken ct = default) => Task.CompletedTask;
}

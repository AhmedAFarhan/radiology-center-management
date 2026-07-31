using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Infrastructure.Persistence;

namespace RadiologyCenter.Inventory.Infrastructure.Services;

public class OrderNumberGenerator : IOrderNumberGenerator
{
    private readonly InventoryDbContext _context;

    public OrderNumberGenerator(InventoryDbContext context) => _context = context;

    public async Task<string> GenerateNextAsync(CancellationToken ct = default)
    {
        var year = DateTime.UtcNow.Year;

        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(ct);

        await using var command = connection.CreateCommand();
        command.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();
        command.CommandText =
            """
            MERGE [Inventory].[OrderNumberSequences] WITH (HOLDLOCK) AS target
            USING (SELECT @year AS [Year]) AS source ON target.[Year] = source.[Year]
            WHEN MATCHED THEN UPDATE SET LastNumber = LastNumber + 1
            WHEN NOT MATCHED THEN INSERT ([Year], LastNumber) VALUES (@year, 1)
            OUTPUT inserted.LastNumber;
            """;
        command.Parameters.Add(new SqlParameter("@year", year));

        var nextNumber = Convert.ToInt32(await command.ExecuteScalarAsync(ct));
        return $"PO-{year}-{nextNumber:0000}";
    }
}

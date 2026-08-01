using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Application.Abstractions.Services;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;

namespace RadiologyCenter.BuildingBlocks.Infrastructure.Services;

public class NumberSequenceGenerator : INumberSequenceGenerator
{
    private readonly AppDbContext _context;

    public NumberSequenceGenerator(AppDbContext context) => _context = context;

    public async Task<string> GenerateNextAsync(
        string sequenceName,
        string prefix,
        int padding = 4,
        DbTransaction? transaction = null,
        CancellationToken ct = default)
    {
        var year = DateTime.UtcNow.Year;

        var connection = transaction?.Connection ?? _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(ct);

        await using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            """
            MERGE [System].[NumberSequences] WITH (HOLDLOCK) AS target
            USING (SELECT @name AS [Name], @year AS [Year]) AS source
            ON target.[Name] = source.[Name] AND target.[Year] = source.[Year]
            WHEN MATCHED THEN UPDATE SET LastNumber = LastNumber + 1
            WHEN NOT MATCHED THEN INSERT ([Name], [Year], LastNumber) VALUES (@name, @year, 1)
            OUTPUT inserted.LastNumber;
            """;
        command.Parameters.Add(new SqlParameter("@name", sequenceName));
        command.Parameters.Add(new SqlParameter("@year", year));

        var nextNumber = Convert.ToInt32(await command.ExecuteScalarAsync(ct));
        return $"{prefix}-{year}-{nextNumber.ToString($"D{padding}")}";
    }
}

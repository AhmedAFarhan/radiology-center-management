using System.Data.Common;

namespace RadiologyCenter.BuildingBlocks.Application.Abstractions.Services;

public interface INumberSequenceGenerator
{
    Task<string> GenerateNextAsync(
        string sequenceName,
        string prefix,
        int padding = 4,
        DbTransaction? transaction = null,
        CancellationToken ct = default);
}

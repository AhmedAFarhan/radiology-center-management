namespace RadiologyCenter.Inventory.Application.Abstractions;

public interface IOrderNumberGenerator
{
    Task<string> GenerateNextAsync(CancellationToken ct = default);
}

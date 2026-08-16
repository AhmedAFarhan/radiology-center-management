using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Entities;
using RadiologyCenter.Insurance.Domain.Enumerations;
using RadiologyCenter.Insurance.Domain.Errors;

namespace RadiologyCenter.Insurance.Domain.Entities;

public sealed class Settlement : Entity<Guid>
{
    public Guid ClaimId { get; private set; }
    public decimal Amount { get; private set; }
    public SettlementMethod Method { get; private set; }
    public DateTime SettledAt { get; private set; }
    public string? Reference { get; private set; }

    private Settlement()
    {
        Method = null!;
    }

    public static Settlement Create(
        Guid claimId,
        SettlementMethod method,
        decimal amount,
        string? reference = null)
    {
        Guard.AgainstEmpty(claimId, nameof(claimId));
        Guard.AgainstNull(method, nameof(method));
        Guard.Against(amount, a => a <= 0, DomainErrors.SettlementAmountPositive, "Settlement amount must be greater than zero.");

        return new Settlement
        {
            Id = Guid.NewGuid(),
            ClaimId = claimId,
            Amount = amount,
            Method = method,
            SettledAt = DateTime.UtcNow,
            Reference = reference?.Trim()
        };
    }
}
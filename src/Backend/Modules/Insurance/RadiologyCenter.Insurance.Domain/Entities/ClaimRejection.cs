using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Entities;
using RadiologyCenter.Insurance.Domain.Enumerations;

namespace RadiologyCenter.Insurance.Domain.Entities;

public sealed class ClaimRejection : Entity<Guid>
{
    public Guid ClaimId { get; private set; }
    public ClaimRejectionCode Code { get; private set; }
    public string Reason { get; private set; }
    public DateTime RejectedAt { get; private set; }

    private ClaimRejection()
    {
        Code = null!;
        Reason = string.Empty;
    }

    public static ClaimRejection Create(Guid claimId, ClaimRejectionCode code, string reason)
    {
        Guard.AgainstEmpty(claimId, nameof(claimId));
        Guard.AgainstNull(code, nameof(code));
        Guard.AgainstNullOrWhiteSpace(reason, nameof(reason));

        return new ClaimRejection
        {
            Id = Guid.NewGuid(),
            ClaimId = claimId,
            Code = code,
            Reason = reason.Trim(),
            RejectedAt = DateTime.UtcNow
        };
    }
}
using RadiologyCenter.BuildingBlocks.Domain.Auditing;
using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.Insurance.Domain.Enumerations;

namespace RadiologyCenter.Insurance.Domain.Entities;

public sealed class PreAuthorization : AuditableAggregateRoot<Guid>
{
    public Guid ExaminationId { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid PolicyId { get; private set; }
    public decimal EstimatedAmount { get; private set; }
    public PreAuthorizationStatus Status { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public DateTime? DecidedAt { get; private set; }
    public decimal? ApprovedAmount { get; private set; }
    public string? RejectionReason { get; private set; }

    private PreAuthorization()
    {
        Status = null!;
    }

    public static PreAuthorization Create(
        Guid examinationId,
        Guid patientId,
        Guid policyId,
        decimal estimatedAmount)
    {
        Guard.AgainstEmpty(examinationId, nameof(examinationId));
        Guard.AgainstEmpty(patientId, nameof(patientId));
        Guard.AgainstEmpty(policyId, nameof(policyId));
        Guard.Against(estimatedAmount, a => a < 0, "Estimated amount cannot be negative.");

        return new PreAuthorization
        {
            Id = Guid.NewGuid(),
            ExaminationId = examinationId,
            PatientId = patientId,
            PolicyId = policyId,
            EstimatedAmount = estimatedAmount,
            Status = PreAuthorizationStatus.Requested,
            RequestedAt = DateTime.UtcNow
        };
    }

    public void Approve(decimal approvedAmount)
    {
        EnsureRequested();

        Guard.Against(approvedAmount, a => a < 0, "Approved amount cannot be negative.");

        Status = PreAuthorizationStatus.Approved;
        ApprovedAmount = approvedAmount;
        DecidedAt = DateTime.UtcNow;
    }

    public void Deny(string reason)
    {
        EnsureRequested();
        Guard.AgainstNullOrWhiteSpace(reason, nameof(reason));

        Status = PreAuthorizationStatus.Denied;
        RejectionReason = reason.Trim();
        DecidedAt = DateTime.UtcNow;
    }

    public void Expire() => Status = PreAuthorizationStatus.Expired;

    private void EnsureRequested()
    {
        if (Status != PreAuthorizationStatus.Requested)
            throw new DomainException($"Pre-authorization '{Id}' is already {Status.Name}.");
    }
}
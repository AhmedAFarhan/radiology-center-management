using RadiologyCenter.BuildingBlocks.Domain.Auditing;
using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.Insurance.Domain.Enumerations;
using RadiologyCenter.Insurance.Domain.Events;

namespace RadiologyCenter.Insurance.Domain.Entities;

public sealed class Claim : AuditableAggregateRoot<Guid>
{
    private readonly List<ClaimRejection> _rejections = [];
    private readonly List<Settlement> _settlements = [];

    public Guid ExaminationId { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid PolicyId { get; private set; }
    public Guid PreAuthorizationId { get; private set; }
    public decimal BilledAmount { get; private set; }
    public decimal PayerShare { get; private set; }
    public decimal PatientShare { get; private set; }
    public ClaimStatus Status { get; private set; }
    public DateTime? SubmittedAt { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }

    public IReadOnlyCollection<ClaimRejection> Rejections => _rejections.AsReadOnly();
    public IReadOnlyCollection<Settlement> Settlements => _settlements.AsReadOnly();
    public decimal TotalSettled => _settlements.Sum(s => s.Amount);
    public decimal RemainingOwed => PayerShare - TotalSettled;

    private Claim()
    {
        Status = null!;
    }

    public static Claim Create(
        Guid examinationId,
        Guid patientId,
        Guid policyId,
        Guid preAuthorizationId,
        decimal billedAmount,
        decimal payerShare,
        decimal patientShare)
    {
        Guard.AgainstEmpty(examinationId, nameof(examinationId));
        Guard.AgainstEmpty(patientId, nameof(patientId));
        Guard.AgainstEmpty(policyId, nameof(policyId));
        Guard.AgainstEmpty(preAuthorizationId, nameof(preAuthorizationId));
        Guard.Against(billedAmount, a => a < 0, "Billed amount cannot be negative.");
        Guard.Against(payerShare, a => a < 0, "Payer share cannot be negative.");
        Guard.Against(patientShare, a => a < 0, "Patient share cannot be negative.");
        Guard.Against(payerShare + patientShare > billedAmount, _ => true,
            "Payer and patient shares cannot exceed the billed amount.");

        var claim = new Claim
        {
            Id = Guid.NewGuid(),
            ExaminationId = examinationId,
            PatientId = patientId,
            PolicyId = policyId,
            PreAuthorizationId = preAuthorizationId,
            BilledAmount = billedAmount,
            PayerShare = payerShare,
            PatientShare = patientShare,
            Status = ClaimStatus.Draft
        };

        claim.RaiseDomainEvent(new ClaimCreatedEvent(claim.Id, claim.ExaminationId, claim.PolicyId));
        return claim;
    }

    public void Submit()
    {
        if (Status != ClaimStatus.Draft && Status != ClaimStatus.Rejected)
            throw new DomainException($"Claim '{Id}' cannot be submitted from status {Status.Name}.");

        Status = ClaimStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;

        RaiseDomainEvent(new ClaimSubmittedEvent(Id, ExaminationId, PolicyId, PayerShare, PatientShare));
    }

    public void AdjudicateApproved(decimal approvedAmount)
    {
        EnsureSubmitted();
        Guard.Against(approvedAmount, a => a < 0, "Approved amount cannot be negative.");
        Guard.Against(approvedAmount > PayerShare, _ => true,
            "Approved amount cannot exceed the payer share.");

        Status = ClaimStatus.Approved;
        PayerShare = approvedAmount;
        ApprovedAt = DateTime.UtcNow;

        RaiseDomainEvent(new ClaimApprovedEvent(Id, ExaminationId, PolicyId, PayerShare));
    }

    public void AdjudicateRejected(ClaimRejectionCode code, string reason)
    {
        EnsureSubmitted();
        Guard.AgainstNull(code, nameof(code));
        Guard.AgainstNullOrWhiteSpace(reason, nameof(reason));

        Status = ClaimStatus.Rejected;
        _rejections.Add(ClaimRejection.Create(Id, code, reason));

        RaiseDomainEvent(new ClaimRejectedEvent(Id, ExaminationId, code.Value, reason));
    }

    public void Resubmit()
    {
        if (Status != ClaimStatus.Rejected)
            throw new DomainException($"Claim '{Id}' is not rejected and cannot be resubmitted.");

        Status = ClaimStatus.Draft;
        SubmittedAt = null;
        ApprovedAt = null;

        RaiseDomainEvent(new ClaimResubmittedEvent(Id, ExaminationId, _rejections.Count));
    }

    public void RecordSettlement(SettlementMethod method, decimal amount, string? reference = null)
    {
        if (Status != ClaimStatus.Approved)
            throw new DomainException($"Claim '{Id}' must be approved before settling payments.");

        Guard.AgainstNull(method, nameof(method));
        Guard.Against(amount, a => a <= 0, "Settlement amount must be greater than zero.");
        Guard.Against(amount > RemainingOwed, _ => true,
            $"Settlement of {amount} exceeds the remaining {RemainingOwed} owed for claim '{Id}'.");

        var settlement = Settlement.Create(Id, method, amount, reference);
        _settlements.Add(settlement);

        RaiseDomainEvent(new ClaimSettledEvent(Id, ExaminationId, amount, TotalSettled, RemainingOwed));

        if (RemainingOwed == 0)
        {
            Status = ClaimStatus.Paid;
            PaidAt = DateTime.UtcNow;
            RaiseDomainEvent(new ClaimPaidEvent(Id, ExaminationId, TotalSettled));
        }
    }

    private void EnsureSubmitted()
    {
        if (Status != ClaimStatus.Submitted)
            throw new DomainException($"Claim '{Id}' is not submitted and cannot be adjudicated.");
    }
}
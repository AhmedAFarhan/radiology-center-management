using RadiologyCenter.BuildingBlocks.Domain.Auditing;
using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.Insurance.Domain.Enumerations;
using RadiologyCenter.Insurance.Domain.Errors;

namespace RadiologyCenter.Insurance.Domain.Entities;

public sealed class PreAuthorization : AuditableAggregateRoot<Guid>
{
    private readonly List<PreAuthorizationDocument> _documents = [];

    public Guid ExaminationId { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid PolicyId { get; private set; }
    public decimal EstimatedAmount { get; private set; }
    public PreAuthorizationStatus Status { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public DateTime? DecidedAt { get; private set; }
    public decimal? ApprovedAmount { get; private set; }
    public string? RejectionReason { get; private set; }
    public bool IsGovernment { get; private set; }

    public IReadOnlyCollection<PreAuthorizationDocument> Documents => _documents.AsReadOnly();

    private PreAuthorization()
    {
        Status = null!;
    }

    public static PreAuthorization Create(
        Guid examinationId,
        Guid patientId,
        Guid policyId,
        decimal estimatedAmount,
        bool isGovernment = false)
    {
        Guard.AgainstEmpty(examinationId, nameof(examinationId));
        Guard.AgainstEmpty(patientId, nameof(patientId));
        Guard.AgainstEmpty(policyId, nameof(policyId));
        Guard.Against(estimatedAmount, a => a < 0, DomainErrors.EstimatedAmountNegative, "Estimated amount cannot be negative.");

        return new PreAuthorization
        {
            Id = Guid.NewGuid(),
            ExaminationId = examinationId,
            PatientId = patientId,
            PolicyId = policyId,
            EstimatedAmount = estimatedAmount,
            Status = PreAuthorizationStatus.Requested,
            RequestedAt = DateTime.UtcNow,
            IsGovernment = isGovernment
        };
    }

    public PreAuthorizationDocument AddDocument(
        DocumentType type,
        string fileName,
        string contentType,
        string storedPath,
        long sizeInBytes)
    {
        if (Status != PreAuthorizationStatus.Requested)
            throw new BusinessRuleViolationException(nameof(AddDocument), DomainErrors.DocumentsRequestedOnly, "Documents can only be attached while the pre-authorization is requested.");

        var document = PreAuthorizationDocument.Create(Id, type, fileName, contentType, storedPath, sizeInBytes);
        _documents.Add(document);
        return document;
    }

    public void Approve(decimal approvedAmount)
    {
        EnsureRequested();

        Guard.Against(approvedAmount, a => a < 0, DomainErrors.ApprovedAmountNegative, "Approved amount cannot be negative.");

        if (IsGovernment && _documents.Count == 0)
            throw new DomainException(DomainErrors.GovernmentDocRequired, "A government pre-authorization cannot be approved without the official approval document.");

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
            throw new BusinessRuleViolationException(nameof(EnsureRequested), DomainErrors.PreAuthorizationAlreadyDecided, $"Pre-authorization is already {Status.Name}.");
    }
}
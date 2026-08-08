using RadiologyCenter.BuildingBlocks.Domain.Auditing;
using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.Insurance.Domain.Enumerations;

namespace RadiologyCenter.Insurance.Domain.Entities;

public sealed class InsurancePolicy : AuditableAggregateRoot<Guid>
{
    private readonly List<PolicyDocument> _documents = [];

    public Guid CompanyId { get; private set; }
    public Guid PatientId { get; private set; }
    public string PolicyNumber { get; private set; }
    public decimal CoveragePercent { get; private set; }
    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }
    public PolicyStatus Status { get; private set; }

    public IReadOnlyCollection<PolicyDocument> Documents => _documents.AsReadOnly();

    private InsurancePolicy()
    {
        PolicyNumber = string.Empty;
        Status = null!;
    }

    public PolicyDocument AddDocument(
        DocumentType type,
        string fileName,
        string contentType,
        string storedPath,
        long sizeInBytes)
    {
        var document = PolicyDocument.Create(Id, type, fileName, contentType, storedPath, sizeInBytes);
        _documents.Add(document);
        return document;
    }

    public static InsurancePolicy Create(
        Guid companyId,
        Guid patientId,
        string policyNumber,
        decimal coveragePercent,
        DateTime effectiveFrom,
        DateTime? effectiveTo = null)
    {
        Guard.AgainstEmpty(companyId, nameof(companyId));
        Guard.AgainstEmpty(patientId, nameof(patientId));
        Guard.AgainstNullOrWhiteSpace(policyNumber, nameof(policyNumber));
        Guard.Against(coveragePercent, p => p < 0 || p > 100, "Coverage percent must be between 0 and 100.");

        return new InsurancePolicy
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            PatientId = patientId,
            PolicyNumber = policyNumber.Trim(),
            CoveragePercent = coveragePercent,
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo,
            Status = PolicyStatus.Active
        };
    }

    public void UpdateCoverage(decimal coveragePercent, DateTime? effectiveTo = null)
    {
        if (Status == PolicyStatus.Expired)
            throw new DomainException("Cannot update an expired policy.");

        Guard.Against(coveragePercent, p => p < 0 || p > 100, "Coverage percent must be between 0 and 100.");

        CoveragePercent = coveragePercent;
        if (effectiveTo.HasValue)
            EffectiveTo = effectiveTo;
    }

    public void Deactivate() => Status = PolicyStatus.Inactive;

    public void Reactivate()
    {
        if (EffectiveTo.HasValue && EffectiveTo.Value <= DateTime.UtcNow)
            throw new DomainException("Cannot reactivate an expired policy.");

        Status = PolicyStatus.Active;
    }

    public void MarkExpired() => Status = PolicyStatus.Expired;

    public bool IsActive => Status == PolicyStatus.Active && (!EffectiveTo.HasValue || EffectiveTo.Value >= DateTime.UtcNow);
}
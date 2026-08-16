using RadiologyCenter.BuildingBlocks.Domain.Auditing;
using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.Reports.Domain.Enumerations;
using RadiologyCenter.Reports.Domain.Errors;
using RadiologyCenter.Reports.Domain.Events;

namespace RadiologyCenter.Reports.Domain.Entities;

public sealed class RadiologyReport : AuditableAggregateRoot<Guid>
{
    private readonly List<ReportVersion> _versions = [];

    public Guid ExaminationId { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid RadiologistId { get; private set; }
    public ReportStatus Status { get; private set; }
    public int CurrentVersionNumber { get; private set; }
    public DateTime? FinalizedAt { get; private set; }
    public string? CancelReason { get; private set; }

    public IReadOnlyCollection<ReportVersion> Versions => _versions.AsReadOnly();

    private RadiologyReport()
    {
        Status = null!;
    }

    public static RadiologyReport Create(
        Guid examinationId,
        Guid patientId,
        Guid radiologistId)
    {
        Guard.AgainstEmpty(examinationId, nameof(examinationId));
        Guard.AgainstEmpty(patientId, nameof(patientId));
        Guard.AgainstEmpty(radiologistId, nameof(radiologistId));

        var report = new RadiologyReport
        {
            Id = Guid.NewGuid(),
            ExaminationId = examinationId,
            PatientId = patientId,
            RadiologistId = radiologistId,
            Status = ReportStatus.Draft,
            CurrentVersionNumber = 1
        };

        report._versions.Add(ReportVersion.Create(report.Id, 1));

        report.RaiseDomainEvent(new ReportDraftedEvent(report.Id, report.ExaminationId));
        return report;
    }

    public ReportVersion CurrentVersion => _versions
        .OrderBy(v => v.VersionNumber)
        .Last();

    public ReportSection? GetSection(ReportSectionType sectionType)
    {
        Guard.AgainstNull(sectionType, nameof(sectionType));
        return CurrentVersion.Sections.FirstOrDefault(s => s.SectionType == sectionType);
    }

    public void UpsertSection(
        ReportSectionType sectionType,
        string title,
        string body,
        int position = 0,
        bool isLocked = false)
    {
        EnsureEditable();
        Guard.AgainstNull(sectionType, nameof(sectionType));
        Guard.AgainstNullOrWhiteSpace(title, nameof(title));

        var section = GetSection(sectionType);
        if (section is null)
        {
            CurrentVersion.AddSection(sectionType, title, body.Trim(), position, isLocked);
            return;
        }

        if (section.IsLocked)
            throw new BusinessRuleViolationException(nameof(UpsertSection), DomainErrors.SectionLocked, $"Section '{sectionType.Name}' is locked and cannot be edited.");

        section.Update(body.Trim(), isLocked);
    }

    public ReportFinding AddFinding(string region, string description, FindingSeverity severity, int position = 0)
    {
        EnsureEditable();
        Guard.AgainstNullOrWhiteSpace(region, nameof(region));
        Guard.AgainstNullOrWhiteSpace(description, nameof(description));
        Guard.AgainstNull(severity, nameof(severity));

        return CurrentVersion.AddFinding(region, description.Trim(), severity, position);
    }

    public void UpdateFinding(Guid findingId, string description, FindingSeverity severity)
    {
        EnsureEditable();
        Guard.AgainstEmpty(findingId, nameof(findingId));
        Guard.AgainstNullOrWhiteSpace(description, nameof(description));
        Guard.AgainstNull(severity, nameof(severity));

        CurrentVersion.UpdateFinding(findingId, description.Trim(), severity);
    }

    public void RemoveFinding(Guid findingId)
    {
        EnsureEditable();
        Guard.AgainstEmpty(findingId, nameof(findingId));

        CurrentVersion.RemoveFinding(findingId);
    }

    public void FinalizeReport()
    {
        EnsureStatus(ReportStatus.Draft);

        EnsureRequiredContent();

        Status = ReportStatus.Finalized;
        FinalizedAt = DateTime.UtcNow;

        RaiseDomainEvent(new ReportFinalizedEvent(Id, CurrentVersionNumber));
    }

    public void Amend(string reason)
    {
        EnsureStatus(ReportStatus.Finalized);
        Guard.AgainstNullOrWhiteSpace(reason, nameof(reason));

        var source = CurrentVersion;
        var nextNumber = _versions.Max(v => v.VersionNumber) + 1;

        var amendment = ReportVersion.Create(Id, nextNumber, reason);
        foreach (var section in source.Sections)
        {
            amendment.AddSection(section.SectionType, section.Title, section.Body, section.Position, section.IsLocked);
        }
        foreach (var finding in source.Findings)
        {
            amendment.AddFinding(finding.Region, finding.Description, finding.Severity, finding.Position);
        }
        _versions.Add(amendment);

        CurrentVersionNumber = nextNumber;
        Status = ReportStatus.Draft;
        FinalizedAt = null;

        RaiseDomainEvent(new ReportAmendedEvent(Id, nextNumber, reason));
    }

    public void Cancel(string? reason = null)
    {
        EnsureStatus(ReportStatus.Draft, ReportStatus.Finalized);

        Status = ReportStatus.Cancelled;
        CancelReason = reason?.Trim();

        RaiseDomainEvent(new ReportCanceledEvent(Id));
    }

    private void EnsureRequiredContent()
    {
        var findings = GetSection(ReportSectionType.Findings);
        var impression = GetSection(ReportSectionType.Impression);

        if (findings is null || string.IsNullOrWhiteSpace(findings.Body))
            throw new DomainException(DomainErrors.FindingsRequired, "Report cannot be finalized without findings.");
        if (impression is null || string.IsNullOrWhiteSpace(impression.Body))
            throw new DomainException(DomainErrors.ImpressionRequired, "Report cannot be finalized without an impression.");
    }

    private void EnsureEditable()
    {
        if (Status != ReportStatus.Draft)
            throw new BusinessRuleViolationException(nameof(EnsureEditable), DomainErrors.ReportContentDraftOnly, "Report content can only be edited while it is a draft.");
    }

    private void EnsureStatus(params ReportStatus[] allowed)
    {
        if (!allowed.Contains(Status))
            throw new BusinessRuleViolationException(nameof(EnsureStatus), DomainErrors.InvalidStatusTransition, $"Report '{Id}' cannot transition from status '{Status}'.");
    }
}
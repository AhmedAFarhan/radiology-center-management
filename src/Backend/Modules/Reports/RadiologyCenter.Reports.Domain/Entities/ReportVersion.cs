using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Entities;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Domain.Entities;

public sealed class ReportVersion : Entity<Guid>
{
    private readonly List<ReportSection> _sections = [];
    private readonly List<ReportFinding> _findings = [];

    public Guid ReportId { get; private set; }
    public int VersionNumber { get; private set; }
    public string? AmendmentReason { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<ReportSection> Sections => _sections.AsReadOnly();
    public IReadOnlyCollection<ReportFinding> Findings => _findings.AsReadOnly();

    private ReportVersion() { }

    public static ReportVersion Create(Guid reportId, int versionNumber, string? amendmentReason = null)
    {
        Guard.AgainstEmpty(reportId, nameof(reportId));
        Guard.AgainstNegativeOrZero(versionNumber, nameof(versionNumber));

        return new ReportVersion
        {
            Id = Guid.NewGuid(),
            ReportId = reportId,
            VersionNumber = versionNumber,
            AmendmentReason = amendmentReason?.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public ReportSection AddSection(
        ReportSectionType sectionType,
        string title,
        string body,
        int position = 0,
        bool isLocked = false)
    {
        Guard.AgainstNull(sectionType, nameof(sectionType));
        Guard.AgainstNullOrWhiteSpace(title, nameof(title));
        var isDuplicate = _sections.Any(s => s.SectionType == sectionType);
        Guard.Against(isDuplicate, duplicate => duplicate, $"Section '{sectionType.Name}' already exists on version '{Id}'.");

        var section = ReportSection.Create(Id, sectionType, title, body, position, isLocked);
        _sections.Add(section);
        return section;
    }

    public ReportFinding AddFinding(string region, string description, FindingSeverity severity, int position = 0)
    {
        Guard.AgainstNullOrWhiteSpace(region, nameof(region));
        Guard.AgainstNullOrWhiteSpace(description, nameof(description));
        Guard.AgainstNull(severity, nameof(severity));

        var finding = ReportFinding.Create(Id, region, description, severity, position);
        _findings.Add(finding);
        return finding;
    }

    public void UpdateFinding(Guid findingId, string description, FindingSeverity severity)
    {
        Guard.AgainstEmpty(findingId, nameof(findingId));
        Guard.AgainstNullOrWhiteSpace(description, nameof(description));
        Guard.AgainstNull(severity, nameof(severity));

        var finding = GetFinding(findingId);
        finding.Update(description.Trim(), severity);
    }

    public void RemoveFinding(Guid findingId)
    {
        Guard.AgainstEmpty(findingId, nameof(findingId));

        var finding = GetFinding(findingId);
        _findings.Remove(finding);
    }

    private ReportFinding GetFinding(Guid findingId)
    {
        return _findings.FirstOrDefault(f => f.Id == findingId)
            ?? throw new DomainException($"Finding '{findingId}' is not on version '{Id}'.");
    }
}
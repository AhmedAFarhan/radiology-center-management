using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Entities;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Domain.Entities;

public sealed class ReportFinding : Entity<Guid>
{
    public Guid ReportVersionId { get; private set; }
    public string Region { get; private set; }
    public string Description { get; private set; }
    public FindingSeverity Severity { get; private set; }
    public int Position { get; private set; }

    private ReportFinding()
    {
        Region = null!;
        Description = null!;
        Severity = null!;
    }

    public static ReportFinding Create(
        Guid reportVersionId,
        string region,
        string description,
        FindingSeverity severity,
        int position = 0)
    {
        Guard.AgainstEmpty(reportVersionId, nameof(reportVersionId));
        Guard.AgainstNullOrWhiteSpace(region, nameof(region));
        Guard.AgainstNullOrWhiteSpace(description, nameof(description));
        Guard.AgainstNull(severity, nameof(severity));

        return new ReportFinding
        {
            Id = Guid.NewGuid(),
            ReportVersionId = reportVersionId,
            Region = region.Trim(),
            Description = description.Trim(),
            Severity = severity,
            Position = position
        };
    }

    public void Update(string description, FindingSeverity severity)
    {
        Description = description?.Trim() ?? string.Empty;
        Severity = severity;
    }
}
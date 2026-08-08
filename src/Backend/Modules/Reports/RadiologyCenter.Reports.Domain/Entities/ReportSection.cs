using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Entities;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Domain.Entities;

public sealed class ReportSection : Entity<Guid>
{
    public Guid ReportVersionId { get; private set; }
    public ReportSectionType SectionType { get; private set; }
    public string Title { get; private set; }
    public string Body { get; private set; }
    public int Position { get; private set; }
    public bool IsLocked { get; private set; }

    private ReportSection()
    {
        SectionType = null!;
        Title = null!;
        Body = null!;
    }

    public static ReportSection Create(
        Guid reportVersionId,
        ReportSectionType sectionType,
        string title,
        string body,
        int position = 0,
        bool isLocked = false)
    {
        Guard.AgainstEmpty(reportVersionId, nameof(reportVersionId));
        Guard.AgainstNull(sectionType, nameof(sectionType));
        Guard.AgainstNullOrWhiteSpace(title, nameof(title));

        return new ReportSection
        {
            Id = Guid.NewGuid(),
            ReportVersionId = reportVersionId,
            SectionType = sectionType,
            Title = title.Trim(),
            Body = body?.Trim() ?? string.Empty,
            Position = position,
            IsLocked = isLocked
        };
    }

    public void Update(string body, bool isLocked = false)
    {
        Body = body?.Trim() ?? string.Empty;
        IsLocked = isLocked;
    }
}
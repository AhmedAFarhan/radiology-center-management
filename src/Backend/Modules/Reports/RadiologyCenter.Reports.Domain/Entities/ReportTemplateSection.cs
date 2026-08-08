using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Entities;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Domain.Entities;

public sealed class ReportTemplateSection : Entity<Guid>
{
    public Guid ReportTemplateId { get; private set; }
    public ReportSectionType SectionType { get; private set; }
    public string Title { get; private set; }
    public string Body { get; private set; }
    public int Position { get; private set; }
    public bool IsLocked { get; private set; }

    private ReportTemplateSection()
    {
        SectionType = null!;
        Title = null!;
        Body = null!;
    }

    public static ReportTemplateSection Create(
        Guid reportTemplateId,
        ReportSectionType sectionType,
        string title,
        string body,
        int position = 0,
        bool isLocked = true)
    {
        Guard.AgainstEmpty(reportTemplateId, nameof(reportTemplateId));
        Guard.AgainstNull(sectionType, nameof(sectionType));
        Guard.AgainstNullOrWhiteSpace(title, nameof(title));

        return new ReportTemplateSection
        {
            Id = Guid.NewGuid(),
            ReportTemplateId = reportTemplateId,
            SectionType = sectionType,
            Title = title.Trim(),
            Body = body?.Trim() ?? string.Empty,
            Position = position,
            IsLocked = isLocked
        };
    }
}
using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.Catalog.Domain.Enumerations;
using RadiologyCenter.Reports.Domain.Enumerations;
using RadiologyCenter.Reports.Domain.Errors;

namespace RadiologyCenter.Reports.Domain.Entities;

public sealed class ReportTemplate : SoftDeletableAggregateRoot<Guid>
{
    private readonly List<ReportTemplateSection> _sections = [];

    public string Name { get; private set; }
    public Modality Modality { get; private set; }
    public string? BodyPart { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsSystem { get; private set; }
    public int UseCount { get; private set; }

    public IReadOnlyCollection<ReportTemplateSection> Sections => _sections.AsReadOnly();

    private ReportTemplate()
    {
        Name = null!;
        Modality = null!;
    }

    public static ReportTemplate Create(
        string name,
        Modality modality,
        string? bodyPart = null,
        string? description = null,
        bool isSystem = false)
    {
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Guard.AgainstNull(modality, nameof(modality));

        var template = new ReportTemplate
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Modality = modality,
            BodyPart = bodyPart?.Trim(),
            Description = description?.Trim(),
            IsActive = true,
            IsSystem = isSystem,
            UseCount = 0
        };

        return template;
    }

    public void Update(
        string name,
        Modality modality,
        string? bodyPart = null,
        string? description = null)
    {
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Guard.AgainstNull(modality, nameof(modality));

        Name = name.Trim();
        Modality = modality;
        BodyPart = bodyPart?.Trim();
        Description = description?.Trim();
    }

    public ReportTemplateSection AddSection(
        ReportSectionType sectionType,
        string title,
        string body,
        int position = 0,
        bool isLocked = true)
    {
        Guard.AgainstNull(sectionType, nameof(sectionType));
        Guard.AgainstNullOrWhiteSpace(title, nameof(title));
        Guard.Against(_sections.Any(s => s.SectionType == sectionType),
            duplicate => duplicate, DomainErrors.DuplicateTemplateSection, $"Section '{sectionType.Name}' already exists on template '{Name}'.");

        var section = ReportTemplateSection.Create(Id, sectionType, title, body, position, isLocked);
        _sections.Add(section);
        return section;
    }

    public void RemoveSection(Guid sectionId)
    {
        var section = _sections.FirstOrDefault(s => s.Id == sectionId)
            ?? throw new DomainException(DomainErrors.SectionNotOnTemplate, $"Section '{sectionId}' is not on template '{Name}'.");

        if (IsSystem)
            throw new BusinessRuleViolationException(nameof(RemoveSection), DomainErrors.SystemTemplateReadOnly, "System templates cannot be modified.");

        _sections.Remove(section);
    }

    public bool ContainsSection(ReportSectionType sectionType) =>
        _sections.Any(s => s.SectionType == sectionType);

    public void RegisterUse()
    {
        UseCount += 1;
    }

    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
    }
}
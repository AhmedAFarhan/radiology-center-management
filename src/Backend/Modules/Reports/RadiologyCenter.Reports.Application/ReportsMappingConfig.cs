using Mapster;
using RadiologyCenter.Reports.Application.DTOs;
using RadiologyCenter.Reports.Domain.Entities;

namespace RadiologyCenter.Reports.Application;

public static class ReportsMappingConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<ReportSection, ReportSectionDto>.NewConfig()
            .Map(d => d.SectionType, s => s.SectionType.Name);

        TypeAdapterConfig<ReportFinding, ReportFindingDto>.NewConfig()
            .Map(d => d.Severity, s => s.Severity.Name);

        TypeAdapterConfig<ReportTemplateSection, ReportTemplateSectionDto>.NewConfig()
            .Map(d => d.SectionType, s => s.SectionType.Name);

        TypeAdapterConfig<ReportTemplate, ReportTemplateDto>.NewConfig()
            .Map(d => d.Modality, s => s.Modality.Name)
            .Map(d => d.Sections, s => s.Sections);
    }
}
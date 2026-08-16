using Mapster;
using RadiologyCenter.BuildingBlocks.Application.Localization;
using RadiologyCenter.Reports.Domain.Entities;

namespace RadiologyCenter.Reports.Application.DTOs;

internal static class ReportTemplateMapper
{
    public static ReportTemplateDto ToDto(this ReportTemplate template) =>
        new(
            template.Id,
            template.Name,
            template.Modality.LocalizedName(),
            template.BodyPart,
            template.Description,
            template.IsActive,
            template.IsSystem,
            template.UseCount,
            template.Sections
                .OrderBy(s => s.Position)
                .Select(s => s.Adapt<ReportTemplateSectionDto>())
                .ToList());
}
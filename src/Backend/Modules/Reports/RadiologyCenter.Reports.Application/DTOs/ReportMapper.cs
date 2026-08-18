using Mapster;
using RadiologyCenter.BuildingBlocks.Application.Localization;
using RadiologyCenter.Reports.Domain.Entities;

namespace RadiologyCenter.Reports.Application.DTOs;

internal static class ReportMapper
{
    public static ReportDto ToDto(this RadiologyReport report) =>
        new(
            report.Id,
            report.ExaminationId,
            report.PatientId,
            report.RadiologistId,
            report.Status.LocalizedName(),
            report.Status.Name,
            report.CurrentVersionNumber,
            report.FinalizedAt,
            report.CancelReason,
            report.CurrentVersion!.Adapt<ReportVersionDto>());

    public static ReportListItemDto ToListItemDto(this RadiologyReport report) =>
        new(
            report.Id,
            report.ExaminationId,
            report.PatientId,
            report.RadiologistId,
            report.Status.LocalizedName(),
            report.Status.Name,
            report.CurrentVersionNumber,
            report.FinalizedAt,
            report.CancelReason);

    public static ReportVersionDto ToDto(this ReportVersion version) =>
        new(
            version.Id,
            version.VersionNumber,
            version.AmendmentReason,
            version.CreatedAt,
            version.Sections.Select(s => s.Adapt<ReportSectionDto>()).ToList(),
            version.Findings.Select(f => f.Adapt<ReportFindingDto>()).ToList());
}
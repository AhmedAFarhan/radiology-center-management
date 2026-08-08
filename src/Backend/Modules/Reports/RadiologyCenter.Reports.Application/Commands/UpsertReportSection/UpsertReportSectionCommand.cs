using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Application.Commands.UpsertReportSection;

public record UpsertReportSectionCommand(
    Guid ReportId,
    string SectionType,
    string Title,
    string Body,
    int Position = 0,
    bool IsLocked = false) : ICommand;
namespace RadiologyCenter.Examinations.Application.Queries.GetExaminationsForCalendar;

public record GetExaminationsForCalendarQuery(
    DateTime StartDate,
    DateTime EndDate,
    Guid? EquipmentId = null,
    Guid? RadiologistId = null,
    string? Modality = null) : IQuery;

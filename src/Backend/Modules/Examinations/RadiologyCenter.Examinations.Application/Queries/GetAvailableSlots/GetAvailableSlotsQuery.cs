namespace RadiologyCenter.Examinations.Application.Queries.GetAvailableSlots;

public record GetAvailableSlotsQuery(
    DateTime Date,
    Guid EquipmentId,
    int IntervalMinutes = 30) : IQuery;

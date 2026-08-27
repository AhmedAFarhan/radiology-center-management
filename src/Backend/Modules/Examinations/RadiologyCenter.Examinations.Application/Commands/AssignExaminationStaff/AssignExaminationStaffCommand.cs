namespace RadiologyCenter.Examinations.Application.Commands.AssignExaminationStaff;

public record AssignExaminationStaffCommand(
    Guid ExaminationId,
    Guid RadiologistId,
    Guid TechnicianId,
    Guid EquipmentId) : ICommand;

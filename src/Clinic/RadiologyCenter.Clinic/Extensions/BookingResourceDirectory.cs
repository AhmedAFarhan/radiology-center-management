using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Scheduling;
using RadiologyCenter.Examinations.Domain.Entities;
using RadiologyCenter.Patients.Infrastructure.Persistence;
using RadiologyCenter.ResourceManagement.Infrastructure.Persistence;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;
using RadiologyCenter.Catalog.Infrastructure.Persistence;

namespace RadiologyCenter.Clinic.Extensions;

public sealed class BookingResourceDirectory(PatientsDbContext patients, ResourceManagementDbContext resources, CatalogDbContext catalog) : IBookingResourceDirectory
{
    public Task<bool> IsOperationalEquipmentAsync(Guid equipmentId, CancellationToken ct) =>
        resources.Equipment.AnyAsync(e => e.Id == equipmentId && e.IsActive && e.Status == EquipmentStatus.Operational, ct);
    public async Task<Result> ValidateAsync(Examination examination, ExaminationTypeInfo type, DateTime? startUtc, DateTime? endUtc, CancellationToken ct)
    {
        if (!await patients.Patients.AnyAsync(p => p.Id == examination.PatientId && p.IsActive, ct))
            return Result.Failure(Error.Validation("Examinations.InvalidPatient", "Select an existing active patient."));
        if (!await catalog.ExaminationTypes.AnyAsync(t => t.Id == type.Id && t.IsActive, ct))
            return Result.Failure(Error.Validation("Examinations.InvalidType", "Select an active examination type."));
        if (examination.EquipmentId is { } equipmentId)
        {
            // Smart-enum Name is evaluated after materialization for EF compatibility.
            var equipment = await resources.Equipment.SingleOrDefaultAsync(e => e.Id == equipmentId, ct);
            if (equipment is null || !equipment.IsActive || equipment.Status != EquipmentStatus.Operational
                || !string.Equals(equipment.Modality.Name, type.Modality, StringComparison.OrdinalIgnoreCase))
                return Result.Failure(Error.Validation("Examinations.InvalidEquipment", "Select operational equipment matching the examination modality."));
        }
        if (examination.ReferralDoctorId is { } referral && !await resources.ReferralDoctors.AnyAsync(r => r.Id == referral && r.IsActive, ct))
            return Result.Failure(Error.Validation("Examinations.InvalidReferral", "Select an active referral doctor."));
        foreach (var assignment in new[] { (examination.RadiologistId, StaffPosition.Radiologist), (examination.TechnicianId, StaffPosition.Technician) })
        {
            if (assignment.Item1 is not { } id) continue;
            var staff = await resources.Staff.SingleOrDefaultAsync(s => s.Id == id, ct);
            if (staff is null || !staff.IsActive || staff.Position != assignment.Item2)
                return Result.Failure(Error.Validation("Examinations.InvalidStaff", "Select active staff with the appropriate clinical role."));
            if (startUtc.HasValue && endUtc.HasValue)
            {
                var localStart = ClinicClock.ToLocal(startUtc.Value);
                var localEnd = ClinicClock.ToLocal(endUtc.Value);
                if (await resources.Leaves.AnyAsync(l => l.StaffId == id && l.StartDate.Date <= localEnd.Date && l.EndDate.Date >= localStart.Date, ct))
                    return Result.Failure(Error.Conflict("Examinations.StaffOnLeave", "An assigned staff member is on leave."));
                var shifts = await resources.WorkShifts.Where(s => s.StaffId == id && s.Date.Date == localStart.Date).ToListAsync(ct);
                if (shifts.Count > 0 && !shifts.Any(s => s.StartTime <= localStart.TimeOfDay && s.EndTime >= localEnd.TimeOfDay
                    && (!s.EquipmentId.HasValue || s.EquipmentId == examination.EquipmentId)))
                    return Result.Failure(Error.Conflict("Examinations.OutsideStaffShift", "The appointment does not fit the assigned staff shift."));
            }
        }
        return Result.Success();
    }
}

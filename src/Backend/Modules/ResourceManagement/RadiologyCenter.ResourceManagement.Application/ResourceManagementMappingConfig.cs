using Mapster;
using RadiologyCenter.ResourceManagement.Application.DTOs;
using RadiologyCenter.ResourceManagement.Domain.Entities;

namespace RadiologyCenter.ResourceManagement.Application;

public static class ResourceManagementMappingConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<Staff, StaffDto>.NewConfig()
            .Map(d => d.Position, s => s.Position.Name);

        TypeAdapterConfig<Equipment, EquipmentDto>.NewConfig()
            .Map(d => d.Modality, s => s.Modality.Name)
            .Map(d => d.Status, s => s.Status.Name);

        TypeAdapterConfig<WorkShift, WorkShiftDto>.NewConfig();

        TypeAdapterConfig<Leave, LeaveDto>.NewConfig()
            .Map(d => d.LeaveType, s => s.LeaveType.Name);
    }
}

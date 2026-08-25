using Mapster;
using RadiologyCenter.BuildingBlocks.Application.Localization;
using RadiologyCenter.ResourceManagement.Application.DTOs;
using RadiologyCenter.ResourceManagement.Domain.Entities;

namespace RadiologyCenter.ResourceManagement.Application;

public static class ResourceManagementMappingConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<Staff, StaffDto>.NewConfig()
            .Map(d => d.Position, s => s.Position.LocalizedName())
            .Map(d => d.PositionKey, s => s.Position.Name)
            .Map(d => d.SalaryCalculationRule, s => s.SalaryCalculationRule.Name);

        TypeAdapterConfig<Equipment, EquipmentDto>.NewConfig()
            .Map(d => d.Modality, s => s.Modality.LocalizedName())
            .Map(d => d.ModalityKey, s => s.Modality.Name)
            .Map(d => d.Status, s => s.Status.LocalizedName())
            .Map(d => d.StatusKey, s => s.Status.Name);

        TypeAdapterConfig<WorkShift, WorkShiftDto>.NewConfig();

        TypeAdapterConfig<Leave, LeaveDto>.NewConfig()
            .Map(d => d.LeaveType, s => s.LeaveType.LocalizedName())
            .Map(d => d.LeaveTypeKey, s => s.LeaveType.Name);
    }
}

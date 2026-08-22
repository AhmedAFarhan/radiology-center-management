using Mapster;
using RadiologyCenter.BuildingBlocks.Application.Localization;
using RadiologyCenter.Patients.Application.DTOs;
using RadiologyCenter.Patients.Domain.Entities;

namespace RadiologyCenter.Patients.Application;

public static class PatientsMappingConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<Patient, PatientDto>.NewConfig()
            .Map(d => d.Gender, s => s.Gender.LocalizedName())
            .Map(d => d.GenderKey, s => s.Gender.Name)
            .Map(d => d.BloodType, s => s.BloodType != null ? s.BloodType.LocalizedName() : null)
            .Map(d => d.BloodTypeKey, s => s.BloodType != null ? s.BloodType.Name : null);
    }
}

using Mapster;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Domain.Entities;

namespace RadiologyCenter.Examinations.Application;

public static class ExaminationsMappingConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<ExaminationType, ExaminationTypeDto>.NewConfig()
            .Map(d => d.Modality, s => s.Modality.Name)
            .Map(d => d.Items, s => s.Items);

        TypeAdapterConfig<ExaminationTypeItem, ExaminationTypeItemDto>.NewConfig();

        TypeAdapterConfig<ExaminationItem, ExaminationItemDto>.NewConfig();
    }
}

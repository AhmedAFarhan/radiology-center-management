using Mapster;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Domain.Entities;

namespace RadiologyCenter.Examinations.Application;

public static class ExaminationsMappingConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<ExaminationItem, ExaminationItemDto>.NewConfig();
    }
}

using Mapster;
using RadiologyCenter.BuildingBlocks.Application.Localization;
using RadiologyCenter.Catalog.Application.DTOs;
using RadiologyCenter.Catalog.Domain.Entities;

namespace RadiologyCenter.Catalog.Application;

public static class CatalogMappingConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<ExaminationType, ExaminationTypeDto>.NewConfig()
            .Map(d => d.Modality, s => s.Modality.LocalizedName());
    }
}
using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.BuildingBlocks.Application.Localization;

public static class EnumerationLocalizationExtensions
{
    public static string LocalizedName(this Enumeration enumeration) => Translator.Current.TranslateEnum(enumeration.GetType().Name, enumeration.Name);
}
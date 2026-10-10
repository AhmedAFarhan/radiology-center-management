using System.Collections;

using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Localization;
using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Clinic.Extensions;

namespace RadiologyCenter.Clinic.Operations.Enums;

public class EnumsOperations : ClinicOperations
{
    private readonly ITranslator _translator;

    public EnumsOperations(ITranslator translator) => _translator = translator;

    /// <summary>
    /// Returns the localized options for a given enum type, e.g. GET /api/enums/Modality
    /// => [ { key: "XRay", value: "XRay" }, ... ] (Arabic: value = "أشعة سينية").
    /// Values are localized by the request's Accept-Language header.
    /// </summary>
    [RequireSignIn]
    [ClinicOperation("GET", "enums/{typeName}")]
    public ClinicResult GetOptions(string typeName)
    {
        if (!EnumerationCatalog.Types.TryGetValue(typeName, out var type))
            return Result.Success<IReadOnlyList<EnumOptionDto>>(Array.Empty<EnumOptionDto>()).ToActionResult();

        var options = GetAllOptions(type).Select(e => new EnumOptionDto(e.Name, _translator.TranslateEnum(type.Name, e.Name))).ToList();
        return Result.Success<IReadOnlyList<EnumOptionDto>>(options).ToActionResult();
    }

    private static IEnumerable<Enumeration> GetAllOptions(Type type)
    {
        var method = typeof(Enumeration).GetMethod(nameof(Enumeration.GetAll))!.MakeGenericMethod(type);
        return ((IEnumerable)method.Invoke(null, null)!).Cast<Enumeration>();
    }
}

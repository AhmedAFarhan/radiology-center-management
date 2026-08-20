using System.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Localization;
using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Localhost.Extensions;

namespace RadiologyCenter.Localhost.Controllers.Enums;

[ApiController]
[Route("api/enums")]
public class EnumsController : ControllerBase
{
    private readonly ITranslator _translator;

    public EnumsController(ITranslator translator) => _translator = translator;

    /// <summary>
    /// Returns the localized options for a given enum type, e.g. GET /api/enums/Modality
    /// => [ { key: "XRay", value: "XRay" }, ... ] (Arabic: value = "أشعة سينية").
    /// Values are localized by the request's Accept-Language header.
    /// </summary>
    [Authorize]
    [HttpGet("{typeName}")]
    public IActionResult GetOptions(string typeName)
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

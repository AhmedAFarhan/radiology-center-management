namespace RadiologyCenter.BuildingBlocks.Application.Common;

/// <summary>
/// A single localized option of an enum, exposed to clients as { key, value }.
/// <paramref name="Key"/> is the stable enum name (e.g. "XRay") and
/// <paramref name="Value"/> is the localized display text (e.g. "أشعة سينية").
/// </summary>
public sealed record EnumOptionDto(string Key, string Value);

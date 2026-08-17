namespace RadiologyCenter.Desktop.Models;

public sealed record GlobalSearchItemDto(
    string Id,
    string Title,
    string? Subtitle);

public sealed record GlobalSearchGroupDto(
    string EntityType,
    IReadOnlyList<GlobalSearchItemDto> Items);
namespace RadiologyCenter.Identity.Application.DTOs;

public record PermissionDto(
    string Code,
    string Name,
    string? Description,
    string? Group);
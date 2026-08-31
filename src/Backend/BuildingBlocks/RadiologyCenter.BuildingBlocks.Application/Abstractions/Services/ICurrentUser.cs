namespace RadiologyCenter.BuildingBlocks.Application.Abstractions;

public interface ICurrentUser
{
    string? Id { get; }
    string? Name { get; }
    string? TimeZoneId { get; }
    bool IsAuthenticated { get; }
}

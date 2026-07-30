namespace RadiologyCenter.BuildingBlocks.Domain.Entities;

public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    public TId Id { get; protected set; } = default!;

    protected Entity(TId id) => Id = id;

    protected Entity() { }

    public override bool Equals(object? obj) =>
        obj is Entity<TId> other && Id.Equals(other.Id);

    public bool Equals(Entity<TId>? other) =>
        other is not null && Id.Equals(other.Id);

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) =>
        left?.Equals(right) ?? right is null;

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) =>
        !(left == right);
}

using System.Reflection;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.BuildingBlocks.Domain.Localization;

namespace RadiologyCenter.BuildingBlocks.Domain.Common;

public abstract class Enumeration : IComparable
{
    public int Value { get; }
    public string Name { get; }

    protected Enumeration(int value, string name)
    {
        Value = value;
        Name = name;
    }

    public override string ToString() => Name;

    public override bool Equals(object? obj) =>
        obj is Enumeration other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public int CompareTo(object? other) => Value.CompareTo(((Enumeration)other!).Value);

    public static IEnumerable<T> GetAll<T>() where T : Enumeration
    {
        return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();
    }

    public static T FromValue<T>(int value) where T : Enumeration
    {
        var item = GetAll<T>().FirstOrDefault(e => e.Value == value)
            ?? throw new DomainException(MessageCodes.Shared.InvalidValue, $"'{value}' is not a valid value for {typeof(T).Name}.");
        return item;
    }

    public static T FromName<T>(string name) where T : Enumeration
    {
        var item = GetAll<T>().FirstOrDefault(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            ?? throw new DomainException(MessageCodes.Shared.InvalidName, $"'{name}' is not a valid name for {typeof(T).Name}.");
        return item;
    }

    public static bool operator ==(Enumeration? left, Enumeration? right) =>
        left?.Equals(right) ?? right is null;

    public static bool operator !=(Enumeration? left, Enumeration? right) =>
        !(left == right);

    public static bool operator <(Enumeration left, Enumeration right) => left.Value < right.Value;

    public static bool operator >(Enumeration left, Enumeration right) => left.Value > right.Value;

    public static implicit operator int(Enumeration enumeration) => enumeration.Value;
}

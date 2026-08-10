using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Cash.Domain.Enumerations;

public sealed class CashEntryDirection : Enumeration
{
    public static readonly CashEntryDirection In = new(1, "In");
    public static readonly CashEntryDirection Out = new(2, "Out");

    private CashEntryDirection(int value, string name) : base(value, name) { }
}
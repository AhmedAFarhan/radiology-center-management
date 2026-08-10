using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Cash.Domain.Enumerations;

public sealed class CashSessionStatus : Enumeration
{
    public static readonly CashSessionStatus Open = new(1, "Open");
    public static readonly CashSessionStatus Closed = new(2, "Closed");

    private CashSessionStatus(int value, string name) : base(value, name) { }
}
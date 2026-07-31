using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Inventory.Domain.Enumerations;

public sealed class UnitType : Enumeration
{
    public static readonly UnitType Piece = new(1, "Piece");
    public static readonly UnitType Box = new(2, "Box");
    public static readonly UnitType Bottle = new(3, "Bottle");
    public static readonly UnitType Vial = new(4, "Vial");
    public static readonly UnitType Ampoule = new(5, "Ampoule");
    public static readonly UnitType Pack = new(6, "Pack");
    public static readonly UnitType Tube = new(7, "Tube");
    public static readonly UnitType Roll = new(8, "Roll");
    public static readonly UnitType Sheet = new(9, "Sheet");
    public static readonly UnitType Kit = new(10, "Kit");

    private UnitType(int value, string name) : base(value, name) { }
}

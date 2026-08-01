namespace RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;

public class NumberSequence
{
    public string Name { get; set; } = default!;
    public int Year { get; set; }
    public int LastNumber { get; set; }
}

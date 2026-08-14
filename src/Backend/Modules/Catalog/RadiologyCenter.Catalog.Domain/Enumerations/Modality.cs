using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Catalog.Domain.Enumerations;

public sealed class Modality : Enumeration
{
    public static readonly Modality XRay = new(1, "XRay");
    public static readonly Modality CT = new(2, "CT");
    public static readonly Modality MRI = new(3, "MRI");
    public static readonly Modality Ultrasound = new(4, "Ultrasound");
    public static readonly Modality Mammography = new(5, "Mammography");
    public static readonly Modality Fluoroscopy = new(6, "Fluoroscopy");
    public static readonly Modality DEXA = new(7, "DEXA");
    public static readonly Modality Other = new(8, "Other");

    private Modality(int value, string name) : base(value, name) { }
}

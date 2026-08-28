namespace RadiologyCenter.Desktop.Shared.Components;

public static class ValidationKeys
{
    // Patients
    public const string DobOrAgeRequired = "validation.dobOrAgeRequired";
    public const string DobNotFuture = "validation.dobNotFuture";
    public const string AgeRange = "validation.ageRange";
    public const string FullNameFirstLast = "validation.fullNameFirstLast";

    // Resources (Staff, ReferralDoctor)
    public const string FullNameFirstAndLast = "validation.fullNameFirstAndLast";
    public const string PhoneEgyptian = "validation.phoneEgyptian";
    public const string HireDateRequired = "validation.hireDateRequired";

    // Insurance (Policy)
    public const string EffectiveToBeforeFrom = "validation.effectiveToBeforeFrom";

    // Inventory (Item)
    public const string ReorderLevelNonNegative = "validation.reorderLevelNonNegative";
    public const string ReorderQuantityNonNegative = "validation.reorderQuantityNonNegative";
}

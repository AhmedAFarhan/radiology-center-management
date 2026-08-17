namespace RadiologyCenter.Desktop.Components.Dashboard;

public record RevenuePoint(string Month, decimal Amount);

public record DoctorAvailability(string Name, string Specialty, bool Available, string NextSlot);

public static class DashboardSampleData
{
    public static IReadOnlyList<RevenuePoint> Revenue { get; } = new List<RevenuePoint>
    {
        new("Jan", 42_000),
        new("Feb", 38_500),
        new("Mar", 51_200),
        new("Apr", 47_800),
        new("May", 58_900),
        new("Jun", 63_400),
        new("Jul", 60_100),
        new("Aug", 71_300),
        new("Sep", 68_900),
        new("Oct", 76_400),
        new("Nov", 82_100),
        new("Dec", 88_700),
    };

    public static IReadOnlyList<decimal> SparklineUp { get; } = new List<decimal> { 12, 17, 14, 21, 19, 26, 24, 31, 29, 36 };

    public static IReadOnlyList<decimal> SparklineDown { get; } = new List<decimal> { 24, 22, 25, 19, 21, 17, 18, 14, 12, 9 };

    public static IReadOnlyList<decimal> SparklineSteady { get; } = new List<decimal> { 15, 16, 14, 17, 16, 18, 17, 19, 18, 20 };

    public static IReadOnlyList<DoctorAvailability> Doctors { get; } = new List<DoctorAvailability>
    {
        new("Dr. Youssef Mahmoud", "Radiologist", true, "09:30"),
        new("Dr. Sara Ali", "CT Specialist", false, "11:00"),
        new("Dr. Karim Hassan", "MRI", true, "13:15"),
        new("Dr. Nour El-Din", "Ultrasound", false, "14:40"),
        new("Dr. Mohamed Fouad", "X-Ray", true, "16:00"),
    };

    public static string TopDoctorName { get; } = "Dr. Youssef Mahmoud";
    public static string TopDoctorSpecialty { get; } = "Senior Radiologist";
    public static int TopDoctorReferrals { get; } = 128;
}
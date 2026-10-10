using RadiologyCenter.Clinic;
namespace RadiologyCenter.Desktop.Services;
public static class DesktopConfiguration
{
    public static string ProfilePath => Environment.GetEnvironmentVariable("EGCARE_CLIENT_CONFIG_PATH")
        ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EGcare", "clinic.json");
    public static ClinicConnectionProfile? Profile
    {
        get { try { return File.Exists(ProfilePath)?ClinicConnectionProfile.Read(ProfilePath):null; } catch { return null; } }
    }
    public static bool UseClinicServer => Profile is not null;
    public static bool IsDevelopment {
        get {
#if DEBUG
            return !File.Exists(ProfilePath) && File.Exists(Path.Combine(AppContext.BaseDirectory, "appsettings.Development.json"));
#else
            return false;
#endif
        }
    }
}

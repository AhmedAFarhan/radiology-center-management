using MudBlazor;

namespace RadiologyCenter.Desktop.Shared;

public static class AppTheme
{
    public static MudTheme Default { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#4C58E0",
            PrimaryDarken = "#3A44B8",
            PrimaryLighten = "#7A84EA",
            Secondary = "#9BB3FD",
            Tertiary = "#34A853",
            Info = "#2196F3",
            Success = "#2E7D32",
            Warning = "#E65100",
            Error = "#D32F2F",
            Dark = "#334155",
            Background = "#F4F7FC",
            BackgroundGray = "#EAF0F8",
            Surface = "#FFFFFF",
            DrawerBackground = "#FFFFFF",
            DrawerText = "#1F2937",
            DrawerIcon = "#64748B",
            AppbarBackground = "#FFFFFF",
            AppbarText = "#1F2937",
            TextPrimary = "#111827",
            TextSecondary = "#6B7280",
            ActionDefault = "#6B7280",
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "16px",
            DrawerWidthLeft = "280px",
        },
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = new[] { "Montserrat", "Cairo", "sans-serif" },
            },
        },
    };
}
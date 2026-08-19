using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;
using MudBlazor.Utilities;
using Microsoft.AspNetCore.Components.Authorization;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var updateService = new UpdateService();
        updateService.Init();

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("RobotoSlab.ttf", "RobotoSlab");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddSingleton(updateService);
        builder.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopEnd;
        });

        builder.Services.AddSingleton<AppLocalizer>();
        builder.Services.AddSingleton<MudLocalizer, AppMudLocalizer>();
        builder.Services.AddSingleton<BackendStatusService>();
        builder.Services.AddSingleton<TokenStorage>();
        builder.Services.AddSingleton<PacsSyncService>();
        builder.Services.AddScoped<AppAuthenticationStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AppAuthenticationStateProvider>());
        builder.Services.AddScoped<ApiClient>();
        builder.Services.AddScoped<ReturnUrlService>();
        builder.Services.AddScoped<AnalyticsService>();
        builder.Services.AddScoped<AnalyticsPeriodService>();
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<DashboardService>();
        builder.Services.AddScoped<PatientService>();
        builder.Services.AddScoped<InventoryService>();
        builder.Services.AddScoped<ExaminationService>();
        builder.Services.AddScoped<IdentityService>();
        builder.Services.AddScoped<ResourceService>();
        builder.Services.AddScoped<PayrollService>();
        builder.Services.AddScoped<ReportService>();
        builder.Services.AddScoped<InsuranceService>();
        builder.Services.AddScoped<CashService>();
        builder.Services.AddScoped<NotificationService>();
        builder.Services.AddScoped<SearchService>();
        builder.Services.AddSingleton<SearchHistoryService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        builder.Logging.AddProvider(new SimpleFileLoggerProvider());

        return builder.Build();
    }
}

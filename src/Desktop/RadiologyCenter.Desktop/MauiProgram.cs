using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;
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
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddSingleton(updateService);
        builder.Services.AddMudServices();

        builder.Services.AddSingleton<AppLocalizer>();
        builder.Services.AddSingleton<BackendStatusService>();
        builder.Services.AddSingleton<TokenStorage>();
        builder.Services.AddScoped<AppAuthenticationStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AppAuthenticationStateProvider>());
        builder.Services.AddScoped<ApiClient>();
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

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

using Microsoft.Extensions.Logging;

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
        builder.Services.AddSingleton(updateService);

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

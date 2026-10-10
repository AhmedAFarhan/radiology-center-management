namespace RadiologyCenter.Desktop;
public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
#if WINDOWS
        blazorWebView.BlazorWebViewInitializing += (_, args) =>
            args.UserDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EGcare", "WebView2");
        blazorWebView.BlazorWebViewInitialized += (_,args) => {
            var core=args.WebView.CoreWebView2;
            core.AddWebResourceRequestedFilter("*",Microsoft.Web.WebView2.Core.CoreWebView2WebResourceContext.All);
            core.WebResourceRequested += async (_,requestArgs) => {
                var origin=Services.PacsService.Instance?.HttpEndpoint;
                if (origin is null || !Uri.TryCreate(requestArgs.Request.Uri,UriKind.Absolute,out var request)
                    || !Uri.TryCreate(origin,UriKind.Absolute,out var allowed)
                    || request.Scheme!=allowed.Scheme || request.Host!=allowed.Host || request.Port!=allowed.Port) return;
                using var deferral=requestArgs.GetDeferral();
                try {
                    await blazorWebView.TryDispatchAsync(services => {
                        var session=Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<Services.BusinessRuntimeService>(services);
                        if (session.PacsAuthorization is {} header) requestArgs.Request.Headers.SetHeader("Authorization",header);
                        else if(requestArgs.Request.Headers.Contains("Authorization")) requestArgs.Request.Headers.RemoveHeader("Authorization");
                    });
                } finally { deferral.Complete(); }
            };
        };
#endif
    }
}

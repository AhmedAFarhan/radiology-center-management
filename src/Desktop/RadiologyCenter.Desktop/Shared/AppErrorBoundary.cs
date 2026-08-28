using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Shared;

public sealed class AppErrorBoundary : ErrorBoundary, IDisposable
{
    private bool _subscribed;

    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private AppLocalizer T { get; set; } = default!;
    [Inject] private ILogger<AppErrorBoundary> Logger { get; set; } = default!;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (!_subscribed)
        {
            Navigation.LocationChanged += OnLocationChanged;
            _subscribed = true;
        }
        ErrorContent = BuildErrorContent;
    }

    protected override async Task OnErrorAsync(Exception exception)
    {
        Logger.LogError(exception, "Unhandled error surfaced by AppErrorBoundary.");
        await base.OnErrorAsync(exception);
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        Recover();
    }

    private RenderFragment<Exception> BuildErrorContent => exception => builder =>
    {
        var seq = 0;
        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "class", "error-boundary d-flex align-items-center justify-content-center");
        builder.AddAttribute(seq++, "style", "min-height:100vh;");
        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "class", "error-boundary-card text-center pa-6");
        builder.AddAttribute(seq++, "style", "max-width:460px;");
        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "class", "error-boundary-icon mx-auto mb-4");
        builder.AddContent(seq++, "⚠️");
        builder.CloseElement();
        builder.OpenElement(seq++, "h2");
        builder.AddContent(seq++, T.Error.Title);
        builder.CloseElement();
        builder.OpenElement(seq++, "p");
        builder.AddAttribute(seq++, "class", "text-secondary");
        builder.AddContent(seq++, T.Error.Message);
        builder.CloseElement();
        builder.OpenElement(seq++, "details");
        builder.AddAttribute(seq++, "class", "text-secondary mb-4");
        builder.OpenElement(seq++, "summary");
        builder.AddContent(seq++, T.Error.Details);
        builder.CloseElement();
        builder.OpenElement(seq++, "pre");
        builder.AddContent(seq++, exception.Message);
        builder.CloseElement();
        builder.CloseElement();
        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "class", "d-flex justify-content-center gap-2");
        builder.OpenElement(seq++, "button");
        builder.AddAttribute(seq++, "type", "button");
        builder.AddAttribute(seq++, "class", "mud-button-root");
        builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create(this, Reload));
        builder.AddContent(seq++, T.Error.Reload);
        builder.CloseElement();
        builder.OpenElement(seq++, "button");
        builder.AddAttribute(seq++, "type", "button");
        builder.AddAttribute(seq++, "class", "mud-button-root");
        builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create(this, GoHome));
        builder.AddContent(seq++, T.Error.GoHome);
        builder.CloseElement();
        builder.CloseElement();
        builder.CloseElement();
        builder.CloseElement();
    };

    private void Reload()
        => Navigation.NavigateTo(Navigation.Uri, forceLoad: true);

    private void GoHome()
        => Navigation.NavigateTo("/dashboard");

    public void Dispose()
    {
        if (_subscribed)
            Navigation.LocationChanged -= OnLocationChanged;
    }
}
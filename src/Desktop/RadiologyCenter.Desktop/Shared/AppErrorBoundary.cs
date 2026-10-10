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

    protected override Task OnErrorAsync(Exception exception)
    {
        Logger.LogError("Unhandled UI error ({FailureType}). {Stack}", exception.GetType().Name, exception.StackTrace);
        return Task.CompletedTask;
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        Recover();
    }

    private RenderFragment<Exception> BuildErrorContent => exception => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "error-boundary d-flex align-items-center justify-content-center");
        builder.AddAttribute(2, "style", "min-height:100vh;");
        builder.OpenElement(3, "div");
        builder.AddAttribute(4, "class", "error-boundary-card text-center pa-6");
        builder.AddAttribute(5, "style", "max-width:460px;");
        builder.OpenElement(6, "div");
        builder.AddAttribute(7, "class", "error-boundary-icon mx-auto mb-4");
        builder.AddContent(8, "⚠️");
        builder.CloseElement();
        builder.OpenElement(9, "h2");
        builder.AddContent(10, T.Error.Title);
        builder.CloseElement();
        builder.OpenElement(11, "p");
        builder.AddAttribute(12, "class", "text-secondary");
        builder.AddContent(13, T.Error.Message);
        builder.CloseElement();
        builder.OpenElement(14, "details");
        builder.AddAttribute(15, "class", "text-secondary mb-4");
        builder.OpenElement(16, "summary");
        builder.AddContent(17, T.Error.Details);
        builder.CloseElement();
        builder.OpenElement(18, "pre");
        builder.AddContent(19, exception.GetType().Name);
        builder.CloseElement();
        builder.CloseElement();
        builder.OpenElement(20, "div");
        builder.AddAttribute(21, "class", "d-flex justify-content-center gap-2");
        builder.OpenElement(22, "button");
        builder.AddAttribute(23, "type", "button");
        builder.AddAttribute(24, "class", "mud-button-root");
        builder.AddAttribute(25, "onclick", EventCallback.Factory.Create(this, Reload));
        builder.AddContent(26, T.Error.Reload);
        builder.CloseElement();
        builder.OpenElement(27, "button");
        builder.AddAttribute(28, "type", "button");
        builder.AddAttribute(29, "class", "mud-button-root");
        builder.AddAttribute(30, "onclick", EventCallback.Factory.Create(this, GoHome));
        builder.AddContent(31, T.Error.GoHome);
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

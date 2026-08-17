using Microsoft.AspNetCore.Components;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Components.Analytics;

public abstract class AnalyticsPageBase : ComponentBase, IDisposable
{
    [Inject]
    protected AnalyticsPeriodService Period { get; set; } = null!;

    protected bool Loading = true;
    protected bool Error;
    protected string? LastError;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Period.Changed += OnPeriodChanged;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await ReloadCore(notify: false);
    }

    protected Task ReloadAsync() => ReloadCore(notify: true);

    protected async Task ReloadCore(bool notify)
    {
        Loading = true;
        Error = false;
        LastError = null;
        try
        {
            await LoadAsync(Period.From, Period.To);
        }
        catch (Exception ex)
        {
            Error = true;
            LastError = ex.Message;
        }
        finally
        {
            Loading = false;
            if (notify)
                await InvokeAsync(StateHasChanged);
        }
    }

    private void OnPeriodChanged()
    {
        _ = ReloadAsync();
    }

    protected abstract Task LoadAsync(DateTime from, DateTime to);

    public virtual void Dispose()
    {
        Period.Changed -= OnPeriodChanged;
    }
}
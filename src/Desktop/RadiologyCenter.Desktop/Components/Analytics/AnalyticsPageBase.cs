using Microsoft.AspNetCore.Components;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Components.Analytics;

public abstract class AnalyticsPageBase : ComponentBase, IDisposable
{
    [Inject]
    protected AnalyticsPeriodService Period { get; set; } = null!;

    protected bool Loading = true;
    protected bool Error;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Period.Changed += OnPeriodChanged;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await ReloadAsync();
    }

    protected async Task ReloadAsync()
    {
        Loading = true;
        Error = false;
        try
        {
            await LoadAsync(Period.From, Period.To);
        }
        catch
        {
            Error = true;
        }
        finally
        {
            Loading = false;
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
        GC.SuppressFinalize(this);
    }
}
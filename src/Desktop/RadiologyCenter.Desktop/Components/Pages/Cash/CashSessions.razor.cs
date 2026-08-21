using MudBlazor;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Components.Pages.Cash;

public partial class CashSessions : ListPageBase<CashSessionDto>
{
    private string _status = string.Empty;
    private IReadOnlyList<EnumOptionDto> _statusOptions = Array.Empty<EnumOptionDto>();

    protected override string UnreachableMessage => T.Cash.Unreachable;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        try
        {
            _statusOptions = await EnumOptionsService.GetOptionsAsync("CashSessionStatus");
        }
        catch
        {
            // filter options are non-critical; leave empty
        }
    }

    protected override async Task<PagedResult<CashSessionDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
        => await CashService.GetSessionsPagedAsync(
            search,
            sortBy,
            sortDescending,
            page,
            pageSize,
            string.IsNullOrWhiteSpace(_status) ? null : _status,
            ct);

    private async Task OnStatusChangedAsync(string value)
    {
        _status = value;
        await ReloadAsync();
    }

    private async Task OpenSessionAsync()
    {
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<OpenCashSessionDialog>(T.Cash.OpenCashSessionTitle, options);
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }

    private async Task OpenDetailAsync(CashSessionDto session)
    {
        var parameters = new DialogParameters { ["SessionId"] = session.Id };
        var options = new DialogOptions { MaxWidth = MaxWidth.Large, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<CashSessionDetailDialog>(T.Cash.SessionTitle, parameters, options);
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }
}
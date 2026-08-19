using System.Net.Http;
using System.Net.Http.Json;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using MudBlazor;
using RadiologyCenter.Desktop;
using RadiologyCenter.Desktop.Components;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Components.Pages.Cash;

public partial class CashSessions : ComponentBase, IDisposable
{
private MudTable<CashSessionDto>? _table;
    private string? _search;
    private string _status = string.Empty;
    private CancellationTokenSource? _searchCts;
    private string? _loadError;
    private bool _offline;

    private async Task<TableData<CashSessionDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var page = await CashService.GetSessionsPagedAsync(
                _search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                string.IsNullOrWhiteSpace(_status) ? null : _status,
                ct);

            _loadError = null;
            _offline = false;
            return new TableData<CashSessionDto> { Items = page.Items, TotalItems = page.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<CashSessionDto> { Items = Array.Empty<CashSessionDto>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            _loadError = ex.Message;
            _offline = false;
            return new TableData<CashSessionDto> { Items = Array.Empty<CashSessionDto>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(T.Cash.Unreachable, Severity.Error);
            _loadError = T.Cash.Unreachable;
            _offline = true;
            return new TableData<CashSessionDto> { Items = Array.Empty<CashSessionDto>(), TotalItems = 0 };
        }
    }

    private async Task OnSearchChanged(string? value)
        => await DebounceReloadAsync(value);

    private async Task OnStatusChangedAsync(string value)
    {
        _status = value;
        await ReloadAsync();
    }

    private async Task DebounceReloadAsync(string? value)
    {
        _search = value;

        _searchCts?.Cancel();
        var cts = _searchCts = new CancellationTokenSource();
        try
        {
            await Task.Delay(400, cts.Token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        await ReloadAsync();
    }

    private Task ReloadAsync()
        => _table is null ? Task.CompletedTask : _table.ReloadServerData();

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

private static string ShortId(string id)
        => id.Length > 8 ? id[..8] : id;

    public void Dispose() => _searchCts?.Cancel();
}
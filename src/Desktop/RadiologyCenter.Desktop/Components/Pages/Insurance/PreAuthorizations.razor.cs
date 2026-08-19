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

namespace RadiologyCenter.Desktop.Components.Pages.Insurance;

public partial class PreAuthorizations : ComponentBase, IDisposable
{
private MudTable<PreAuthorizationListItemDto>? _table;
    private string? _search;
    private CancellationTokenSource? _searchCts;
    private string? _loadError;
    private bool _offline;

    private async Task<TableData<PreAuthorizationListItemDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var page = await InsuranceService.GetPreAuthorizationsPagedAsync(
                _search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                ct);

            _loadError = null;
            _offline = false;
            return new TableData<PreAuthorizationListItemDto> { Items = page.Items, TotalItems = page.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<PreAuthorizationListItemDto> { Items = Array.Empty<PreAuthorizationListItemDto>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            _loadError = ex.Message;
            _offline = false;
            return new TableData<PreAuthorizationListItemDto> { Items = Array.Empty<PreAuthorizationListItemDto>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(T.PreAuth.Unreachable, Severity.Error);
            _loadError = T.PreAuth.Unreachable;
            _offline = true;
            return new TableData<PreAuthorizationListItemDto> { Items = Array.Empty<PreAuthorizationListItemDto>(), TotalItems = 0 };
        }
    }

    private async Task OnSearchChanged(string? value)
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

        if (_table is not null)
            await _table.ReloadServerData();
    }

    private Task ReloadAsync()
        => _table is null ? Task.CompletedTask : _table.ReloadServerData();

    private async Task OpenCreateDialogAsync()
    {
        var options = new DialogOptions { MaxWidth = MaxWidth.Large, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<PreAuthDialog>(T.PreAuth.RequestTitle, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenDetailDialogAsync(PreAuthorizationListItemDto preAuth)
    {
        var parameters = new DialogParameters { ["PreAuthorization"] = preAuth };
        var options = new DialogOptions { MaxWidth = MaxWidth.Large, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<PreAuthDetailDialog>(preAuth.PatientName, parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ReloadIfSavedAsync(IDialogReference dialog)
    {
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }

    private static string FormatStatus(string status) => status switch
    {
        "Requested" => "Requested",
        "Approved" => "Approved",
        "Denied" => "Denied",
        _ => status,
    };

    public void Dispose() => _searchCts?.Cancel();
}
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

namespace RadiologyCenter.Desktop.Components.Pages.Payroll;

public partial class PayRuns : ComponentBase, IDisposable
{
[Inject] private PermissionService Permissions { get; set; } = default!;

    private bool Can(string code) => Permissions.HasPermission(code);

    private MudTable<PayRunDto>? _table;
    private string? _search;
    private CancellationTokenSource? _searchCts;
    private string? _loadError;
    private bool _offline;
    private bool _statsLoaded;
    private PayRunStats _stats = new();

    private async Task<TableData<PayRunDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var page = await PayrollService.GetPayRunsPagedAsync(
                _search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                ct);

            _stats = new PayRunStats
            {
                Total = page.Items.Count,
                Draft = page.Items.Count(p => p.Status == "Draft"),
                Computed = page.Items.Count(p => p.Status == "Computed"),
                Approved = page.Items.Count(p => p.Status == "Approved"),
                Paid = page.Items.Count(p => p.Status == "Paid"),
                Rejected = page.Items.Count(p => p.Status == "Rejected"),
            };
            _statsLoaded = true;

            _loadError = null;
            _offline = false;
            return new TableData<PayRunDto> { Items = page.Items, TotalItems = page.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<PayRunDto> { Items = Array.Empty<PayRunDto>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            _loadError = ex.Message;
            _offline = false;
            return new TableData<PayRunDto> { Items = Array.Empty<PayRunDto>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(T.Payroll.Unreachable, Severity.Error);
            _loadError = T.Payroll.Unreachable;
            _offline = true;
            return new TableData<PayRunDto> { Items = Array.Empty<PayRunDto>(), TotalItems = 0 };
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
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<PayRunEditorDialog>(T.Payroll.NewPayRun, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenDetailDialogAsync(PayRunDto payRun)
    {
        var parameters = new DialogParameters { ["PayRunId"] = payRun.Id };
        var options = new DialogOptions { MaxWidth = MaxWidth.Large, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<PayRunDetailDialog>(T.FormatValue(T.Payroll.PayRunDetailTitle, payRun.RunFrom.ToString("yyyy-MM-dd"), payRun.RunTo.ToString("yyyy-MM-dd")), parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ReloadIfSavedAsync(IDialogReference dialog)
    {
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }

    private async Task DeletePayRunAsync(PayRunDto payRun)
    {
        var parameters = new DialogParameters
        {
            ["Title"] = T.Payroll.DeleteTitle,
            ["Message"] = T.FormatValue(T.Payroll.DeleteConfirm, payRun.RunFrom.ToString("yyyy-MM-dd"), payRun.RunTo.ToString("yyyy-MM-dd")),
            ["Icon"] = Icons.Material.Filled.Delete,
            ["Color"] = MudBlazor.Color.Error,
            ["ConfirmText"] = T.Common.Delete,
            ["CancelText"] = T.Common.Cancel,
        };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, options);
        var result = await dialog.Result;

        if (result?.Canceled != false)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await PayrollService.DeletePayRunAsync(payRun.Id);
                Snackbar.Add(T.Payroll.Deleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Payroll.Unreachable);
    }

    private static string FormatStatus(string status) => status switch
    {
        "Computed" => "Computed",
        "Approved" => "Approved",
        "Rejected" => "Rejected",
        _ => status,
    };

    public void Dispose() => _searchCts?.Cancel();

    private sealed class PayRunStats
    {
        public int Total { get; set; }
        public int Draft { get; set; }
        public int Computed { get; set; }
        public int Approved { get; set; }
        public int Paid { get; set; }
        public int Rejected { get; set; }
    }
}
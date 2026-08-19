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

namespace RadiologyCenter.Desktop.Components.Pages.Resources;

public partial class Leaves : ComponentBase, IDisposable
{
private MudTable<LeaveDto>? _table;
    private string? _search;
    private CancellationTokenSource? _searchCts;
    private string? _loadError;
    private bool _offline;
    private IReadOnlyDictionary<string, string> _staffNames = new Dictionary<string, string>();

    private async Task<TableData<LeaveDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var leavesTask = ResourceService.GetLeavesPagedAsync(
                _search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                ct);
            var staffTask = ResourceService.GetStaffsPagedAsync(null, null, false, 1, 100, ct);

            await Task.WhenAll(leavesTask, staffTask);

            var leaves = await leavesTask;
            _staffNames = (await staffTask).Items.ToDictionary(s => s.Id, s => s.FullName);

            return new TableData<LeaveDto> { Items = leaves.Items, TotalItems = leaves.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<LeaveDto> { Items = Array.Empty<LeaveDto>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            _loadError = ex.Message;
            _offline = false;
            return new TableData<LeaveDto> { Items = Array.Empty<LeaveDto>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(T.Leave.Unreachable, Severity.Error);
            _loadError = T.Leave.Unreachable;
            _offline = true;
            return new TableData<LeaveDto> { Items = Array.Empty<LeaveDto>(), TotalItems = 0 };
        }
    }

    private string ResolveStaff(string staffId)
        => _staffNames.TryGetValue(staffId, out var name) ? name : staffId;

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
        var dialog = await DialogService.ShowAsync<LeaveEditorDialog>(T.Leave.NewLeave, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(LeaveDto leave)
    {
        var parameters = new DialogParameters { ["Leave"] = leave };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<LeaveEditorDialog>(T.Leave.EditLeave, parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ReloadIfSavedAsync(IDialogReference dialog)
    {
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }

    private async Task DeleteLeaveAsync(LeaveDto leave)
    {
        var confirmed = await DialogService.ShowMessageBoxAsync(
            T.Leave.DeleteTitle,
            T.FormatValue(T.Leave.DeleteConfirm, leave.LeaveType),
            T.Common.Delete,
            T.Common.Cancel);

        if (confirmed != true)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await ResourceService.DeleteLeaveAsync(leave.Id);
                Snackbar.Add(T.Leave.Deleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Leave.Unreachable);
    }

    private static string FormatLeaveType(string leaveType) => leaveType switch
    {
        "Annual" => "Annual",
        "Sick" => "Sick",
        "Unpaid" => "Unpaid",
        "Maternity" => "Maternity",
        _ => leaveType,
    };

    public void Dispose() => _searchCts?.Cancel();
}
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

public partial class WorkShifts : ComponentBase, IDisposable
{
private MudTable<WorkShiftDto>? _table;
    private string? _search;
    private CancellationTokenSource? _searchCts;
    private string? _loadError;
    private bool _offline;
    private IReadOnlyDictionary<string, string> _staffNames = new Dictionary<string, string>();
    private IReadOnlyDictionary<string, string> _equipmentNames = new Dictionary<string, string>();

    private async Task<TableData<WorkShiftDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var shiftsTask = ResourceService.GetWorkShiftsPagedAsync(
                _search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                ct);
            var staffTask = ResourceService.GetStaffsPagedAsync(null, null, false, 1, 100, ct);
            var equipmentTask = ResourceService.GetEquipmentPagedAsync(null, null, false, 1, 100, ct);

            await Task.WhenAll(shiftsTask, staffTask, equipmentTask);

            var shifts = await shiftsTask;
            _staffNames = (await staffTask).Items.ToDictionary(s => s.Id, s => s.FullName);
            _equipmentNames = (await equipmentTask).Items.ToDictionary(e => e.Id, e => e.Name);

            return new TableData<WorkShiftDto> { Items = shifts.Items, TotalItems = shifts.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<WorkShiftDto> { Items = Array.Empty<WorkShiftDto>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            _loadError = ex.Message;
            _offline = false;
            return new TableData<WorkShiftDto> { Items = Array.Empty<WorkShiftDto>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(T.WorkShift.Unreachable, Severity.Error);
            _loadError = T.WorkShift.Unreachable;
            _offline = true;
            return new TableData<WorkShiftDto> { Items = Array.Empty<WorkShiftDto>(), TotalItems = 0 };
        }
    }

    private string ResolveStaff(string staffId)
        => _staffNames.TryGetValue(staffId, out var name) ? name : staffId;

    private string ResolveEquipment(string equipmentId)
        => _equipmentNames.TryGetValue(equipmentId, out var name) ? name : equipmentId;

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
        var dialog = await DialogService.ShowAsync<WorkShiftEditorDialog>(T.WorkShift.NewWorkShift, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(WorkShiftDto shift)
    {
        var parameters = new DialogParameters { ["Shift"] = shift };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<WorkShiftEditorDialog>(T.WorkShift.EditWorkShift, parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ReloadIfSavedAsync(IDialogReference dialog)
    {
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }

    private async Task DeleteShiftAsync(WorkShiftDto shift)
    {
        var confirmed = await DialogService.ShowMessageBoxAsync(
            T.WorkShift.DeleteTitle,
            T.FormatValue(T.WorkShift.DeleteConfirm, shift.Date.ToString("yyyy-MM-dd")),
            T.Common.Delete,
            T.Common.Cancel);

        if (confirmed != true)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await ResourceService.DeleteWorkShiftAsync(shift.Id);
                Snackbar.Add(T.WorkShift.Deleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.WorkShift.Unreachable);
    }

    public void Dispose() => _searchCts?.Cancel();
}
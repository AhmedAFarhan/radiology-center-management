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

namespace RadiologyCenter.Desktop.Features.Resources.Pages;

public partial class WorkShifts : ListPageBase<WorkShiftDto>
{
    protected override string UnreachableMessage => T.WorkShift.Unreachable;

    private IReadOnlyDictionary<string, string> _staffNames = new Dictionary<string, string>();
    private IReadOnlyDictionary<string, string> _equipmentNames = new Dictionary<string, string>();

    protected override async Task<PagedResult<WorkShiftDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var shiftsTask = ResourceService.GetWorkShiftsPagedAsync(search, sortBy, sortDescending, page, pageSize, ct);
        var staffTask = ResourceService.GetStaffsPagedAsync(null, null, false, 1, 100, ct);
        var equipmentTask = ResourceService.GetEquipmentPagedAsync(null, null, false, 1, 100, ct);

        await Task.WhenAll(shiftsTask, staffTask, equipmentTask);

        var shifts = await shiftsTask;
        _staffNames = (await staffTask).Items.ToDictionary(s => s.Id, s => s.FullName);
        _equipmentNames = (await equipmentTask).Items.ToDictionary(e => e.Id, e => e.Name);
        return shifts;
    }

    private string ResolveStaff(string staffId)
        => _staffNames.TryGetValue(staffId, out var name) ? name : "-";

    private string ResolveEquipment(string equipmentId)
        => _equipmentNames.TryGetValue(equipmentId, out var name) ? name : "-";

    private async Task OpenCreateDialogAsync()
    {
        var dialog = await DialogService.ShowAsync<WorkShiftEditorDialog>(T.WorkShift.NewWorkShift, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(WorkShiftDto shift)
    {
        var parameters = new DialogParameters { ["Shift"] = shift };
        var dialog = await DialogService.ShowAsync<WorkShiftEditorDialog>(T.WorkShift.EditWorkShift, parameters, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task DeleteShiftAsync(WorkShiftDto shift)
    {
        var parameters = new DialogParameters
        {
            ["Title"] = T.WorkShift.DeleteTitle,
            ["Message"] = T.FormatValue(T.WorkShift.DeleteConfirm, shift.Date.ToString("yyyy-MM-dd")),
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
                await ResourceService.DeleteWorkShiftAsync(shift.Id);
                Snackbar.Add(T.WorkShift.Deleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.WorkShift.Unreachable);
    }
}

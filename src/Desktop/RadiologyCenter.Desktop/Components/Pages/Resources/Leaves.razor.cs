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

public partial class Leaves : ListPageBase<LeaveDto>
{
    protected override string UnreachableMessage => T.Leave.Unreachable;

    private IReadOnlyDictionary<string, string> _staffNames = new Dictionary<string, string>();

    protected override async Task<PagedResult<LeaveDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var leavesTask = ResourceService.GetLeavesPagedAsync(search, sortBy, sortDescending, page, pageSize, ct);
        var staffTask = ResourceService.GetStaffsPagedAsync(null, null, false, 1, 100, ct);

        await Task.WhenAll(leavesTask, staffTask);

        var leaves = await leavesTask;
        _staffNames = (await staffTask).Items.ToDictionary(s => s.Id, s => s.FullName);
        return leaves;
    }

    private string ResolveStaff(string staffId)
        => _staffNames.TryGetValue(staffId, out var name) ? name : staffId;

    private async Task OpenCreateDialogAsync()
    {
        var dialog = await DialogService.ShowAsync<LeaveEditorDialog>(T.Leave.NewLeave, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(LeaveDto leave)
    {
        var parameters = new DialogParameters { ["Leave"] = leave };
        var dialog = await DialogService.ShowAsync<LeaveEditorDialog>(T.Leave.EditLeave, parameters, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task DeleteLeaveAsync(LeaveDto leave)
    {
        var parameters = new DialogParameters
        {
            ["Title"] = T.Leave.DeleteTitle,
            ["Message"] = T.FormatValue(T.Leave.DeleteConfirm, leave.LeaveType),
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
                await ResourceService.DeleteLeaveAsync(leave.Id);
                Snackbar.Add(T.Leave.Deleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Leave.Unreachable);
    }
}
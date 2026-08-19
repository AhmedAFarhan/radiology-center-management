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

public partial class Staff : ComponentBase, IDisposable
{
private MudTable<StaffDto>? _table;
    private string? _search;
    private CancellationTokenSource? _searchCts;
    private string? _loadError;
    private bool _offline;
    private string? _openId;

    [SupplyParameterFromQuery(Name = "q")]
    public string? SearchQuery { get; set; }

    [SupplyParameterFromQuery(Name = "open")]
    public string? OpenId { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (!string.IsNullOrWhiteSpace(OpenId) && Guid.TryParse(OpenId, out _))
            _openId = OpenId;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_openId is not null)
        {
            var id = _openId;
            _openId = null;
            await OpenByDeepLinkAsync(id);
        }
    }

    private async Task OpenByDeepLinkAsync(string id)
    {
        StaffDto? staff = null;
        var ok = await SafeExecute.RunAsync(
            async () => staff = await ResourceService.GetStaffByIdAsync(id),
            Snackbar,
            () => T.Staff.Unreachable);

        if (ok && staff is not null)
        {
            var parameters = new DialogParameters { ["Staff"] = staff };
            var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
            var dialog = await DialogService.ShowAsync<StaffEditorDialog>(T.FormatValue(T.Staff.Edit, staff.FullName), parameters, options);
            await ReloadIfSavedAsync(dialog);
        }

        NavigationManager.NavigateTo("/resources/staff", replace: true);
    }

    private async Task<TableData<StaffDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var page = await ResourceService.GetStaffsPagedAsync(
                _search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                ct);

            _loadError = null;
            _offline = false;
            return new TableData<StaffDto> { Items = page.Items, TotalItems = page.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<StaffDto> { Items = Array.Empty<StaffDto>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            _loadError = ex.Message;
            _offline = false;
            return new TableData<StaffDto> { Items = Array.Empty<StaffDto>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(T.Staff.Unreachable, Severity.Error);
            _loadError = T.Staff.Unreachable;
            _offline = true;
            return new TableData<StaffDto> { Items = Array.Empty<StaffDto>(), TotalItems = 0 };
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
        var dialog = await DialogService.ShowAsync<StaffEditorDialog>(T.Staff.NewStaff, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(StaffDto staff)
    {
        var parameters = new DialogParameters { ["Staff"] = staff };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<StaffEditorDialog>(T.FormatValue(T.Staff.Edit, staff.FullName), parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ReloadIfSavedAsync(IDialogReference dialog)
    {
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }

    private async Task ToggleActiveAsync(StaffDto staff)
    {
        await SafeExecute.RunAsync(async () =>
            {
                if (staff.IsActive)
                    await ResourceService.DeactivateStaffAsync(staff.Id);
                else
                    await ResourceService.ActivateStaffAsync(staff.Id);

                Snackbar.Add(staff.IsActive ? T.Staff.Deactivated : T.Staff.Activated, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Staff.Unreachable);
    }

    private async Task DeleteStaffAsync(StaffDto staff)
    {
        var confirmed = await DialogService.ShowMessageBoxAsync(
            T.Staff.DeleteTitle,
            T.FormatValue(T.Staff.DeleteConfirm, staff.FullName),
            T.Common.Delete,
            T.Common.Cancel);

        if (confirmed != true)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await ResourceService.DeleteStaffAsync(staff.Id);
                Snackbar.Add(T.Staff.Deleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Staff.Unreachable);
    }

    private static string FormatPosition(string position) => position switch
    {
        "Technician" => "Technician",
        "Radiologist" => "Radiologist",
        "Receptionist" => "Receptionist",
        "Nurse" => "Nurse",
        _ => position,
    };

    public void Dispose() => _searchCts?.Cancel();
}
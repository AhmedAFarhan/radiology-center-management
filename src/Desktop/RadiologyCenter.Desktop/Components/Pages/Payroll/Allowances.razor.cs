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

public partial class Allowances : ComponentBase, IDisposable
{
private MudTable<AllowanceAssignmentDto>? _table;
    private string? _search;
    private CancellationTokenSource? _searchCts;
    private string? _loadError;
    private bool _offline;
    private IReadOnlyDictionary<string, string> _staffNames = new Dictionary<string, string>();

    private async Task<TableData<AllowanceAssignmentDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var allowancesTask = PayrollService.GetAllowancesPagedAsync(
                _search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                ct);
            var staffTask = ResourceService.GetStaffsPagedAsync(null, null, false, 1, 100, ct);

            await Task.WhenAll(allowancesTask, staffTask);

            var allowances = await allowancesTask;
            _staffNames = (await staffTask).Items.ToDictionary(s => s.Id, s => s.FullName);

            return new TableData<AllowanceAssignmentDto> { Items = allowances.Items, TotalItems = allowances.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<AllowanceAssignmentDto> { Items = Array.Empty<AllowanceAssignmentDto>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            _loadError = ex.Message;
            _offline = false;
            return new TableData<AllowanceAssignmentDto> { Items = Array.Empty<AllowanceAssignmentDto>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(T.Allowance.Unreachable, Severity.Error);
            _loadError = T.Allowance.Unreachable;
            _offline = true;
            return new TableData<AllowanceAssignmentDto> { Items = Array.Empty<AllowanceAssignmentDto>(), TotalItems = 0 };
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
        var dialog = await DialogService.ShowAsync<AllowanceEditorDialog>(T.Allowance.NewAllowance, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(AllowanceAssignmentDto allowance)
    {
        var parameters = new DialogParameters { ["Allowance"] = allowance };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<AllowanceEditorDialog>(T.FormatValue(T.Allowance.EditTitle, allowance.Name), parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ReloadIfSavedAsync(IDialogReference dialog)
    {
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }

    private async Task ToggleActiveAsync(AllowanceAssignmentDto allowance)
    {
        await SafeExecute.RunAsync(async () =>
            {
                if (allowance.IsActive)
                    await PayrollService.DeactivateAllowanceAsync(allowance.Id);
                else
                    await PayrollService.ActivateAllowanceAsync(allowance.Id);

                Snackbar.Add(allowance.IsActive ? T.Allowance.Deactivated : T.Allowance.Activated, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Allowance.Unreachable);
    }

    private async Task DeleteAllowanceAsync(AllowanceAssignmentDto allowance)
    {
        var confirmed = await DialogService.ShowMessageBoxAsync(
            T.Allowance.DeleteTitle,
            T.FormatValue(T.Allowance.DeleteConfirm, allowance.Name),
            T.Common.Delete,
            T.Common.Cancel);

        if (confirmed != true)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await PayrollService.DeleteAllowanceAsync(allowance.Id);
                Snackbar.Add(T.Allowance.Deleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Allowance.Unreachable);
    }

    private static string FormatFrequency(string frequency) => frequency switch
    {
        "OneTime" => "One Time",
        "Monthly" => "Monthly",
        "Quarterly" => "Quarterly",
        "Annual" => "Annual",
        _ => frequency,
    };

    public void Dispose() => _searchCts?.Cancel();
}
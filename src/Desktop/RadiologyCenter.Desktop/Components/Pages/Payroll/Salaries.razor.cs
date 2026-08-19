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

public partial class Salaries : ComponentBase, IDisposable
{
private MudTable<SalaryDto>? _table;
    private string? _search;
    private CancellationTokenSource? _searchCts;
    private string? _loadError;
    private bool _offline;
    private IReadOnlyDictionary<string, string> _staffNames = new Dictionary<string, string>();

    private async Task<TableData<SalaryDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var salariesTask = PayrollService.GetSalariesPagedAsync(
                _search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                ct);
            var staffTask = ResourceService.GetStaffsPagedAsync(null, null, false, 1, 100, ct);

            await Task.WhenAll(salariesTask, staffTask);

            var salaries = await salariesTask;
            _staffNames = (await staffTask).Items.ToDictionary(s => s.Id, s => s.FullName);

            return new TableData<SalaryDto> { Items = salaries.Items, TotalItems = salaries.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<SalaryDto> { Items = Array.Empty<SalaryDto>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            _loadError = ex.Message;
            _offline = false;
            return new TableData<SalaryDto> { Items = Array.Empty<SalaryDto>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(T.Salary.Unreachable, Severity.Error);
            _loadError = T.Salary.Unreachable;
            _offline = true;
            return new TableData<SalaryDto> { Items = Array.Empty<SalaryDto>(), TotalItems = 0 };
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
        var dialog = await DialogService.ShowAsync<SalaryEditorDialog>(T.Salary.NewSalary, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(SalaryDto salary)
    {
        var parameters = new DialogParameters { ["Salary"] = salary };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<SalaryEditorDialog>(T.Salary.EditTitle, parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ReloadIfSavedAsync(IDialogReference dialog)
    {
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }

    private async Task ToggleActiveAsync(SalaryDto salary)
    {
        await SafeExecute.RunAsync(async () =>
            {
                if (salary.IsActive)
                    await PayrollService.DeactivateSalaryAsync(salary.Id);
                else
                    await PayrollService.ActivateSalaryAsync(salary.Id);

                Snackbar.Add(salary.IsActive ? T.Salary.Deactivated : T.Salary.Activated, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Salary.Unreachable);
    }

    private async Task DeleteSalaryAsync(SalaryDto salary)
    {
        var confirmed = await DialogService.ShowMessageBoxAsync(
            T.Salary.DeleteTitle,
            T.Salary.DeleteConfirm,
            T.Common.Delete,
            T.Common.Cancel);

        if (confirmed != true)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await PayrollService.DeleteSalaryAsync(salary.Id);
                Snackbar.Add(T.Salary.Deleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Salary.Unreachable);
    }

    public void Dispose() => _searchCts?.Cancel();
}
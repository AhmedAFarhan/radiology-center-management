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

public partial class SalaryComponents : ComponentBase, IDisposable
{
private MudTable<SalaryComponentDto>? _table;
    private string? _search;
    private CancellationTokenSource? _searchCts;
    private string? _loadError;
    private bool _offline;

    private async Task<TableData<SalaryComponentDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var page = await PayrollService.GetSalaryComponentsPagedAsync(
                _search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                ct);

            _loadError = null;
            _offline = false;
            return new TableData<SalaryComponentDto> { Items = page.Items, TotalItems = page.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<SalaryComponentDto> { Items = Array.Empty<SalaryComponentDto>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            _loadError = ex.Message;
            _offline = false;
            return new TableData<SalaryComponentDto> { Items = Array.Empty<SalaryComponentDto>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(T.SalaryComponent.Unreachable, Severity.Error);
            _loadError = T.SalaryComponent.Unreachable;
            _offline = true;
            return new TableData<SalaryComponentDto> { Items = Array.Empty<SalaryComponentDto>(), TotalItems = 0 };
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
        var dialog = await DialogService.ShowAsync<SalaryComponentEditorDialog>(T.SalaryComponent.NewSalaryComponent, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(SalaryComponentDto component)
    {
        var parameters = new DialogParameters { ["Component"] = component };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<SalaryComponentEditorDialog>(T.FormatValue(T.SalaryComponent.EditTitle, component.Name), parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ReloadIfSavedAsync(IDialogReference dialog)
    {
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }

    private async Task ToggleActiveAsync(SalaryComponentDto component)
    {
        await SafeExecute.RunAsync(async () =>
            {
                if (component.IsActive)
                    await PayrollService.DeactivateSalaryComponentAsync(component.Id);
                else
                    await PayrollService.ActivateSalaryComponentAsync(component.Id);

                Snackbar.Add(component.IsActive ? T.SalaryComponent.Deactivated : T.SalaryComponent.Activated, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.SalaryComponent.Unreachable);
    }

    private async Task DeleteComponentAsync(SalaryComponentDto component)
    {
        var confirmed = await DialogService.ShowMessageBoxAsync(
            T.SalaryComponent.DeleteTitle,
            T.FormatValue(T.SalaryComponent.DeleteConfirm, component.Name),
            T.Common.Delete,
            T.Common.Cancel);

        if (confirmed != true)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await PayrollService.DeleteSalaryComponentAsync(component.Id);
                Snackbar.Add(T.SalaryComponent.Deleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.SalaryComponent.Unreachable);
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
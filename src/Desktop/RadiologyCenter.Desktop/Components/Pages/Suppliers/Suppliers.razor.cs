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

namespace RadiologyCenter.Desktop.Components.Pages.Suppliers;

public partial class Suppliers : ComponentBase, IDisposable
{
private MudTable<SupplierDto>? _table;
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
        SupplierDto? supplier = null;
        var ok = await SafeExecute.RunAsync(
            async () => supplier = await InventoryService.GetSupplierByIdAsync(id),
            Snackbar,
            () => T.Suppliers.Unreachable);

        if (ok && supplier is not null)
        {
            var parameters = new DialogParameters { ["Supplier"] = supplier };
            var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
            var dialog = await DialogService.ShowAsync<SupplierEditorDialog>(T.SupplierDialog.EditSupplier, parameters, options);
            await ReloadIfSavedAsync(dialog);
        }

        NavigationManager.NavigateTo("/inventory/suppliers", replace: true);
    }

    private async Task<TableData<SupplierDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var page = await InventoryService.GetSuppliersPagedAsync(
                _search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                ct);

            _loadError = null;
            _offline = false;
            return new TableData<SupplierDto> { Items = page.Items, TotalItems = page.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<SupplierDto> { Items = Array.Empty<SupplierDto>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            _loadError = ex.Message;
            _offline = false;
            return new TableData<SupplierDto> { Items = Array.Empty<SupplierDto>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(T.Suppliers.Unreachable, Severity.Error);
            _loadError = T.Suppliers.Unreachable;
            _offline = true;
            return new TableData<SupplierDto> { Items = Array.Empty<SupplierDto>(), TotalItems = 0 };
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
        var dialog = await DialogService.ShowAsync<SupplierEditorDialog>(T.SupplierDialog.NewSupplier, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(SupplierDto supplier)
    {
        var parameters = new DialogParameters { ["Supplier"] = supplier };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<SupplierEditorDialog>(T.SupplierDialog.EditSupplier, parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ReloadIfSavedAsync(IDialogReference dialog)
    {
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }

    private async Task ToggleActiveAsync(SupplierDto supplier)
    {
        await SafeExecute.RunAsync(async () =>
            {
                if (supplier.IsActive)
                    await InventoryService.DeactivateSupplierAsync(supplier.Id);
                else
                    await InventoryService.ActivateSupplierAsync(supplier.Id);

                Snackbar.Add(supplier.IsActive ? T.Suppliers.SupplierDeactivated : T.Suppliers.SupplierActivated, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Suppliers.Unreachable);
    }

    private async Task DeleteSupplierAsync(SupplierDto supplier)
    {
        var confirmed = await DialogService.ShowMessageBoxAsync(
            T.Suppliers.DeleteSupplierTitle,
            T.FormatValue(T.Suppliers.DeleteConfirm, supplier.Name),
            T.Common.Delete,
            T.Common.Cancel);

        if (confirmed != true)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await InventoryService.DeleteSupplierAsync(supplier.Id);
                Snackbar.Add(T.Suppliers.SupplierDeleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Suppliers.Unreachable);
    }

    public void Dispose() => _searchCts?.Cancel();
}
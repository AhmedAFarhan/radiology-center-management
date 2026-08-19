using MudBlazor;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Components.Pages.Suppliers;

public partial class Suppliers : ListPageBase<SupplierDto>
{
    protected override string BaseRoute => "/inventory/suppliers";

    protected override string UnreachableMessage => T.Suppliers.Unreachable;

    protected override async Task<PagedResult<SupplierDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
        => await InventoryService.GetSuppliersPagedAsync(search, sortBy, sortDescending, page, pageSize, ct);

    protected override async Task OpenByDeepLinkAsync(string id)
    {
        SupplierDto? supplier = null;
        var ok = await SafeExecute.RunAsync(
            async () => supplier = await InventoryService.GetSupplierByIdAsync(id),
            Snackbar,
            () => T.Suppliers.Unreachable);

        if (ok && supplier is not null)
        {
            var parameters = new DialogParameters { ["Supplier"] = supplier };
            var dialog = await DialogService.ShowAsync<SupplierEditorDialog>(T.SupplierDialog.EditSupplier, parameters, EditorDialogOptions);
            await ReloadIfSavedAsync(dialog);
        }

        NavigationManager.NavigateTo(BaseRoute, replace: true);
    }

    private async Task OpenCreateDialogAsync()
    {
        var dialog = await DialogService.ShowAsync<SupplierEditorDialog>(T.SupplierDialog.NewSupplier, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(SupplierDto supplier)
    {
        var parameters = new DialogParameters { ["Supplier"] = supplier };
        var dialog = await DialogService.ShowAsync<SupplierEditorDialog>(T.SupplierDialog.EditSupplier, parameters, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
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
}
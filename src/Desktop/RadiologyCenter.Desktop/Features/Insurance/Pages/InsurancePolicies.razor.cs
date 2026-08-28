using MudBlazor;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Features.Insurance.Pages;

public partial class InsurancePolicies : ListPageBase<InsurancePolicyListItemDto>
{
    protected override string BaseRoute => "/insurance/policies";

    protected override string UnreachableMessage => T.Policy.Unreachable;

    protected override async Task<PagedResult<InsurancePolicyListItemDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
        => await InsuranceService.GetPoliciesPagedAsync(search, sortBy, sortDescending, page, pageSize, ct);

    protected override async Task OpenByDeepLinkAsync(string id)
    {
        var parameters = new DialogParameters { ["PolicyId"] = id };
        var options = new DialogOptions { MaxWidth = MaxWidth.Large, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<PolicyDetailDialog>(T.Policy.Title, parameters, options);
        await ReloadIfSavedAsync(dialog);

        NavigationManager.NavigateTo(BaseRoute, replace: true);
    }

    private async Task OpenCreateDialogAsync()
    {
        var options = new DialogOptions { MaxWidth = MaxWidth.Large, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<PolicyEditorDialog>(T.Policy.NewTitle, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenDetailDialogAsync(InsurancePolicyListItemDto policy)
    {
        var parameters = new DialogParameters { ["PolicyId"] = policy.Id };
        var options = new DialogOptions { MaxWidth = MaxWidth.Large, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<PolicyDetailDialog>(policy.PolicyNumber, parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ToggleActiveAsync(InsurancePolicyListItemDto policy)
    {
        if (!await ConfirmDialogs.ConfirmStatusChangeAsync(DialogService, T, T.Policy.ToggleStatus, policy.PolicyNumber, !policy.IsActive))
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await InsuranceService.ChangePolicyStatusAsync(policy.Id, policy.IsActive ? "Deactivate" : "Reactivate");
                Snackbar.Add(policy.IsActive ? T.Policy.Deactivated : T.Policy.Reactivated, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Policy.Unreachable);
    }
}

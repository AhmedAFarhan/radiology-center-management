using MudBlazor;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Components.Pages.Insurance;

public partial class PreAuthorizations : ListPageBase<PreAuthorizationListItemDto>
{
    protected override string UnreachableMessage => T.PreAuth.Unreachable;

    protected override async Task<PagedResult<PreAuthorizationListItemDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
        => await InsuranceService.GetPreAuthorizationsPagedAsync(search, sortBy, sortDescending, page, pageSize, ct);

    private async Task OpenCreateDialogAsync()
    {
        var options = new DialogOptions { MaxWidth = MaxWidth.Large, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<PreAuthDialog>(T.PreAuth.RequestTitle, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenDetailDialogAsync(PreAuthorizationListItemDto preAuth)
    {
        var parameters = new DialogParameters { ["PreAuthorization"] = preAuth };
        var options = new DialogOptions { MaxWidth = MaxWidth.Large, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<PreAuthDetailDialog>(preAuth.PatientName, parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private static string FormatStatus(string status) => status switch
    {
        "Requested" => "Requested",
        "Approved" => "Approved",
        "Denied" => "Denied",
        _ => status,
    };
}
using MudBlazor;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Components.Pages.Insurance;

public partial class Claims : ListPageBase<ClaimListItemDto>
{
    protected override string UnreachableMessage => T.Claim.Unreachable;

    protected override async Task<PagedResult<ClaimListItemDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
        => await InsuranceService.GetClaimsPagedAsync(search, sortBy, sortDescending, page, pageSize, ct);

    private async Task OpenCreateDialogAsync()
    {
        var options = new DialogOptions { MaxWidth = MaxWidth.Large, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<ClaimCreateDialog>(T.Claim.CreateTitle, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenDetailDialogAsync(ClaimListItemDto claim)
    {
        var parameters = new DialogParameters { ["ClaimId"] = claim.Id };
        var options = new DialogOptions { MaxWidth = MaxWidth.Large, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<ClaimDetailDialog>(claim.PatientName, parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private static string FormatStatus(string status) => status switch
    {
        "Draft" => "Draft",
        "Submitted" => "Submitted",
        "Approved" => "Approved",
        "Rejected" => "Rejected",
        "Paid" => "Paid",
        _ => status,
    };
}
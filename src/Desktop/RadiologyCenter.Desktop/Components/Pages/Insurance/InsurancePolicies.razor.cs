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

namespace RadiologyCenter.Desktop.Components.Pages.Insurance;

public partial class InsurancePolicies : ComponentBase, IDisposable
{
private MudTable<InsurancePolicyListItemDto>? _table;
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
        var parameters = new DialogParameters { ["PolicyId"] = id };
        var options = new DialogOptions { MaxWidth = MaxWidth.Large, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<PolicyDetailDialog>(T.Policy.Title, parameters, options);
        await ReloadIfSavedAsync(dialog);

        NavigationManager.NavigateTo("/insurance/policies", replace: true);
    }

    private async Task<TableData<InsurancePolicyListItemDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var page = await InsuranceService.GetPoliciesPagedAsync(
                _search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                ct);

            _loadError = null;
            _offline = false;
            return new TableData<InsurancePolicyListItemDto> { Items = page.Items, TotalItems = page.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<InsurancePolicyListItemDto> { Items = Array.Empty<InsurancePolicyListItemDto>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            _loadError = ex.Message;
            _offline = false;
            return new TableData<InsurancePolicyListItemDto> { Items = Array.Empty<InsurancePolicyListItemDto>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(T.Policy.Unreachable, Severity.Error);
            _loadError = T.Policy.Unreachable;
            _offline = true;
            return new TableData<InsurancePolicyListItemDto> { Items = Array.Empty<InsurancePolicyListItemDto>(), TotalItems = 0 };
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

    private async Task ReloadIfSavedAsync(IDialogReference dialog)
    {
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }

    private async Task ToggleActiveAsync(InsurancePolicyListItemDto policy)
    {
        await SafeExecute.RunAsync(async () =>
            {
                await InsuranceService.ChangePolicyStatusAsync(policy.Id, policy.IsActive ? "Deactivate" : "Reactivate");
                Snackbar.Add(policy.IsActive ? T.Policy.Deactivated : T.Policy.Reactivated, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Policy.Unreachable);
    }

    public void Dispose() => _searchCts?.Cancel();
}
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
using RadiologyCenter.Desktop.Features.Insurance.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Features.Insurance.Pages;

public partial class Companies : ComponentBase, IDisposable
{
    private MudTable<InsuranceCompanyDto>? _table;
    private List<InsuranceCompanyDto> _companies = new();
    private string? _search;
    private CancellationTokenSource? _searchCts;
    private string? _loadError;
    private bool _offline;
    private bool _loaded;
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
        InsuranceCompanyDto? company = null;
        var ok = await SafeExecute.RunAsync(
            async () => { company = await InsuranceService.GetCompanyByIdAsync(id); },
            Snackbar,
            () => T.Insurance.Unreachable);

        if (ok && company is not null)
        {
            var parameters = new DialogParameters { ["Company"] = company };
            var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
            var dialog = await DialogService.ShowAsync<CompanyEditorDialog>(T.Insurance.EditCompanyTitle, parameters, options);
            await ReloadIfSavedAsync(dialog);
        }

        NavigationManager.NavigateTo("/insurance/companies", replace: true);
    }

    private IEnumerable<InsuranceCompanyDto> Filter(IEnumerable<InsuranceCompanyDto> companies)
    {
        if (string.IsNullOrWhiteSpace(_search))
            return companies;

        var term = _search.Trim();
        return companies.Where(c =>
            c.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
            || (c.Phone?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)
            || (c.Email?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)
            || (c.TaxId?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false));
    }

    private async Task LoadAsync(CancellationToken ct = default)
    {
        try
        {
            _companies = (await InsuranceService.GetCompaniesAsync(ct)).ToList();
            _loadError = null;
            _offline = false;
            _loaded = true;
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return;
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(SafeExecute.FormatError(ex), Severity.Error);
            _loadError = ex.Message;
            _offline = false;
        }
        catch (Exception)
        {
            _loadError = T.Insurance.Unreachable;
            _offline = true;
        }
    }

    private async Task<TableData<InsuranceCompanyDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        if (!_loaded)
            await LoadAsync(ct);

        if (_loadError is not null)
            return new TableData<InsuranceCompanyDto> { Items = Array.Empty<InsuranceCompanyDto>(), TotalItems = 0 };

        var query = Filter(_companies);
        if (!string.IsNullOrWhiteSpace(state.SortLabel))
        {
            query = state.SortDirection == SortDirection.Descending
                ? query.OrderByDescending(c => c.Name)
                : query.OrderBy(c => c.Name);
        }

        var items = query.ToList();
        return new TableData<InsuranceCompanyDto>
        {
            Items = items.Skip(state.Page * state.PageSize).Take(state.PageSize).ToList(),
            TotalItems = items.Count,
        };
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

    private async Task ReloadAsync()
    {
        _loaded = false;
        if (_table is not null)
            await _table.ReloadServerData();
        else
            await LoadAsync();
    }

    private async Task OpenCreateDialogAsync()
    {
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<CompanyEditorDialog>(T.Insurance.NewCompanyTitle, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(InsuranceCompanyDto company)
    {
        var parameters = new DialogParameters { ["Company"] = company };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<CompanyEditorDialog>(T.Insurance.EditCompanyTitle, parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ReloadIfSavedAsync(IDialogReference dialog)
    {
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }

    private async Task DeleteCompanyAsync(InsuranceCompanyDto company)
    {
        var parameters = new DialogParameters
        {
            ["Title"] = T.Insurance.DeleteCompanyTitle,
            ["Message"] = T.FormatValue(T.Insurance.DeleteCompanyConfirm, company.Name),
            ["Icon"] = Icons.Material.Filled.Delete,
            ["Color"] = MudBlazor.Color.Error,
            ["ConfirmText"] = T.Common.Delete,
            ["CancelText"] = T.Common.Cancel,
        };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, options);
        var result = await dialog.Result;

        if (result?.Canceled != false)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await InsuranceService.DeleteCompanyAsync(company.Id);
                Snackbar.Add(T.Insurance.CompanyDeleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Insurance.Unreachable);
    }

    public void Dispose() => _searchCts?.Cancel();
}

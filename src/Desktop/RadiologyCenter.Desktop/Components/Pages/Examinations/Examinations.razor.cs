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

namespace RadiologyCenter.Desktop.Components.Pages.Examinations;

public partial class Examinations : ComponentBase, IDisposable
{
private MudTable<ExaminationTypeDto>? _table;
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
        ExaminationTypeDto? type = null;
        var ok = await SafeExecute.RunAsync(
            async () => type = await ExaminationService.GetTypeByIdAsync(id),
            Snackbar,
            () => T.Examinations.Unreachable);

        if (ok && type is not null)
        {
            var parameters = new DialogParameters { ["Type"] = type };
            var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
            var dialog = await DialogService.ShowAsync<ExaminationTypeEditorDialog>(T.Examinations.EditType, parameters, options);
            await ReloadIfSavedAsync(dialog);
        }

        NavigationManager.NavigateTo("/examinations", replace: true);
    }

    private async Task<TableData<ExaminationTypeDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var page = await ExaminationService.GetTypesPagedAsync(
                _search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                ct);

            _loadError = null;
            _offline = false;
            return new TableData<ExaminationTypeDto> { Items = page.Items, TotalItems = page.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<ExaminationTypeDto> { Items = Array.Empty<ExaminationTypeDto>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            _loadError = ex.Message;
            _offline = false;
            return new TableData<ExaminationTypeDto> { Items = Array.Empty<ExaminationTypeDto>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(T.Examinations.Unreachable, Severity.Error);
            _loadError = T.Examinations.Unreachable;
            _offline = true;
            return new TableData<ExaminationTypeDto> { Items = Array.Empty<ExaminationTypeDto>(), TotalItems = 0 };
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
        var dialog = await DialogService.ShowAsync<ExaminationTypeEditorDialog>(T.Examinations.NewType, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(ExaminationTypeDto type)
    {
        var parameters = new DialogParameters { ["Type"] = type };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<ExaminationTypeEditorDialog>(T.Examinations.EditType, parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ReloadIfSavedAsync(IDialogReference dialog)
    {
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }

    private async Task ToggleActiveAsync(ExaminationTypeDto type)
    {
        await SafeExecute.RunAsync(async () =>
            {
                if (type.IsActive)
                    await ExaminationService.DeactivateTypeAsync(type.Id);
                else
                    await ExaminationService.ActivateTypeAsync(type.Id);

                Snackbar.Add(type.IsActive ? T.Examinations.Deactivated : T.Examinations.Activated, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Examinations.Unreachable);
    }

    private async Task DeleteTypeAsync(ExaminationTypeDto type)
    {
        var confirmed = await DialogService.ShowMessageBoxAsync(
            T.Examinations.DeleteTitle,
            T.FormatValue(T.Examinations.DeleteConfirm, type.Name),
            T.Common.Delete,
            T.Common.Cancel);

        if (confirmed != true)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await ExaminationService.DeleteTypeAsync(type.Id);
                Snackbar.Add(T.Examinations.Deleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.Examinations.Unreachable);
    }

    public void Dispose() => _searchCts?.Cancel();
}
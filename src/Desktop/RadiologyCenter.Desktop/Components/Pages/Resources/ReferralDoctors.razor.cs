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

namespace RadiologyCenter.Desktop.Components.Pages.Resources;

public partial class ReferralDoctors : ComponentBase, IDisposable
{
private MudTable<ReferralDoctorDto>? _table;
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
        ReferralDoctorDto? doctor = null;
        var ok = await SafeExecute.RunAsync(
            async () => doctor = await ResourceService.GetReferralDoctorByIdAsync(id),
            Snackbar,
            () => T.ReferralDoctor.Unreachable);

        if (ok && doctor is not null)
        {
            var parameters = new DialogParameters { ["Doctor"] = doctor };
            var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
            var dialog = await DialogService.ShowAsync<ReferralDoctorEditorDialog>(T.FormatValue(T.ReferralDoctor.Edit, doctor.FullName), parameters, options);
            await ReloadIfSavedAsync(dialog);
        }

        NavigationManager.NavigateTo("/resources/referral-doctors", replace: true);
    }

    private async Task<TableData<ReferralDoctorDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var page = await ResourceService.GetReferralDoctorsPagedAsync(
                _search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                ct);

            _loadError = null;
            _offline = false;
            return new TableData<ReferralDoctorDto> { Items = page.Items, TotalItems = page.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<ReferralDoctorDto> { Items = Array.Empty<ReferralDoctorDto>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            _loadError = ex.Message;
            _offline = false;
            return new TableData<ReferralDoctorDto> { Items = Array.Empty<ReferralDoctorDto>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(T.ReferralDoctor.Unreachable, Severity.Error);
            _loadError = T.ReferralDoctor.Unreachable;
            _offline = true;
            return new TableData<ReferralDoctorDto> { Items = Array.Empty<ReferralDoctorDto>(), TotalItems = 0 };
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
        var dialog = await DialogService.ShowAsync<ReferralDoctorEditorDialog>(T.ReferralDoctor.NewReferralDoctor, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(ReferralDoctorDto doctor)
    {
        var parameters = new DialogParameters { ["Doctor"] = doctor };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<ReferralDoctorEditorDialog>(T.FormatValue(T.ReferralDoctor.Edit, doctor.FullName), parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ReloadIfSavedAsync(IDialogReference dialog)
    {
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }

    private async Task ToggleActiveAsync(ReferralDoctorDto doctor)
    {
        await SafeExecute.RunAsync(async () =>
            {
                if (doctor.IsActive)
                    await ResourceService.DeactivateReferralDoctorAsync(doctor.Id);
                else
                    await ResourceService.ActivateReferralDoctorAsync(doctor.Id);

                Snackbar.Add(doctor.IsActive ? T.ReferralDoctor.Deactivated : T.ReferralDoctor.Activated, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.ReferralDoctor.Unreachable);
    }

    private async Task DeleteDoctorAsync(ReferralDoctorDto doctor)
    {
        var confirmed = await DialogService.ShowMessageBoxAsync(
            T.ReferralDoctor.DeleteTitle,
            T.FormatValue(T.ReferralDoctor.DeleteConfirm, doctor.FullName),
            T.Common.Delete,
            T.Common.Cancel);

        if (confirmed != true)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await ResourceService.DeleteReferralDoctorAsync(doctor.Id);
                Snackbar.Add(T.ReferralDoctor.Deleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.ReferralDoctor.Unreachable);
    }

    public void Dispose() => _searchCts?.Cancel();
}
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

public partial class ReferralFees : ComponentBase, IDisposable
{
private MudTable<ReferralFeeDto>? _table;
    private string? _search;
    private CancellationTokenSource? _searchCts;
    private string? _loadError;
    private bool _offline;
    private IReadOnlyDictionary<string, string> _doctorNames = new Dictionary<string, string>();
    private IReadOnlyDictionary<string, string> _examTypeNames = new Dictionary<string, string>();

    private async Task<TableData<ReferralFeeDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var feesTask = PayrollService.GetReferralFeesPagedAsync(
                _search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                ct);
            var doctorsTask = ResourceService.GetReferralDoctorsPagedAsync(null, null, false, 1, 100, ct);
            var typesTask = ExaminationService.GetTypesPagedAsync(null, null, false, 1, 100, ct);

            await Task.WhenAll(feesTask, doctorsTask, typesTask);

            var fees = await feesTask;
            _doctorNames = (await doctorsTask).Items.ToDictionary(d => d.Id, d => d.FullName);
            _examTypeNames = (await typesTask).Items.ToDictionary(t => t.Id, t => $"{t.Code} - {t.Name}");

            return new TableData<ReferralFeeDto> { Items = fees.Items, TotalItems = fees.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<ReferralFeeDto> { Items = Array.Empty<ReferralFeeDto>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            _loadError = ex.Message;
            _offline = false;
            return new TableData<ReferralFeeDto> { Items = Array.Empty<ReferralFeeDto>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(T.ReferralFee.Unreachable, Severity.Error);
            _loadError = T.ReferralFee.Unreachable;
            _offline = true;
            return new TableData<ReferralFeeDto> { Items = Array.Empty<ReferralFeeDto>(), TotalItems = 0 };
        }
    }

    private string ResolveDoctor(string doctorId)
        => _doctorNames.TryGetValue(doctorId, out var name) ? name : doctorId;

    private string ResolveExamType(string examTypeId)
        => _examTypeNames.TryGetValue(examTypeId, out var name) ? name : examTypeId;

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
        var dialog = await DialogService.ShowAsync<ReferralFeeEditorDialog>(T.ReferralFee.NewReferralFee, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(ReferralFeeDto fee)
    {
        var parameters = new DialogParameters { ["Fee"] = fee };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<ReferralFeeEditorDialog>(T.ReferralFee.EditReferralFee, parameters, options);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ReloadIfSavedAsync(IDialogReference dialog)
    {
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }

    private async Task ToggleActiveAsync(ReferralFeeDto fee)
    {
        await SafeExecute.RunAsync(async () =>
            {
                if (fee.IsActive)
                    await PayrollService.DeactivateReferralFeeAsync(fee.Id);
                else
                    await PayrollService.ActivateReferralFeeAsync(fee.Id);

                Snackbar.Add(fee.IsActive ? T.ReferralFee.Deactivated : T.ReferralFee.Activated, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.ReferralFee.Unreachable);
    }

    private async Task DeleteFeeAsync(ReferralFeeDto fee)
    {
        var confirmed = await DialogService.ShowMessageBoxAsync(
            T.ReferralFee.DeleteTitle,
            T.ReferralFee.DeleteConfirm,
            T.Common.Delete,
            T.Common.Cancel);

        if (confirmed != true)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await PayrollService.DeleteReferralFeeAsync(fee.Id);
                Snackbar.Add(T.ReferralFee.Deleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.ReferralFee.Unreachable);
    }

    public void Dispose() => _searchCts?.Cancel();
}
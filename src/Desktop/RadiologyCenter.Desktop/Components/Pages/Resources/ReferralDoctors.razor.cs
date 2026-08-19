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

public partial class ReferralDoctors : ListPageBase<ReferralDoctorDto>
{
    protected override string BaseRoute => "/resources/referral-doctors";

    protected override string UnreachableMessage => T.ReferralDoctor.Unreachable;

    protected override async Task<PagedResult<ReferralDoctorDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
        => await ResourceService.GetReferralDoctorsPagedAsync(search, sortBy, sortDescending, page, pageSize, ct);

    protected override async Task OpenByDeepLinkAsync(string id)
    {
        ReferralDoctorDto? doctor = null;
        var ok = await SafeExecute.RunAsync(
            async () => doctor = await ResourceService.GetReferralDoctorByIdAsync(id),
            Snackbar,
            () => T.ReferralDoctor.Unreachable);

        if (ok && doctor is not null)
        {
            var parameters = new DialogParameters { ["Doctor"] = doctor };
            var dialog = await DialogService.ShowAsync<ReferralDoctorEditorDialog>(T.FormatValue(T.ReferralDoctor.Edit, doctor.FullName), parameters, EditorDialogOptions);
            await ReloadIfSavedAsync(dialog);
        }

        NavigationManager.NavigateTo(BaseRoute, replace: true);
    }

    private async Task OpenCreateDialogAsync()
    {
        var dialog = await DialogService.ShowAsync<ReferralDoctorEditorDialog>(T.ReferralDoctor.NewReferralDoctor, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task OpenEditDialogAsync(ReferralDoctorDto doctor)
    {
        var parameters = new DialogParameters { ["Doctor"] = doctor };
        var dialog = await DialogService.ShowAsync<ReferralDoctorEditorDialog>(T.FormatValue(T.ReferralDoctor.Edit, doctor.FullName), parameters, EditorDialogOptions);
        await ReloadIfSavedAsync(dialog);
    }

    private async Task ToggleActiveAsync(ReferralDoctorDto doctor)
    {
        if (!await ConfirmDialogs.ConfirmStatusChangeAsync(DialogService, T, T.ReferralDoctor.ToggleStatus, doctor.FullName, !doctor.IsActive))
            return;

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
        var parameters = new DialogParameters
        {
            ["Title"] = T.ReferralDoctor.DeleteTitle,
            ["Message"] = T.FormatValue(T.ReferralDoctor.DeleteConfirm, doctor.FullName),
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
                await ResourceService.DeleteReferralDoctorAsync(doctor.Id);
                Snackbar.Add(T.ReferralDoctor.Deleted, Severity.Success);
                await ReloadAsync();
            },
            Snackbar,
            () => T.ReferralDoctor.Unreachable);
    }
}
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
using Color = MudBlazor.Color;

namespace RadiologyCenter.Desktop.Components.Pages.Cash;

public partial class CashSessionDetailDialog : ComponentBase
{
[Parameter] public string SessionId { get; set; } = string.Empty;

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private CashSessionDto? Session { get; set; }
    private List<CashEntryDto> _entries = new();
    private CashHandoverDto? _handover;
    private bool _busy;
    private bool _loadFailed;

    private bool IsOpen => Session?.Status == "Open";

    protected override async Task OnInitializedAsync()
        => await LoadAsync();

    private async Task LoadAsync()
    {
        try
        {
            Session = await CashService.GetByIdAsync(SessionId);
            _entries = (await CashService.GetEntriesAsync(SessionId)).ToList();
            _handover = Session?.Status == "Closed"
                ? await CashService.GetHandoverBySessionAsync(SessionId)
                : null;
            _loadFailed = false;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            _loadFailed = true;
        }
        catch (Exception)
        {
            Snackbar.Add(T.CashSession.Unreachable, Severity.Error);
            _loadFailed = true;
        }
    }

    private async Task AddEntryAsync()
    {
        var parameters = new DialogParameters { ["SessionId"] = SessionId };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<AddCashEntryDialog>(T.CashSession.AddCashEntryTitle, parameters, options);
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await LoadAsync();
    }

    private async Task CloseAsync()
    {
        var parameters = new DialogParameters
        {
            ["SessionId"] = SessionId,
            ["ExpectedTotal"] = Session?.Balance ?? 0,
        };
        var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<CloseCashSessionDialog>(T.CashSession.CloseCashSessionTitle, parameters, options);
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await LoadAsync();
    }

    private async Task ApproveAsync()
    {
var parameters = new DialogParameters
        {
            ["Title"] = T.CashSession.ApproveHandover,
            ["Message"] = T.CashSession.ApproveHandoverConfirm,
            ["Icon"] = Icons.Material.Filled.CheckCircle,
            ["Color"] = MudBlazor.Color.Success,
            ["ConfirmText"] = T.CashSession.Approve,
            ["CancelText"] = T.Common.Cancel,
        };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, options);
        var result = await dialog.Result;

        if (result?.Canceled != false)
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await CashService.ApproveHandoverAsync(SessionId);
                Snackbar.Add(T.CashSession.HandoverApproved, Severity.Success);
                await LoadAsync();
            },
            Snackbar,
            () => T.CashSession.Unreachable,
            busy => _busy = busy);
    }

    private static Color OverShortColor(decimal amount) => amount switch
    {
        > 0 => Color.Success,
        < 0 => Color.Error,
_ => Color.Default,
    };

    private void CloseDialogAsync()
        => MudDialog.Close(DialogResult.Ok(!_loadFailed));
}
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

namespace RadiologyCenter.Desktop.Components.Pages.Visits;

public partial class VisitViewDialog : ComponentBase
{
[Parameter] public ExaminationDto Visit { get; set; } = default!;

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private ExaminationDto? _visit;
    private string _patientName = string.Empty;
    private string? _loadError;
    private bool _loading = true;
    private readonly Dictionary<string, string> _itemNames = new();

    protected override async Task OnInitializedAsync()
        => await LoadAsync();

    private async Task LoadAsync()
    {
        _loading = true;
        _loadError = null;
        _patientName = T.Common.Loading;
        try
        {
            var itemsTask = InventoryService.GetItemsPagedAsync(null, "Name", false, 1, 500);
            _visit = await ExaminationService.GetByIdAsync(Visit.Id);

            var items = await itemsTask;
            foreach (var item in items.Items)
                _itemNames[item.Id] = item.Name;

            try
            {
                var patient = await PatientService.GetByIdAsync(_visit.PatientId);
                _patientName = patient.FullName;
            }
            catch
            {
                _patientName = Visit.PatientId;
            }
        }
        catch (ApiException ex)
        {
            _loadError = ex.Message;
        }
        catch (Exception)
        {
            _loadError = T.VisitDialog.Unreachable;
        }
        finally
        {
            _loading = false;
        }
    }

    private string ResolveItemName(string itemId)
        => _itemNames.TryGetValue(itemId, out var name) ? name : itemId;

    private void CloseAsync()
        => MudDialog.Cancel();
}
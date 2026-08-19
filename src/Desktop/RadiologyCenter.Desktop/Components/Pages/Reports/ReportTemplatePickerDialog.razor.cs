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

namespace RadiologyCenter.Desktop.Components.Pages.Reports;

public partial class ReportTemplatePickerDialog : ComponentBase
{
[CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private string? _search;
    private bool _loading;
    private bool _busy;
    private IReadOnlyList<ReportTemplateDto> _templates = Array.Empty<ReportTemplateDto>();

    private string? _error;

    private IEnumerable<ReportTemplateDto> FilteredTemplates =>
        _templates.Where(t =>
            string.IsNullOrWhiteSpace(_search) ||
            t.Name.Contains(_search, StringComparison.OrdinalIgnoreCase) ||
            (t.Modality?.Contains(_search, StringComparison.OrdinalIgnoreCase) ?? false));

    protected override async Task OnInitializedAsync()
        => await LoadAsync();

    private async Task LoadAsync()
    {
        _loading = true;
        _error = null;
        try
        {
            var result = await ReportService.GetTemplatesPagedAsync(null, true, 1, 100);
            _templates = result.Items;
        }
        catch (Exception)
        {
            _error = T.ReportTemplate.LoadFailed;
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task PickAsync(ReportTemplateDto template)
    {
        _busy = true;
        try
        {
            MudDialog.Close(DialogResult.Ok(template.Id));
        }
        finally
        {
            _busy = false;
        }
    }

    private void CancelAsync()
        => MudDialog.Cancel();
}
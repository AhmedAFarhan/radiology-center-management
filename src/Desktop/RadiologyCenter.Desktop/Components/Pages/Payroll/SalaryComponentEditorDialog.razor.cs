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

public partial class SalaryComponentEditorDialog : ComponentBase
{
[Parameter] public SalaryComponentDto? Component { get; set; }

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private static readonly string[] Frequencies = { "OneTime", "Monthly", "Quarterly", "Annual" };

    private readonly SalaryComponentFormModel _model = new();
    private EditContext _editContext = default!;
    private bool _busy;

    private bool IsEdit => Component is not null;

    protected override void OnInitialized()
    {
        _editContext = new EditContext(_model);

        if (Component is null)
            return;

        _model.Name = Component.Name;
        _model.Kind = Component.Kind;
        _model.Frequency = Component.Frequency;
        _model.IsPercentage = Component.IsPercentage;
        _model.IsPerWorkDay = Component.IsPerWorkDay;
        _model.DefaultValue = Component.DefaultValue;
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        await SafeExecute.RunAsync(async () =>
            {
                var input = new SalaryComponentInput
                {
                    Name = _model.Name,
                    Kind = _model.Kind,
                    Frequency = string.IsNullOrWhiteSpace(_model.Frequency) ? null : _model.Frequency,
                    IsPercentage = _model.IsPercentage,
                    IsPerWorkDay = _model.IsPerWorkDay,
                    DefaultValue = _model.DefaultValue,
                };

                if (IsEdit)
                    await PayrollService.UpdateSalaryComponentAsync(Component!.Id, input);
                else
                    await PayrollService.CreateSalaryComponentAsync(input);

                Snackbar.Add(IsEdit ? T.SalaryComponent.Updated : T.SalaryComponent.Created, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            },
            Snackbar,
            () => T.SalaryComponent.UnreachableTryAgain,
            busy => _busy = busy);
    }

    private void CancelAsync()
        => MudDialog.Cancel();

    private static string FormatFrequency(string frequency) => frequency switch
    {
        "OneTime" => "One Time",
        "Monthly" => "Monthly",
        "Quarterly" => "Quarterly",
        "Annual" => "Annual",
        _ => frequency,
    };

    private sealed class SalaryComponentFormModel
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name must be 100 characters or fewer.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kind is required.")]
        public string Kind { get; set; } = "Earning";

        public string? Frequency { get; set; }

        public bool IsPercentage { get; set; }

        public bool IsPerWorkDay { get; set; }

        public decimal DefaultValue { get; set; }
    }
}
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

namespace RadiologyCenter.Desktop.Features.Payroll.Pages;

public partial class SalaryComponentEditorDialog : EditorDialogBase
{
    [Parameter] public SalaryComponentDto? Component { get; set; }

    private IReadOnlyList<EnumOptionDto> _kindOptions = Array.Empty<EnumOptionDto>();
    private IReadOnlyList<EnumOptionDto> _frequencyOptions = Array.Empty<EnumOptionDto>();

    private readonly SalaryComponentFormModel _model = new();
    private EditContext _editContext = default!;

    private bool IsEdit => Component is not null;

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_model);

        await SafeExecute.RunAsync(async () =>
            {
                _kindOptions = await EnumOptionsService.GetOptionsAsync("ComponentKind");
                _frequencyOptions = await EnumOptionsService.GetOptionsAsync("Frequency");
            },
            Snackbar,
            () => T.SalaryComponent.UnreachableTryAgain);

        if (Component is null)
            return;

        _model.Name = Component.Name;
        _model.Kind = Component.KindKey;
        _model.Frequency = Component.FrequencyKey;
        _model.IsPercentage = Component.IsPercentage;
        _model.IsPerWorkDay = Component.IsPerWorkDay;
        _model.DefaultValue = Component.DefaultValue;
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        var input = new SalaryComponentInput
        {
            Name = _model.Name,
            Kind = _model.Kind,
            Frequency = string.IsNullOrWhiteSpace(_model.Frequency) ? null : _model.Frequency,
            IsPercentage = _model.IsPercentage,
            IsPerWorkDay = _model.IsPerWorkDay,
            DefaultValue = _model.DefaultValue,
        };

        if (await TrySaveAsync(
                () => IsEdit
                    ? PayrollService.UpdateSalaryComponentAsync(Component!.Id, input)
                    : PayrollService.CreateSalaryComponentAsync(input),
                () => T.SalaryComponent.UnreachableTryAgain))
        {
            Snackbar.Add(IsEdit ? T.SalaryComponent.Updated : T.SalaryComponent.Created, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

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

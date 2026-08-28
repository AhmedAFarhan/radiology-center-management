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
using RadiologyCenter.Desktop.Features.Payroll.Models;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Features.Payroll.Components;

public partial class AllowanceEditorDialog : EditorDialogBase
{
    [Parameter] public AllowanceAssignmentDto? Allowance { get; set; }

    private IReadOnlyList<EnumOptionDto> _frequencyOptions = Array.Empty<EnumOptionDto>();

    private readonly AllowanceFormModel _model = new();
    private EditContext _editContext = default!;
    private StaffDto? _selectedStaff;
    private SalaryComponentDto? _selectedComponent;
    private string _staffId = string.Empty;
    private string _componentId = string.Empty;
    private string _employeeName = string.Empty;
    private string _componentName = string.Empty;

    private bool IsEdit => Allowance is not null;

    private async Task<IEnumerable<StaffDto>> SearchStaffAsync(string? value, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<StaffDto>();

        try
        {
            var page = await ResourceService.GetStaffsPagedAsync(value, "LastName", false, 1, 20, ct);
            return page.Items;
        }
        catch (Exception)
        {
            return Array.Empty<StaffDto>();
        }
    }

    private async Task<IEnumerable<SalaryComponentDto>> SearchComponentsAsync(string? value, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<SalaryComponentDto>();

        try
        {
            var page = await PayrollService.GetSalaryComponentsPagedAsync(value, "Name", false, 1, 20, ct);
            return page.Items;
        }
        catch (Exception)
        {
            return Array.Empty<SalaryComponentDto>();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_model);

        await SafeExecute.RunAsync(async () =>
            _frequencyOptions = await EnumOptionsService.GetOptionsAsync("Frequency"),
            Snackbar,
            () => T.AllowanceDialog.Unreachable);

        if (Allowance is null)
        {
            _model.EffectiveDate = DateTime.Today;
            return;
        }

        _staffId = Allowance.StaffId;
        _componentId = Allowance.SalaryComponentId ?? string.Empty;
        _model.Name = Allowance.Name;
        _model.Amount = Allowance.Amount;
        _model.Frequency = Allowance.Frequency;
        _model.IsPerWorkDay = Allowance.IsPerWorkDay;
        _model.EffectiveDate = Allowance.EffectiveDate;
        _model.EndDate = Allowance.EndDate;

        try
        {
            var staffTask = ResourceService.GetStaffByIdAsync(Allowance.StaffId);
            var componentTask = string.IsNullOrWhiteSpace(_componentId)
                ? null
                : PayrollService.GetSalaryComponentByIdAsync(_componentId);

            _employeeName = (await staffTask).FullName;
            if (componentTask is not null)
                _componentName = (await componentTask).Name;
        }
        catch (Exception)
        {
            _employeeName = Allowance.StaffId;
        }
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        if (!IsEdit && _selectedStaff is null)
        {
            Snackbar.Add(T.AllowanceDialog.SelectEmployee, Severity.Warning);
            return;
        }

        if (_model.EndDate is { } end && end < _model.EffectiveDate)
        {
            Snackbar.Add(T.AllowanceDialog.EndDateValidation, Severity.Warning);
            return;
        }

        var input = new AllowanceAssignmentInput
        {
            StaffId = IsEdit ? _staffId : _selectedStaff!.Id,
            SalaryComponentId = IsEdit
                ? (string.IsNullOrWhiteSpace(_componentId) ? null : _componentId)
                : _selectedComponent?.Id,
            Name = _model.Name,
            Amount = _model.Amount,
            Frequency = string.IsNullOrWhiteSpace(_model.Frequency) ? null : _model.Frequency,
            IsPerWorkDay = _model.IsPerWorkDay,
            EffectiveDate = _model.EffectiveDate.Value.Date,
            EndDate = _model.EndDate?.Date,
        };

        if (await TrySaveAsync(
                () => IsEdit
                    ? PayrollService.UpdateAllowanceAsync(Allowance!.Id, input)
                    : PayrollService.CreateAllowanceAsync(input),
                () => T.AllowanceDialog.Unreachable))
        {
            Snackbar.Add(IsEdit ? T.AllowanceDialog.Updated : T.AllowanceDialog.Created, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

}

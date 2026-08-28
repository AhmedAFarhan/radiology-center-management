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

public partial class SalaryEditorDialog : EditorDialogBase
{
    [Parameter] public SalaryDto? Salary { get; set; }

    private IReadOnlyList<EnumOptionDto> _salaryTypeOptions = Array.Empty<EnumOptionDto>();

    private readonly SalaryFormModel _model = new();
    private EditContext _editContext = default!;
    private StaffDto? _selectedStaff;
    private string _employeeName = string.Empty;
    private string _staffId = string.Empty;

    private bool IsEdit => Salary is not null;

    private async Task<IEnumerable<StaffDto>> SearchStaffAsync(string? value, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<StaffDto>();

        var page = await SafeExecute.RunAsync(
            () => ResourceService.GetStaffsPagedAsync(value, "LastName", false, 1, 20, ct),
            Snackbar,
            () => T.SalaryDialog.SearchError);
        return page?.Items ?? Array.Empty<StaffDto>();
    }

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_model);

        await SafeExecute.RunAsync(async () =>
            _salaryTypeOptions = await EnumOptionsService.GetOptionsAsync("SalaryType"),
            Snackbar,
            () => T.SalaryDialog.Unreachable);

        if (Salary is null)
        {
            _model.EffectiveDate = DateTime.Today;
            return;
        }

        _staffId = Salary.StaffId;
        _model.BaseSalary = Salary.BaseSalary;
        _model.SalaryType = Salary.SalaryTypeKey;
        _model.EffectiveDate = Salary.EffectiveDate;

        try
        {
            var staff = await ResourceService.GetStaffByIdAsync(Salary.StaffId);
            _employeeName = staff.FullName;
        }
        catch (Exception)
        {
            _employeeName = Salary.StaffId;
        }
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        if (!IsEdit && _selectedStaff is null)
        {
            Snackbar.Add(T.SalaryDialog.SelectEmployee, Severity.Warning);
            return;
        }

        var input = new SalaryInput
        {
            StaffId = IsEdit ? _staffId : _selectedStaff!.Id,
            BaseSalary = _model.BaseSalary,
            SalaryType = _model.SalaryType,
            EffectiveDate = _model.EffectiveDate.Value.Date,
        };

        if (await TrySaveAsync(
                () => IsEdit
                    ? PayrollService.UpdateSalaryAsync(Salary!.Id, input)
                    : PayrollService.CreateSalaryAsync(input),
                () => T.SalaryDialog.Unreachable))
        {
            Snackbar.Add(IsEdit ? T.SalaryDialog.Updated : T.SalaryDialog.Created, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

}

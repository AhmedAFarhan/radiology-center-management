using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using RadiologyCenter.Desktop.Features.Payroll.Models;
using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Features.Payroll.Pages;

public partial class AddPayslipDialog : ComponentBase
{
    [Parameter] public Func<string?, CancellationToken, Task<IEnumerable<StaffDto>>> StaffSearchFunc { get; set; } = default!;

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private readonly AddPayslipFormModel _model = new();
    private EditContext _editContext = default!;
    private StaffDto? _selectedStaff;

    protected override void OnInitialized()
    {
        _editContext = new EditContext(_model);
    }

    private void SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        if (_selectedStaff is null)
        {
            Snackbar.Add(T.Payslip.SelectEmployee, Severity.Warning);
            return;
        }

        MudDialog.Close(DialogResult.Ok(_selectedStaff.Id));
    }

    private void CancelAsync()
        => MudDialog.Cancel();

}

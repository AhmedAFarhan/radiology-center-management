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

public partial class PayRunEditorDialog : EditorDialogBase
{
    private readonly PayRunFormModel _model = new();
    private EditContext _editContext = default!;

    protected override void OnInitialized()
    {
        _editContext = new EditContext(_model);
        _model.RunFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        _model.RunTo = _model.RunFrom.Value.AddMonths(1).AddDays(-1);
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        if (_model.RunFrom is null || _model.RunTo is null)
        {
            Snackbar.Add(T.PayRunDialog.SelectBothDates, Severity.Warning);
            return;
        }

        if (_model.RunTo < _model.RunFrom)
        {
            Snackbar.Add(T.PayRunDialog.PeriodToAfterFrom, Severity.Warning);
            return;
        }

        if (await TrySaveAsync(
                () => PayrollService.CreatePayRunAsync(new CreatePayRunInput
                {
                    RunFrom = _model.RunFrom.Value.Date,
                    RunTo = _model.RunTo.Value.Date,
                    Notes = _model.Notes,
                }),
                () => T.PayRunDialog.Unreachable))
        {
            Snackbar.Add(T.PayRunDialog.Created, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

}

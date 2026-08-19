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

public partial class ExaminationFeeEditorDialog : ComponentBase
{
[Parameter] public ExaminationFeeDto? Fee { get; set; }

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private readonly ExaminationFeeFormModel _model = new();
    private EditContext _editContext = default!;
    private ExaminationTypeDto? _selectedType;
    private string _examTypeId = string.Empty;
    private string _examTypeName = string.Empty;
    private bool _busy;

    private bool IsEdit => Fee is not null;

    private async Task<IEnumerable<ExaminationTypeDto>> SearchTypesAsync(string? value, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<ExaminationTypeDto>();

        try
        {
            var page = await ExaminationService.GetTypesPagedAsync(value, "Name", false, 1, 20, ct);
            return page.Items;
        }
        catch (Exception)
        {
            return Array.Empty<ExaminationTypeDto>();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_model);

        if (Fee is null)
            return;

        _examTypeId = Fee.ExaminationTypeId;
        _model.Role = Fee.Role;
        _model.Amount = Fee.Amount;
        _model.IsPercentage = Fee.IsPercentage;

        try
        {
            var type = await ExaminationService.GetTypeByIdAsync(Fee.ExaminationTypeId);
            _examTypeName = $"{type.Code} - {type.Name}";
        }
        catch (Exception)
        {
            _examTypeName = Fee.ExaminationTypeId;
        }
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        if (!IsEdit && _selectedType is null)
        {
            Snackbar.Add(T.ExamFee.SelectExamType, Severity.Warning);
            return;
        }

        if (_model.IsPercentage && _model.Amount > 100)
        {
            Snackbar.Add(T.ExamFee.PercentageLimit, Severity.Warning);
            return;
        }

        await SafeExecute.RunAsync(async () =>
            {
                var input = new ExaminationFeeInput
                {
                    ExaminationTypeId = IsEdit ? _examTypeId : _selectedType!.Id,
                    Role = _model.Role,
                    Amount = _model.Amount,
                    IsPercentage = _model.IsPercentage,
                };

                if (IsEdit)
                    await PayrollService.UpdateExaminationFeeAsync(Fee!.Id, input);
                else
                    await PayrollService.CreateExaminationFeeAsync(input);

                Snackbar.Add(IsEdit ? T.ExamFee.Updated : T.ExamFee.Created, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            },
            Snackbar,
            () => T.ExamFee.UnreachableTryAgain,
            busy => _busy = busy);
    }

    private void CancelAsync()
        => MudDialog.Cancel();

    private sealed class ExaminationFeeFormModel
    {
        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; } = "Radiologist";

        public decimal Amount { get; set; }

        public bool IsPercentage { get; set; }
    }
}
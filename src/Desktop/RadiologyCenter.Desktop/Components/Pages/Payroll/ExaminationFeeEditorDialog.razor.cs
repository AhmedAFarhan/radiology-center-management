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

public partial class ExaminationFeeEditorDialog : EditorDialogBase
{
    [Parameter] public ExaminationFeeDto? Fee { get; set; }

    private IReadOnlyList<EnumOptionDto> _roleOptions = Array.Empty<EnumOptionDto>();

    private readonly ExaminationFeeFormModel _model = new();
    private EditContext _editContext = default!;
    private ExaminationTypeDto? _selectedType;
    private string _examTypeId = string.Empty;
    private string _examTypeName = string.Empty;

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

        await SafeExecute.RunAsync(async () =>
            _roleOptions = await EnumOptionsService.GetOptionsAsync("ExamFeeRole"),
            Snackbar,
            () => T.ExamFee.UnreachableTryAgain);

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

        var input = new ExaminationFeeInput
        {
            ExaminationTypeId = IsEdit ? _examTypeId : _selectedType!.Id,
            Role = _model.Role,
            Amount = _model.Amount,
            IsPercentage = _model.IsPercentage,
        };

        if (await TrySaveAsync(
                () => IsEdit
                    ? PayrollService.UpdateExaminationFeeAsync(Fee!.Id, input)
                    : PayrollService.CreateExaminationFeeAsync(input),
                () => T.ExamFee.UnreachableTryAgain))
        {
            Snackbar.Add(IsEdit ? T.ExamFee.Updated : T.ExamFee.Created, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

    private sealed class ExaminationFeeFormModel
    {
        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; } = "Radiologist";

        public decimal Amount { get; set; }

        public bool IsPercentage { get; set; }
    }
}
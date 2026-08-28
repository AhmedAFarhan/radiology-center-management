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
using RadiologyCenter.Desktop.Features.Payroll.Models;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Features.Payroll.Components;

public partial class ReferralFeeEditorDialog : EditorDialogBase
{
    [Parameter] public ReferralFeeDto? Fee { get; set; }

    private readonly ReferralFeeFormModel _model = new();
    private EditContext _editContext = default!;
    private ReferralDoctorDto? _selectedDoctor;
    private ExaminationTypeDto? _selectedType;
    private string _doctorId = string.Empty;
    private string _examTypeId = string.Empty;
    private string _doctorName = string.Empty;
    private string _examTypeName = string.Empty;

    private bool IsEdit => Fee is not null;

    private async Task<IEnumerable<ReferralDoctorDto>> SearchDoctorsAsync(string? value, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<ReferralDoctorDto>();

        try
        {
            var page = await ResourceService.GetReferralDoctorsPagedAsync(value, "LastName", false, 1, 20, ct);
            return page.Items;
        }
        catch (Exception)
        {
            return Array.Empty<ReferralDoctorDto>();
        }
    }

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

        _doctorId = Fee.ReferralDoctorId;
        _examTypeId = Fee.ExaminationTypeId;
        _model.Amount = Fee.Amount;
        _model.IsPercentage = Fee.IsPercentage;

        try
        {
            var doctorTask = ResourceService.GetReferralDoctorByIdAsync(Fee.ReferralDoctorId);
            var typeTask = ExaminationService.GetTypeByIdAsync(Fee.ExaminationTypeId);

            _doctorName = (await doctorTask).FullName;
            var type = await typeTask;
            _examTypeName = $"{type.Code} - {type.Name}";
        }
        catch (Exception)
        {
            _doctorName = Fee.ReferralDoctorId;
            _examTypeName = Fee.ExaminationTypeId;
        }
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        if (!IsEdit && _selectedDoctor is null)
        {
            Snackbar.Add(T.ReferralFee.SelectDoctor, Severity.Warning);
            return;
        }

        if (!IsEdit && _selectedType is null)
        {
            Snackbar.Add(T.ReferralFee.SelectExamType, Severity.Warning);
            return;
        }

        if (_model.IsPercentage && _model.Amount > 100)
        {
            Snackbar.Add(T.ReferralFee.PercentageLimit, Severity.Warning);
            return;
        }

        var input = new ReferralFeeInput
        {
            ReferralDoctorId = IsEdit ? _doctorId : _selectedDoctor!.Id,
            ExaminationTypeId = IsEdit ? _examTypeId : _selectedType!.Id,
            Amount = _model.Amount,
            IsPercentage = _model.IsPercentage,
        };

        if (await TrySaveAsync(
                () => IsEdit
                    ? PayrollService.UpdateReferralFeeAsync(Fee!.Id, input)
                    : PayrollService.CreateReferralFeeAsync(input),
                () => T.ReferralFee.UnreachableTryAgain))
        {
            Snackbar.Add(IsEdit ? T.ReferralFee.Updated : T.ReferralFee.Created, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

}

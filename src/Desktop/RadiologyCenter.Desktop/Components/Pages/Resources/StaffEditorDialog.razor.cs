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

namespace RadiologyCenter.Desktop.Components.Pages.Resources;

public partial class StaffEditorDialog : EditorDialogBase
{
    [Parameter] public StaffDto? Staff { get; set; }

    private IReadOnlyList<EnumOptionDto> _positionOptions = Array.Empty<EnumOptionDto>();

    private readonly StaffFormModel _model = new();
    private EditContext _editContext = default!;
    private UserDto? _selectedUser;

    private bool IsEdit => Staff is not null;

    private async Task<IEnumerable<UserDto>> SearchUsersAsync(string? value, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<UserDto>();

        try
        {
            var page = await IdentityService.GetUsersPagedAsync(value, "LastName", false, 1, 20, ct);
            return page.Items;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            return Array.Empty<UserDto>();
        }
        catch (Exception)
        {
            Snackbar.Add(T.StaffDialog.SearchError, Severity.Error);
            return Array.Empty<UserDto>();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_model);

        await SafeExecute.RunAsync(async () =>
            _positionOptions = await EnumOptionsService.GetOptionsAsync("StaffPosition"),
            Snackbar,
            () => T.StaffDialog.Unreachable);

        if (Staff is null)
            return;

        _model.FullName = Staff.FullName;
        _model.PhoneNumber = Staff.PhoneNumber;
        _model.Position = Staff.PositionKey;
        _model.HireDate = Staff.HireDate;
        _model.Department = Staff.Department;
        _model.Specialization = Staff.Specialization;
        _model.LicenseNumber = Staff.LicenseNumber;

        try
        {
            _selectedUser = await IdentityService.GetUserByIdAsync(Staff.UserId);
        }
        catch (Exception)
        {
            _selectedUser = new UserDto(
                Staff.UserId,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                null,
                true,
                false,
                false,
                false,
                null,
                null,
                default);
        }
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        if (_selectedUser is null)
        {
            Snackbar.Add(T.StaffDialog.SelectUser, Severity.Warning);
            return;
        }

        var input = new StaffInput
        {
            UserId = _selectedUser.Id,
            FullName = _model.FullName,
            PhoneNumber = _model.PhoneNumber,
            Position = _model.Position,
            HireDate = _model.HireDate ?? DateTime.Today,
            Department = _model.Department,
            Specialization = _model.Specialization,
            LicenseNumber = _model.LicenseNumber,
        };

        if (await TrySaveAsync(
                () => IsEdit
                    ? ResourceService.UpdateStaffAsync(Staff!.Id, input)
                    : ResourceService.CreateStaffAsync(input),
                () => T.StaffDialog.Unreachable))
        {
            Snackbar.Add(IsEdit ? T.StaffDialog.Updated : T.StaffDialog.Created, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

    private sealed class StaffFormModel : IValidatableObject
    {
        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(300, ErrorMessage = "Full name must be 300 characters or fewer.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Position is required.")]
        public string Position { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [MaxLength(30, ErrorMessage = "Phone number must be 30 characters or fewer.")]
        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime? HireDate { get; set; }

        [MaxLength(200, ErrorMessage = "Department must be 200 characters or fewer.")]
        public string? Department { get; set; }

        [MaxLength(200, ErrorMessage = "Specialization must be 200 characters or fewer.")]
        public string? Specialization { get; set; }

        [MaxLength(100, ErrorMessage = "License number must be 100 characters or fewer.")]
        public string? LicenseNumber { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Length < 2)
                yield return new ValidationResult("Full name must include at least a first and last name.", new[] { nameof(FullName) });

            if (HireDate is null)
                yield return new ValidationResult("Hire date is required.", new[] { nameof(HireDate) });

            if (!string.IsNullOrWhiteSpace(PhoneNumber) && !EgyptianPhoneNumber.IsValid(PhoneNumber))
                yield return new ValidationResult("Phone number must be a valid Egyptian number (e.g. 01012345678).", new[] { nameof(PhoneNumber) });
        }
    }
}
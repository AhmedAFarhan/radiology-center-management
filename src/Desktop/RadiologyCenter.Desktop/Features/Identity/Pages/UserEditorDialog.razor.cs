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

namespace RadiologyCenter.Desktop.Features.Identity.Pages;

public partial class UserEditorDialog : EditorDialogBase
{
[Parameter] public UserDto? User { get; set; }

    private readonly UserFormModel _model = new();
    private EditContext _editContext = default!;
    private List<RoleDto> _roles = new();
    private bool _showPassword;

    private bool IsEdit => User is not null;

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_model);

        await SafeExecute.RunAsync(async () =>
            {
                var roles = await IdentityService.GetRolesPagedAsync(null, "Name", false, 1, 100);
                _roles = roles.Items.ToList();
            },
            Snackbar,
            () => T.UserDialog.LoadRolesError);

        if (User is null)
            return;

        _model.UserName = User.UserName;
        _model.Email = User.Email;
        _model.Password = "placeholder";
        _model.FirstName = User.FirstName;
        _model.LastName = User.LastName;
        _model.PhoneNumber = User.PhoneNumber;
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        await TrySaveAsync(async () =>
            {
                if (IsEdit)
                {
                    await IdentityService.UpdateUserProfileAsync(User!.Id, new UpdateUserProfileInput
                    {
                        FirstName = _model.FirstName,
                        LastName = _model.LastName,
                        PhoneNumber = _model.PhoneNumber,
                    });
                    Snackbar.Add(T.UserDialog.ProfileUpdated, Severity.Success);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(_model.Password))
                    {
                        Snackbar.Add(T.UserDialog.PasswordRequired, Severity.Warning);
                        return;
                    }

                    if (!_model.SelectedRoleIds.Any())
                    {
                        Snackbar.Add(T.UserDialog.SelectRole, Severity.Warning);
                        return;
                    }

                    await IdentityService.CreateUserAsync(new CreateUserInput
                    {
                        UserName = _model.UserName,
                        Email = _model.Email,
                        FirstName = _model.FirstName,
                        LastName = _model.LastName,
                        PhoneNumber = _model.PhoneNumber ?? string.Empty,
                        Password = _model.Password,
                        RoleIds = _model.SelectedRoleIds.ToList(),
                    });
                    Snackbar.Add(T.UserDialog.Created, Severity.Success);
                }

                MudDialog.Close(DialogResult.Ok(true));
            },
            () => T.UserDialog.UnreachableRetry);
    }

    private sealed class UserFormModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [MaxLength(256, ErrorMessage = "Username must be 256 characters or fewer.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(200, ErrorMessage = "First name must be 200 characters or fewer.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(200, ErrorMessage = "Last name must be 200 characters or fewer.")]
        public string LastName { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{8,}$", ErrorMessage = "Password must contain both letters and digits.")]
        public string Password { get; set; } = string.Empty;

        public IReadOnlyCollection<string> SelectedRoleIds { get; set; } = new List<string>();
    }
}

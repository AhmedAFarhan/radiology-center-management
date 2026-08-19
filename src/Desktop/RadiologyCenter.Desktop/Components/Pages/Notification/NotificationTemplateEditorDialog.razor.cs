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

namespace RadiologyCenter.Desktop.Components.Pages.Notification;

public partial class NotificationTemplateEditorDialog : ComponentBase
{
[Parameter] public NotificationTemplateDto? Template { get; set; }

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private readonly NotificationTemplateFormModel _model = new();
    private EditContext _editContext = default!;
    private bool _busy;
    private bool _isEdit;

    protected override void OnInitialized()
    {
        _editContext = new EditContext(_model);
        _isEdit = Template is not null;

        if (Template is null)
            return;

        _model.Code = Template.Code;
        _model.Name = Template.Name;
        _model.Subject = Template.Subject;
        _model.Body = Template.Body;
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        await SafeExecute.RunAsync(async () =>
            {
                var input = new NotificationTemplateInput
                {
                    Code = _model.Code.Trim(),
                    Name = _model.Name.Trim(),
                    Subject = (_model.Subject ?? string.Empty).Trim(),
                    Body = _model.Body.Trim(),
                };

                if (_isEdit)
                    await NotificationService.UpdateTemplateAsync(Template!.Id, input);
                else
                    await NotificationService.CreateTemplateAsync(input);

                Snackbar.Add(_isEdit ? T.TemplateDialog.Updated : T.TemplateDialog.Created, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            },
            Snackbar,
            () => T.TemplateDialog.Unreachable,
            busy => _busy = busy);
    }

    private void CancelAsync()
        => MudDialog.Cancel();

    private sealed class NotificationTemplateFormModel
    {
        [Required(ErrorMessage = "Code is required.")]
        [MaxLength(100, ErrorMessage = "Code must be 100 characters or fewer.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(200, ErrorMessage = "Name must be 200 characters or fewer.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(400, ErrorMessage = "Subject must be 400 characters or fewer.")]
        public string? Subject { get; set; }

        [Required(ErrorMessage = "Body is required.")]
        public string Body { get; set; } = string.Empty;
    }
}
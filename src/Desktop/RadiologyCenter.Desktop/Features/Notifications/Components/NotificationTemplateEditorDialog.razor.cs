using System.ComponentModel.DataAnnotations;

using RadiologyCenter.Desktop.Features.Notifications.Models;

namespace RadiologyCenter.Desktop.Features.Notifications.Components;

public partial class NotificationTemplateEditorDialog : EditorDialogBase
{
    [Parameter] public NotificationTemplateDto? Template { get; set; }

    private readonly NotificationTemplateFormModel _model = new();
    private EditContext _editContext = default!;
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

        var input = new NotificationTemplateInput
        {
            Code = _model.Code.Trim(),
            Name = _model.Name.Trim(),
            Subject = (_model.Subject ?? string.Empty).Trim(),
            Body = _model.Body.Trim(),
        };

        if (await TrySaveAsync(
                () => _isEdit
                    ? NotificationService.UpdateTemplateAsync(Template!.Id, input)
                    : NotificationService.CreateTemplateAsync(input),
                () => T.TemplateDialog.Unreachable))
        {
            Snackbar.Add(_isEdit ? T.TemplateDialog.Updated : T.TemplateDialog.Created, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

}

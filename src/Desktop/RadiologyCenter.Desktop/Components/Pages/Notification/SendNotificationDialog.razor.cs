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

public partial class SendNotificationDialog : ComponentBase
{
[CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private readonly SendNotificationFormModel _model = new();
    private EditContext _editContext = default!;
    private bool _busy;
    private IReadOnlyList<EnumOptionDto> _channelOptions = Array.Empty<EnumOptionDto>();
    private IReadOnlyList<NotificationTemplateDto> _templates = Array.Empty<NotificationTemplateDto>();
    private NotificationPreviewDto? _preview;

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_model);

        try
        {
            _channelOptions = await EnumOptionsService.GetOptionsAsync("NotificationChannel");
        }
        catch
        {
            // channel options fall back to an empty list; the select will render no items
        }

        try
        {
            var page = await NotificationService.GetTemplatesPagedAsync(null, "Name", false, 1, 100);
            _templates = page.Items;
        }
        catch
        {
            // template picker is optional; leave empty
        }
    }

    private Task OnChannelChangedAsync(string value)
    {
        _model.Channel = value;
        return Task.CompletedTask;
    }

    private async Task OnTemplateCodeChangedAsync(string value)
    {
        _model.TemplateCode = value;
        _preview = null;

        if (string.IsNullOrWhiteSpace(value))
            return;

        var template = _templates.FirstOrDefault(t => t.Code == value);
        if (template is null)
            return;

        _model.Subject = template.Subject;
        _model.Body = template.Body;
    }

    private async Task PreviewAsync()
    {
        if (!_editContext.Validate())
            return;

        await SafeExecute.RunAsync(async () =>
            {
                _preview = await NotificationService.PreviewAsync(BuildInput());
            },
            Snackbar,
            () => T.SendDialog.Unreachable,
            busy => _busy = busy);
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await NotificationService.SendAsync(BuildInput());
                Snackbar.Add(T.SendDialog.Sent, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            },
            Snackbar,
            () => T.SendDialog.UnreachableRetry,
            busy => _busy = busy);
    }

    private void CancelAsync()
        => MudDialog.Cancel();

    private SendNotificationInput BuildInput()
    {
        var input = new SendNotificationInput
        {
            Recipient = _model.Recipient.Trim(),
            Channel = _model.Channel,
            TemplateCode = string.IsNullOrWhiteSpace(_model.TemplateCode) ? null : _model.TemplateCode.Trim(),
            Subject = string.IsNullOrWhiteSpace(_model.Subject) ? null : _model.Subject.Trim(),
            Body = string.IsNullOrWhiteSpace(_model.Body) ? null : _model.Body.Trim(),
            ReferenceId = string.IsNullOrWhiteSpace(_model.ReferenceId) ? null : _model.ReferenceId.Trim(),
        };

        var placeholders = _model.PlaceholdersSplit();
        if (placeholders.Count > 0)
            input.Placeholders = placeholders;

        return input;
    }

    private sealed class SendNotificationFormModel
    {
        [Required(ErrorMessage = "Recipient is required.")]
        [MaxLength(500, ErrorMessage = "Recipient must be 500 characters or fewer.")]
        public string Recipient { get; set; } = string.Empty;

        [Required(ErrorMessage = "Channel is required.")]
        public string Channel { get; set; } = "Sms";

        public string? TemplateCode { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string? ReferenceId { get; set; }
        public string? Placeholders { get; set; }

        public Dictionary<string, string> PlaceholdersSplit()
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(Placeholders))
                return result;

            foreach (var pair in Placeholders.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var eq = pair.IndexOf('=');
                if (eq <= 0)
                    continue;

                result[pair[..eq].Trim()] = pair[(eq + 1)..].Trim();
            }

            return result;
        }
    }
}
using Microsoft.AspNetCore.Components;
using MudBlazor;
using RadiologyCenter.Desktop.Localization;
using RadiologyCenter.Desktop.Services.Updates;

namespace RadiologyCenter.Desktop.Shared.Components;

public partial class ClinicUpdateDialog
{
    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;
    [Inject] public IAppUpdates Updates { get; set; } = default!;
    [Inject] public AppLocalizer T { get; set; } = default!;
    private readonly CancellationTokenSource _lifetime = new();
    private CancellationTokenSource? _download;
    private UpdateOffer? _update;
    private bool _busy, _downloading, _restarting, _checked, _disposed;
    private int _progress;
    private string? _error;
    private string L(string english, string arabic) => T.IsRTL ? arabic : english;
    protected override Task OnInitializedAsync() => CheckAsync();

    private async Task CheckAsync()
    {
        if (_busy) return;
        _busy = true; _error = null; _update = null;
        try { _update = await Updates.CheckAsync(_lifetime.Token); _checked = true; }
        catch (OperationCanceledException) when (_lifetime.IsCancellationRequested) { }
        catch (Exception ex) { _error = ErrorMessage(ex); }
        finally { _busy = false; }
    }

    private async Task InstallAsync()
    {
        if (_update is null || _busy) return;
        _busy = true; _error = null;
        try
        {
            if (!Updates.IsDownloaded(_update))
            {
                _downloading = true; _progress = 0;
                _download = CancellationTokenSource.CreateLinkedTokenSource(_lifetime.Token);
                await Updates.DownloadAsync(_update, value =>
                {
                    if (!_disposed) _ = InvokeAsync(() => { _progress = Math.Clamp(value, 0, 100); StateHasChanged(); });
                }, _download.Token);
            }
            else
            {
                _restarting = true;
                await Updates.RestartAsync(_update, _lifetime.Token);
            }
        }
        catch (OperationCanceledException)
        { if (!_disposed) _error = L("Download cancelled. You can try again when ready.", "تم إلغاء التنزيل. يمكنك المحاولة مجددًا عندما تكون مستعدًا."); }
        catch (Exception ex)
        {
            _error = _restarting
                ? L("The update could not restart EGcare. Fully exit from the tray and reopen the app before continuing.", "تعذرت إعادة تشغيل EGcare للتحديث. اخرج بالكامل من أيقونة التطبيق بجوار الساعة ثم افتحه مجددًا قبل المتابعة.")
                : ErrorMessage(ex);
        }
        finally
        {
            _download?.Dispose(); _download = null;
            _busy = _downloading = _restarting = false;
        }
    }

    private string ErrorMessage(Exception exception) => exception is UpdateFailure failure ? failure.Reason switch
    {
        UpdateFailureReason.Configuration => L("The update source is invalid. Ask the clinic administrator to check the connection settings.", "مصدر التحديثات غير صالح. اطلب من مسؤول العيادة مراجعة إعدادات الاتصال."),
        UpdateFailureReason.NotInstalled => L("Install EGcare using its setup file to enable updates. Visual Studio and portable builds cannot update themselves.", "ثبّت EGcare باستخدام ملف التثبيت لتفعيل التحديثات. نسخ Visual Studio والنسخ المحمولة لا تدعم التحديث الذاتي."),
        UpdateFailureReason.SourceChanged => L("The update source or version changed. Check again before continuing.", "تغير مصدر التحديث أو الإصدار. تحقق مجددًا قبل المتابعة."),
        _ => L("Download the update before installing it.", "نزّل التحديث قبل تثبيته.")
    } : L("The update could not be completed. Check your internet or clinic-share connection, access permissions and free disk space, then retry.", "تعذر إكمال التحديث. تحقق من الإنترنت أو مجلد العيادة والصلاحيات والمساحة المتاحة، ثم حاول مجددًا.");

    private void CancelDownload() => _download?.Cancel();
    private void Close() => MudDialog.Cancel();
    public void Dispose() { _disposed = true; _lifetime.Cancel(); _lifetime.Dispose(); }
}

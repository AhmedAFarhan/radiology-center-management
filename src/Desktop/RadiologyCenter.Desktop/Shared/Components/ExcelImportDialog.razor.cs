using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Shared.Components;

public partial class ExcelImportDialog : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Inject] private AppLocalizer T { get; set; } = default!;

    [Parameter] public Func<Task<byte[]>> DownloadTemplate { get; set; } = default!;
    [Parameter] public Func<string, Stream, Task<ExcelImportResultDto>> ImportFile { get; set; } = default!;
    [Parameter] public EventCallback Imported { get; set; }

    private IBrowserFile? _file;
    private ExcelImportResultDto? _result;
    private bool _busy;

    private void OnFileSelected(InputFileChangeEventArgs e)
    {
        _file = e.File;
        _result = null;
    }

    private async Task DownloadTemplateAsync()
    {
        await SafeExecute.RunAsync(
            async () =>
            {
                var content = await DownloadTemplate();
                var path = await FileSaveHelper.SaveAsync(content, "import-template.xlsx");
                Snackbar.Add(T.FormatValue(T.Common.SavedTo, path), Severity.Success);
            },
            Snackbar,
            () => T.Common.ImportFailed);
    }

    private async Task ImportAsync()
    {
        if (_file is null)
            return;

        _busy = true;
        try
        {
            await using var stream = _file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
            _result = await ImportFile(_file.Name, stream);

            if (_result.Errors.Count == 0)
            {
                Snackbar.Add(T.FormatValue(T.Common.ImportSummary, _result.ImportedCount, _result.TotalRows), Severity.Success);
                await Imported.InvokeAsync();
                MudDialog.Close();
            }
            else
            {
                Snackbar.Add(T.Common.ImportPartial, Severity.Warning);
                await Imported.InvokeAsync();
                _file = null;
            }
        }
        catch (ApiException ex)
        {
            Snackbar.Add(SafeExecute.FormatError(ex), Severity.Error);
        }
        catch (Exception)
        {
            Snackbar.Add(T.Common.ImportFailed, Severity.Error);
        }
        finally
        {
            _busy = false;
        }
    }

    private void Cancel() => MudDialog.Cancel();
}


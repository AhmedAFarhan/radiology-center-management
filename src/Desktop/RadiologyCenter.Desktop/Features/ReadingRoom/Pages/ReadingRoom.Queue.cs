using Microsoft.AspNetCore.Components;
using RadiologyCenter.Desktop.Features.ReadingRoom.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Features.ReadingRoom.Pages;

public partial class ReadingRoom
{
    private IEnumerable<QueueExam> Queue => _queue;
    private int PendingCount => _pendingTotal;
    private int ImagedCount => _imagedTotal;
    private bool IsSelected(QueueExam exam) => _selected?.Id == exam.Id;
    private int _queueRequestVersion;
    private async Task LoadQueueAsync()
    {
        var version=++_queueRequestVersion;
        _loadingQueue=true; _queueError=null;
        try
        {
            var result=await Api.PostAsync<ReadingQueuePage<QueueExam>>("api/examinations/reading-queue",
                new { pageNumber=_queuePage,pageSize=50,search=_filter,includeFinalized=_includeFinalized });
            if(version!=_queueRequestVersion) return;
            _queueTotal=result.TotalCount; _pendingTotal=result.PendingCount; _imagedTotal=result.ImagedCount;
            var lastPage=Math.Max(1,(_queueTotal+49)/50);
            if(_queuePage>lastPage) { _queuePage=lastPage; await LoadQueueAsync(); return; }
            _queue.Clear(); _queue.AddRange(result.Items);
        }
        catch(Exception) { if(version==_queueRequestVersion) _queueError=T.ReadingRoom.Unreachable; }
        finally { if(version==_queueRequestVersion) _loadingQueue=false; }
        StateHasChanged();
    }
    private async Task OnFilterChanged(string? value) { _filter=value; _queuePage=1; await LoadQueueAsync(); }
    private async Task IncludeFinalizedChangedAsync(bool value) { _includeFinalized=value; _queuePage=1; await LoadQueueAsync(); }
    private async Task PreviousQueuePageAsync() { if(_queuePage>1) { _queuePage--; await LoadQueueAsync(); } }
    private async Task NextQueuePageAsync() { if(_queuePage*50<_queueTotal) { _queuePage++; await LoadQueueAsync(); } }

    private bool IsLinking(QueueExam exam) => _linking.Contains(exam.Id);

    private async Task LinkImagesAsync(QueueExam item)
    {
        if (IsLinking(item) || item.StudyInstanceUid is not null)
            return;

        _linking.Add(item.Id);
        StateHasChanged();
        try
        {
            await SafeExecute.RunAsync(
                async () =>
                {
                    var current = await ExaminationService.GetByIdAsync(item.Id);
                    var linkedUid = await PacsSync.LinkStudyToExamAsync(
                        item.Id,
                        item.PatientCode,
                        accessionNumber: current.AccessionNumber,
                        item.PatientName);

                    if (linkedUid is null)
                    {
                        Snackbar.Add(T.ReadingRoom.NoMatchingStudy, Severity.Warning);
                        return;
                    }

                    await LoadQueueAsync();
                    var refreshed = _queue.FirstOrDefault(q => q.Id == item.Id);
                    if (refreshed is not null)
                        await SelectExamAsync(refreshed);
                    Snackbar.Add(T.ReadingRoom.ImagesLinked, Severity.Success);
                },
                Snackbar,
                () => T.ReadingRoom.Unreachable,
                busy => { });
        }
        finally
        {
            _linking.Remove(item.Id);
        }
    }
}

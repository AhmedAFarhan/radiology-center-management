using Microsoft.AspNetCore.Components;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Features.ReadingRoom.Pages;

public partial class ReadingRoom
{
    private IEnumerable<QueueExam> Queue
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_filter))
                return _queue;

            var f = _filter.Trim();
            return _queue.Where(q =>
                q.PatientName.Contains(f, StringComparison.OrdinalIgnoreCase) ||
                q.ExamName.Contains(f, StringComparison.OrdinalIgnoreCase) ||
                q.PatientCode.Contains(f, StringComparison.OrdinalIgnoreCase) ||
                q.Priority.Contains(f, StringComparison.OrdinalIgnoreCase) ||
                q.ReportStatus.Contains(f, StringComparison.OrdinalIgnoreCase));
        }
    }

    private int PendingCount => _queue.Count(q => q.ReportStatusKey is "New" or "Draft");

    private int ImagedCount => _queue.Count(q => q.StudyInstanceUid is not null);

    private bool IsSelected(QueueExam exam) => _selected?.Id == exam.Id;

    private async Task LoadQueueAsync()
    {
        _loadingQueue = true;
        _queueError = null;
        try
        {
            var exams = await ExaminationService.GetPagedAsync(null, null, false, 1, 100);
            var visible = exams.Items
                .Where(e => e.StatusKey is "CheckedIn")
                .ToList();

            IReadOnlyList<ReportListItemDto> reportItems = Array.Empty<ReportListItemDto>();
            try
            {
                var reports = await ReportService.GetPagedAsync(null, null, false, 1, 100);
                reportItems = reports.Items;
            }
            catch (Exception)
            {
                // report status badges are best-effort
            }

            _reportStatusByExam.Clear();
            _reportStatusKeyByExam.Clear();
            foreach (var report in reportItems)
            {
                _reportStatusByExam[report.ExaminationId] = report.Status;
                _reportStatusKeyByExam[report.ExaminationId] = report.StatusKey;
            }

            _queue.Clear();
            foreach (var exam in visible)
            {
                var patient = await GetPatientAsync(exam.PatientId);
                var patientCode = patient?.PatientCode;
                _queue.Add(new QueueExam
                {
                    Id = exam.Id,
                    PatientId = exam.PatientId,
                    PatientName = patient?.FullName ?? T.ReadingRoom.UnknownPatient,
                    PatientCode = patientCode ?? "-",
                    ExamName = exam.ExaminationTypeName ?? T.Common.Examination,
                    ExaminationTypeId = exam.ExaminationTypeId,
                    StatusKey = exam.StatusKey,
                    ScheduledAt = exam.ScheduledAt,
                    CompletedAt = exam.CompletedAt,
                    Priority = exam.Priority,
                    PriorityKey = exam.PriorityKey,
                    Indication = exam.ClinicalIndication,
                    AssignedRadiologistId = exam.RadiologistId,
                    AssignedTechnicianId = exam.TechnicianId,
                    ReportStatus = _reportStatusByExam.GetValueOrDefault(exam.Id, "New"),
                    ReportStatusKey = _reportStatusKeyByExam.GetValueOrDefault(exam.Id, "New"),
                    StudyInstanceUid = string.IsNullOrEmpty(exam.StudyInstanceUID) ? null : exam.StudyInstanceUID,
                });
            }

            _queue.Sort((a, b) =>
                (a.ScheduledAt ?? DateTime.MaxValue).CompareTo(b.ScheduledAt ?? DateTime.MaxValue));
        }
        catch (Exception)
        {
            _queueError = T.ReadingRoom.Unreachable;
        }
        finally
        {
            _loadingQueue = false;
        }

        StateHasChanged();
    }

    private async Task<PatientDto?> GetPatientAsync(string patientId)
    {
        if (_patientCache.TryGetValue(patientId, out var cached))
            return cached;
        try
        {
            var patient = await PatientService.GetByIdAsync(patientId);
            _patientCache[patientId] = patient;
            return patient;
        }
        catch
        {
            return null;
        }
    }

    private void OnFilterChanged(string? value)
        => _filter = value;

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
                    var linkedUid = await PacsSync.LinkStudyToExamAsync(
                        item.Id,
                        item.PatientCode,
                        accessionNumber: null,
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

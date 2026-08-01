using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.Examinations.Domain.Enumerations;
using RadiologyCenter.Examinations.Domain.Events;

namespace RadiologyCenter.Examinations.Domain.Entities;

public sealed class Visit : SoftDeletableAggregateRoot<Guid>
{
    private readonly List<Examination> _examinations = [];

    public Guid PatientId { get; private set; }
    public Guid? AppointmentId { get; private set; }
    public DateTime VisitedAt { get; private set; }
    public VisitStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyCollection<Examination> Examinations => _examinations.AsReadOnly();

    private Visit()
    {
        Status = null!;
    }

    public static Visit Create(
        Guid patientId,
        DateTime? visitedAt = null,
        Guid? appointmentId = null,
        string? notes = null)
    {
        Guard.AgainstEmpty(patientId, nameof(patientId));

        var visit = new Visit
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            VisitedAt = visitedAt ?? DateTime.UtcNow,
            AppointmentId = appointmentId,
            Notes = notes?.Trim(),
            Status = VisitStatus.CheckedIn
        };

        visit.RaiseDomainEvent(new VisitCreatedEvent(visit.Id, visit.PatientId));
        return visit;
    }

    public Examination AddExamination(
        Guid examinationTypeId,
        string referringDoctor,
        string clinicalIndication,
        ExaminationPriority priority,
        string? notes = null)
    {
        EnsureCheckedIn();

        var examination = Examination.Create(
            Id,
            examinationTypeId,
            referringDoctor,
            clinicalIndication,
            priority,
            notes);

        _examinations.Add(examination);
        return examination;
    }

    public void ScheduleExamination(Guid examinationId, DateTime scheduledAt)
    {
        EnsureCheckedIn();
        GetExamination(examinationId).Schedule(scheduledAt);
    }

    public void CheckInExamination(Guid examinationId)
    {
        EnsureCheckedIn();
        GetExamination(examinationId).CheckIn();
    }

    public void StartExamination(Guid examinationId, Guid performedByUserId)
    {
        EnsureCheckedIn();
        GetExamination(examinationId).Start(performedByUserId);
    }

    public void CompleteExamination(Guid examinationId)
    {
        EnsureCheckedIn();
        GetExamination(examinationId).Complete();
        CompleteIfAllExaminationsDone();
    }

    public void CancelExamination(Guid examinationId, string? reason = null)
    {
        EnsureCheckedIn();
        GetExamination(examinationId).Cancel(reason);
        CompleteIfAllExaminationsDone();
    }

    public ExaminationItem AddExaminationItem(
        Guid examinationId,
        Guid itemId,
        int quantity,
        bool isContrast = false,
        bool isRequired = false,
        string? notes = null)
    {
        EnsureCheckedIn();
        return GetExamination(examinationId).AddItem(itemId, quantity, isContrast, isRequired, notes);
    }

    public void UpdateExaminationItem(
        Guid examinationId,
        Guid examinationItemId,
        int quantity,
        bool isContrast,
        bool isRequired,
        string? notes = null)
    {
        EnsureCheckedIn();
        GetExamination(examinationId).UpdateItem(examinationItemId, quantity, isContrast, isRequired, notes);
    }

    public void RemoveExaminationItem(Guid examinationId, Guid examinationItemId)
    {
        EnsureCheckedIn();
        GetExamination(examinationId).RemoveItem(examinationItemId);
    }

    public void CancelVisit(string? reason = null)
    {
        if (Status != VisitStatus.CheckedIn)
            throw new DomainException($"Visit '{Id}' in status '{Status}' cannot be cancelled.");

        if (_examinations.Any(e => e.Status == ExaminationStatus.InProgress || e.Status == ExaminationStatus.Completed))
            throw new DomainException("A visit cannot be cancelled while an examination is in progress or completed.");

        foreach (var examination in _examinations.Where(e => !e.IsTerminal))
            examination.Cancel(reason);

        Status = VisitStatus.Cancelled;
        RaiseDomainEvent(new VisitCancelledEvent(Id));
    }

    private void CompleteIfAllExaminationsDone()
    {
        if (Status != VisitStatus.CheckedIn) return;
        if (_examinations.Count == 0 || _examinations.Any(e => !e.IsTerminal)) return;

        Status = VisitStatus.Completed;
        RaiseDomainEvent(new VisitCompletedEvent(Id));
    }

    private Examination GetExamination(Guid examinationId)
    {
        return _examinations.FirstOrDefault(e => e.Id == examinationId)
            ?? throw new DomainException($"Examination '{examinationId}' is not on visit '{Id}'.");
    }

    private void EnsureCheckedIn()
    {
        if (Status != VisitStatus.CheckedIn)
            throw new DomainException($"Visit '{Id}' is no longer open (status: '{Status}').");
    }
}

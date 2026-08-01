using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.Examinations.Domain.Enumerations;
using RadiologyCenter.Examinations.Domain.Events;

namespace RadiologyCenter.Examinations.Domain.Entities;

public sealed class Examination : SoftDeletableAggregateRoot<Guid>
{
    private readonly List<ExaminationItem> _items = [];

    public Guid VisitId { get; private set; }
    public Guid ExaminationTypeId { get; private set; }
    public string ReferringDoctor { get; private set; }
    public string ClinicalIndication { get; private set; }
    public ExaminationPriority Priority { get; private set; }
    public ExaminationStatus Status { get; private set; }
    public DateTime? ScheduledAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public Guid? PerformedByUserId { get; private set; }
    public string? Notes { get; private set; }
    public string? CancellationReason { get; private set; }

    public IReadOnlyCollection<ExaminationItem> Items => _items.AsReadOnly();

    private Examination()
    {
        ReferringDoctor = null!;
        ClinicalIndication = null!;
        Priority = null!;
        Status = null!;
    }

    public static Examination Create(
        Guid visitId,
        Guid examinationTypeId,
        string referringDoctor,
        string clinicalIndication,
        ExaminationPriority priority,
        string? notes = null)
    {
        Guard.AgainstEmpty(visitId, nameof(visitId));
        Guard.AgainstEmpty(examinationTypeId, nameof(examinationTypeId));
        Guard.AgainstNullOrWhiteSpace(referringDoctor, nameof(referringDoctor));
        Guard.AgainstNullOrWhiteSpace(clinicalIndication, nameof(clinicalIndication));
        Guard.AgainstNull(priority, nameof(priority));

        var examination = new Examination
        {
            Id = Guid.NewGuid(),
            VisitId = visitId,
            ExaminationTypeId = examinationTypeId,
            ReferringDoctor = referringDoctor.Trim(),
            ClinicalIndication = clinicalIndication.Trim(),
            Priority = priority,
            Status = ExaminationStatus.Requested,
            Notes = notes?.Trim()
        };

        examination.RaiseDomainEvent(new ExaminationCreatedEvent(examination.VisitId, examination.Id, examination.ExaminationTypeId));
        return examination;
    }

    public ExaminationItem AddItem(
        Guid itemId,
        int quantity,
        bool isContrast = false,
        bool isRequired = false,
        string? notes = null)
    {
        EnsureNotTerminal();
        Guard.Against(_items.Any(i => i.ItemId == itemId), isDuplicate => isDuplicate, $"Item '{itemId}' is already on examination '{Id}'.");

        var item = ExaminationItem.Create(Id, itemId, quantity, isContrast, isRequired, notes);
        _items.Add(item);
        return item;
    }

    public void UpdateItem(
        Guid examinationItemId,
        int quantity,
        bool isContrast,
        bool isRequired,
        string? notes = null)
    {
        EnsureNotTerminal();
        var item = GetItem(examinationItemId);
        item.Update(quantity, isContrast, isRequired, notes);
    }

    public void RemoveItem(Guid examinationItemId)
    {
        EnsureNotTerminal();
        var item = GetItem(examinationItemId);
        Guard.Against(item.IsRequired, isRequired => isRequired, $"Item '{item.ItemId}' is required for this examination and cannot be removed.");

        _items.Remove(item);
    }

    public void Schedule(DateTime scheduledAt)
    {
        EnsureStatus(ExaminationStatus.Requested);
        Guard.Against(scheduledAt, s => s == default, "Scheduled time cannot be the default value.");

        ScheduledAt = scheduledAt;
        Status = ExaminationStatus.Scheduled;
        RaiseDomainEvent(new ExaminationScheduledEvent(VisitId, Id, scheduledAt));
    }

    public void CheckIn()
    {
        EnsureStatus(ExaminationStatus.Requested, ExaminationStatus.Scheduled);

        Status = ExaminationStatus.CheckedIn;
        RaiseDomainEvent(new ExaminationCheckedInEvent(VisitId, Id));
    }

    public void Start(Guid performedByUserId)
    {
        EnsureStatus(ExaminationStatus.CheckedIn);
        Guard.AgainstEmpty(performedByUserId, nameof(performedByUserId));

        PerformedByUserId = performedByUserId;
        StartedAt = DateTime.UtcNow;
        Status = ExaminationStatus.InProgress;
        RaiseDomainEvent(new ExaminationStartedEvent(VisitId, Id, performedByUserId));
    }

    public void Complete()
    {
        EnsureStatus(ExaminationStatus.InProgress);

        CompletedAt = DateTime.UtcNow;
        Status = ExaminationStatus.Completed;
        RaiseDomainEvent(new ExaminationCompletedEvent(VisitId, Id));
    }

    public void Cancel(string? reason = null)
    {
        EnsureStatus(ExaminationStatus.Requested, ExaminationStatus.Scheduled, ExaminationStatus.CheckedIn);

        CancellationReason = reason?.Trim();
        Status = ExaminationStatus.Cancelled;
        RaiseDomainEvent(new ExaminationCancelledEvent(VisitId, Id));
    }

    public bool IsTerminal => Status == ExaminationStatus.Completed || Status == ExaminationStatus.Cancelled;

    private ExaminationItem GetItem(Guid examinationItemId)
    {
        return _items.FirstOrDefault(i => i.Id == examinationItemId)
            ?? throw new DomainException($"Item '{examinationItemId}' is not on examination '{Id}'.");
    }

    private void EnsureNotTerminal()
    {
        if (IsTerminal)
            throw new DomainException($"Examination '{Id}' is '{Status}' and its items can no longer be modified.");
    }

    private void EnsureStatus(params ExaminationStatus[] allowed)
    {
        if (!allowed.Contains(Status))
            throw new DomainException($"Examination '{Id}' cannot transition from status '{Status}'.");
    }
}

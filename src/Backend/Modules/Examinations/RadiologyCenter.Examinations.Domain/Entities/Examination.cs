using RadiologyCenter.BuildingBlocks.Domain.Auditing;
using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.Examinations.Domain.Common;
using RadiologyCenter.Examinations.Domain.Enumerations;
using RadiologyCenter.Examinations.Domain.Errors;
using RadiologyCenter.Examinations.Domain.Events;

namespace RadiologyCenter.Examinations.Domain.Entities;

public sealed class Examination : AuditableAggregateRoot<Guid>
{
    private readonly List<ExaminationItem> _items = [];

    public Guid PatientId { get; private set; }
    public Guid ExaminationTypeId { get; private set; }
    public Guid? ReferralDoctorId { get; private set; }
    public Guid RadiologistId { get; private set; }
    public Guid TechnicianId { get; private set; }
    public string ClinicalIndication { get; private set; }
    public ExaminationPriority Priority { get; private set; }
    public ExaminationStatus Status { get; private set; }
    public DateTime? ScheduledAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public Guid? PerformedByUserId { get; private set; }
    public string? Notes { get; private set; }
    public string? CancellationReason { get; private set; }
    public decimal Price { get; private set; }
    public decimal Discount { get; private set; }
    public bool IsDiscountPercentage { get; private set; }
    public decimal Paid { get; private set; }
    public decimal Remaining { get; private set; }
    public string? StudyInstanceUID { get; private set; }
    public string? AccessionNumber { get; private set; }
    public DateTime? ImagesReceivedAt { get; private set; }

    public IReadOnlyCollection<ExaminationItem> Items => _items.AsReadOnly();

    private Examination()
    {
        ClinicalIndication = null!;
        Priority = null!;
        Status = null!;
    }

    public static Examination Create(
        Guid patientId,
        Guid examinationTypeId,
        Guid radiologistId,
        Guid technicianId,
        string clinicalIndication,
        ExaminationPriority priority,
        decimal price,
        Guid? referralDoctorId = null,
        decimal discount = 0,
        bool isDiscountPercentage = false,
        decimal paid = 0,
        string? notes = null)
    {
        Guard.AgainstEmpty(patientId, nameof(patientId));
        Guard.AgainstEmpty(examinationTypeId, nameof(examinationTypeId));
        Guard.AgainstEmpty(radiologistId, nameof(radiologistId));
        Guard.AgainstEmpty(technicianId, nameof(technicianId));
        Guard.AgainstNullOrWhiteSpace(clinicalIndication, nameof(clinicalIndication));
        Guard.AgainstNull(priority, nameof(priority));
        Guard.Against(price, p => p < 0, DomainErrors.PriceNegative, "Price cannot be negative.");
        Guard.Against(discount, d => d < 0, DomainErrors.DiscountNegative, "Discount cannot be negative.");
        if (isDiscountPercentage)
            Guard.Against(discount, d => d > ExaminationPricing.PercentageCap, DomainErrors.PercentageDiscountMax, "Percentage discount cannot exceed 100.");
        Guard.Against(paid, p => p < 0, DomainErrors.PaidAmountNegative, "Paid amount cannot be negative.");

        var examination = new Examination
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            ExaminationTypeId = examinationTypeId,
            ReferralDoctorId = referralDoctorId,
            RadiologistId = radiologistId,
            TechnicianId = technicianId,
            ClinicalIndication = clinicalIndication.Trim(),
            Priority = priority,
            Status = ExaminationStatus.Requested,
            Price = price,
            Discount = discount,
            IsDiscountPercentage = isDiscountPercentage,
            Paid = paid,
            Remaining = 0,
            Notes = notes?.Trim()
        };
        examination.RecalculateRemaining();

        examination.RaiseDomainEvent(new ExaminationCreatedEvent(examination.Id, examination.ExaminationTypeId));
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
        Guard.Against(_items.Any(i => i.ItemId == itemId), isDuplicate => isDuplicate, DomainErrors.DuplicateItem, $"Item '{itemId}' is already on examination '{Id}'.");

        var item = ExaminationItem.Create(Id, itemId, quantity, isContrast, isRequired, notes);
        _items.Add(item);
        return item;
    }

    public void RemoveItem(Guid examinationItemId)
    {
        EnsureNotTerminal();
        var item = GetItem(examinationItemId);
        Guard.Against(item.IsRequired, isRequired => isRequired, DomainErrors.RequiredItemCannotRemove, $"Item '{item.ItemId}' is required for this examination and cannot be removed.");

        _items.Remove(item);
    }

    public void Update(
        Guid radiologistId,
        Guid technicianId,
        string clinicalIndication,
        ExaminationPriority priority,
        Guid? referralDoctorId = null,
        string? notes = null)
    {
        EnsureNotTerminal();
        Guard.AgainstEmpty(radiologistId, nameof(radiologistId));
        Guard.AgainstEmpty(technicianId, nameof(technicianId));
        Guard.AgainstNullOrWhiteSpace(clinicalIndication, nameof(clinicalIndication));
        Guard.AgainstNull(priority, nameof(priority));

        RadiologistId = radiologistId;
        TechnicianId = technicianId;
        ReferralDoctorId = referralDoctorId;
        ClinicalIndication = clinicalIndication.Trim();
        Priority = priority;
        Notes = notes?.Trim();
    }

    public void Schedule(DateTime scheduledAt)
    {
        EnsureStatus(ExaminationStatus.Requested);
        Guard.Against(scheduledAt, s => s == default, DomainErrors.ScheduledTimeDefault, "Scheduled time cannot be the default value.");
        Guard.Against(scheduledAt, s => s < DateTime.UtcNow.AddMinutes(-1), DomainErrors.ScheduledTimePast, "Scheduled time cannot be in the past.");

        ScheduledAt = scheduledAt;
        Status = ExaminationStatus.Scheduled;
        RaiseDomainEvent(new ExaminationScheduledEvent(Id, scheduledAt));
    }

    public void CheckIn()
    {
        EnsureStatus(ExaminationStatus.Requested, ExaminationStatus.Scheduled);

        Status = ExaminationStatus.CheckedIn;
        RaiseDomainEvent(new ExaminationCheckedInEvent(Id));
    }

    public void Start(Guid performedByUserId)
    {
        EnsureStatus(ExaminationStatus.CheckedIn);
        Guard.AgainstEmpty(performedByUserId, nameof(performedByUserId));

        PerformedByUserId = performedByUserId;
        StartedAt = DateTime.UtcNow;
        Status = ExaminationStatus.InProgress;
        RaiseDomainEvent(new ExaminationStartedEvent(Id, performedByUserId));
    }

    public void Complete()
    {
        EnsureStatus(ExaminationStatus.InProgress);

        CompletedAt = DateTime.UtcNow;
        Status = ExaminationStatus.Completed;
        RaiseDomainEvent(new ExaminationCompletedEvent(Id));
    }

    public void Cancel(string? reason = null)
    {
        EnsureStatus(ExaminationStatus.Requested, ExaminationStatus.Scheduled, ExaminationStatus.CheckedIn);

        CancellationReason = reason?.Trim();
        Status = ExaminationStatus.Cancelled;
        RaiseDomainEvent(new ExaminationCancelledEvent(Id));
    }

    public bool IsTerminal => Status == ExaminationStatus.Completed || Status == ExaminationStatus.Cancelled;

    public void RecordPacsImages(string? studyInstanceUID, string? accessionNumber)
    {
        if (!string.IsNullOrWhiteSpace(studyInstanceUID))
            StudyInstanceUID = studyInstanceUID.Trim();
        if (!string.IsNullOrWhiteSpace(accessionNumber))
            AccessionNumber = accessionNumber.Trim();
        if (!string.IsNullOrWhiteSpace(studyInstanceUID) || !string.IsNullOrWhiteSpace(accessionNumber))
            ImagesReceivedAt = DateTime.UtcNow;
    }

    public void SetBilling(decimal discount, bool isDiscountPercentage, decimal? paid = null)
    {
        Guard.Against(discount, d => d < 0, DomainErrors.DiscountNegative, "Discount cannot be negative.");
        if (isDiscountPercentage)
            Guard.Against(discount, d => d > ExaminationPricing.PercentageCap, DomainErrors.PercentageDiscountMax, "Percentage discount cannot exceed 100.");
        if (paid.HasValue)
        {
            Guard.Against(paid.Value, p => p < 0, DomainErrors.PaidAmountNegative, "Paid amount cannot be negative.");
            Guard.Against(
                paid.Value,
                p => Paid > 0 && p != Paid,
                DomainErrors.PaidAmountImmutable,
                "Paid amount cannot be modified once a payment has been recorded.");
        }

        Discount = discount;
        IsDiscountPercentage = isDiscountPercentage;
        if (paid.HasValue)
            Paid = paid.Value;
        RecalculateRemaining();
    }

    public void RecordPayment(decimal amount)
    {
        Guard.Against(amount, a => a < 0, DomainErrors.PaymentNegative, "Payment cannot be negative.");
        Guard.Against(amount, a => a > Remaining, DomainErrors.PaymentExceedsRemaining, $"Payment of '{amount}' exceeds the remaining balance of '{Remaining}'.");

        Paid += amount;
        RecalculateRemaining();
    }

    public void Refund(decimal amount)
    {
        Guard.Against(amount, a => a < 0, DomainErrors.RefundNegative, "Refund cannot be negative.");
        Guard.Against(amount, a => a > Paid, DomainErrors.RefundExceedsPaid, "Refund cannot exceed the amount paid.");

        Paid -= amount;
        RecalculateRemaining();
    }

    private void RecalculateRemaining()
    {
        var discountValue = ExaminationPricing.DiscountValue(Price, Discount, IsDiscountPercentage);
        Remaining = Price - discountValue - Paid;
        if (Remaining < 0) Remaining = 0;
    }

    private ExaminationItem GetItem(Guid examinationItemId)
    {
        return _items.FirstOrDefault(i => i.Id == examinationItemId)
            ?? throw new DomainException(DomainErrors.ItemNotOnExamination, $"Item '{examinationItemId}' is not on examination '{Id}'.");
    }

    private void EnsureNotTerminal()
    {
        if (IsTerminal)
            throw new BusinessRuleViolationException(nameof(EnsureNotTerminal), DomainErrors.ItemsCannotBeModified, $"Examination '{Id}' is '{Status}' and its items can no longer be modified.");
    }

    private void EnsureStatus(params ExaminationStatus[] allowed)
    {
        if (!allowed.Contains(Status))
            throw new BusinessRuleViolationException(nameof(EnsureStatus), DomainErrors.InvalidStatusTransition, $"Examination '{Id}' cannot transition from status '{Status}'.");
    }
}

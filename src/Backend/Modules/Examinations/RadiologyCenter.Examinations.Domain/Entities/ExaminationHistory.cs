using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Entities;
using RadiologyCenter.Examinations.Domain.Enumerations;
using RadiologyCenter.Examinations.Domain.ValueObjects;

namespace RadiologyCenter.Examinations.Domain.Entities;

public sealed class ExaminationHistory : Entity<Guid>
{
    private readonly List<ExaminationHistoryItem> _items = [];

    public Guid? ExaminationId { get; private set; }
    public Guid ExaminationTypeId { get; private set; }
    public string TypeCode { get; private set; }
    public string TypeName { get; private set; }
    public Modality TypeModality { get; private set; }
    public string TypeBodyPart { get; private set; }
    public decimal TypePrice { get; private set; }
    public int TypeStandardDurationMinutes { get; private set; }
    public Guid? ReferralDoctorId { get; private set; }
    public Guid RadiologistId { get; private set; }
    public Guid TechnicianId { get; private set; }
    public decimal? RadiologistFee { get; private set; }
    public decimal? TechnicianFee { get; private set; }
    public decimal? ReferralFee { get; private set; }
    public string ClinicalIndication { get; private set; }
    public ExaminationPriority Priority { get; private set; }
    public decimal Price { get; private set; }
    public decimal Discount { get; private set; }
    public bool IsDiscountPercentage { get; private set; }
    public decimal Paid { get; private set; }
    public decimal Remaining { get; private set; }
    public DateTime? ScheduledAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public Guid? PerformedByUserId { get; private set; }
    public string? Notes { get; private set; }
    public string? CancellationReason { get; private set; }

    public IReadOnlyCollection<ExaminationHistoryItem> Items => _items.AsReadOnly();

    private ExaminationHistory()
    {
        TypeCode = null!;
        TypeName = null!;
        TypeModality = null!;
        TypeBodyPart = null!;
        ClinicalIndication = null!;
        Priority = null!;
    }

    public static ExaminationHistory Create(
        Examination examination,
        ExaminationTypeSnapshot type,
        IReadOnlyDictionary<Guid, ItemSnapshot> itemSnapshots,
        decimal? radiologistFee = null,
        decimal? technicianFee = null,
        decimal? referralFee = null)
    {
        Guard.AgainstNull(examination, nameof(examination));
        Guard.AgainstNull(type, nameof(type));
        Guard.AgainstNull(itemSnapshots, nameof(itemSnapshots));
        Guard.Against(radiologistFee, f => f.HasValue && f.Value < 0, "Radiologist fee cannot be negative.");
        Guard.Against(technicianFee, f => f.HasValue && f.Value < 0, "Technician fee cannot be negative.");
        Guard.Against(referralFee, f => f.HasValue && f.Value < 0, "Referral fee cannot be negative.");

        var history = new ExaminationHistory
        {
            Id = Guid.NewGuid(),
            ExaminationId = examination.Id,
            ExaminationTypeId = examination.ExaminationTypeId,
            TypeCode = type.Code,
            TypeName = type.Name,
            TypeModality = type.Modality,
            TypeBodyPart = type.BodyPart,
            TypePrice = type.Price,
            TypeStandardDurationMinutes = type.StandardDurationMinutes,
            ReferralDoctorId = examination.ReferralDoctorId,
            RadiologistId = examination.RadiologistId,
            TechnicianId = examination.TechnicianId,
            RadiologistFee = radiologistFee,
            TechnicianFee = technicianFee,
            ReferralFee = referralFee,
            ClinicalIndication = examination.ClinicalIndication,
            Priority = examination.Priority,
            Price = examination.Price,
            Discount = examination.Discount,
            IsDiscountPercentage = examination.IsDiscountPercentage,
            Paid = examination.Paid,
            Remaining = examination.Remaining,
            ScheduledAt = examination.ScheduledAt,
            StartedAt = examination.StartedAt,
            CompletedAt = examination.CompletedAt,
            PerformedByUserId = examination.PerformedByUserId,
            Notes = examination.Notes,
            CancellationReason = examination.CancellationReason
        };

        foreach (var item in examination.Items)
        {
            if (!itemSnapshots.TryGetValue(item.ItemId, out var snapshot))
                continue;

            history._items.Add(ExaminationHistoryItem.Create(
                history.Id,
                item.ItemId,
                snapshot.Name,
                snapshot.CategoryValue,
                item.Quantity,
                snapshot.UnitCost,
                item.IsContrast,
                item.IsRequired,
                item.Notes));
        }

        return history;
    }

    public void UpdatePaymentSnapshot(decimal paid, decimal remaining)
    {
        Guard.Against(paid, p => p < 0, "Paid amount cannot be negative.");
        Guard.Against(remaining, r => r < 0, "Remaining amount cannot be negative.");

        Paid = paid;
        Remaining = remaining;
    }
}

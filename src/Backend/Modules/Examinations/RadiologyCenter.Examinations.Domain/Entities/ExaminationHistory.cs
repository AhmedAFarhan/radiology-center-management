using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Entities;
using RadiologyCenter.Examinations.Domain.Enumerations;
using RadiologyCenter.Examinations.Domain.ValueObjects;

namespace RadiologyCenter.Examinations.Domain.Entities;

public sealed class ExaminationHistory : Entity<Guid>
{
    private readonly List<ExaminationHistoryItem> _items = [];

    public Guid ExaminationId { get; private set; }
    public Guid VisitId { get; private set; }
    public Guid ExaminationTypeId { get; private set; }
    public string TypeCode { get; private set; }
    public string TypeName { get; private set; }
    public Modality TypeModality { get; private set; }
    public string TypeBodyPart { get; private set; }
    public decimal TypePrice { get; private set; }
    public int TypeStandardDurationMinutes { get; private set; }
    public string ReferringDoctor { get; private set; }
    public string ClinicalIndication { get; private set; }
    public ExaminationPriority Priority { get; private set; }
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
        ReferringDoctor = null!;
        ClinicalIndication = null!;
        Priority = null!;
    }

    public static ExaminationHistory Create(
        Examination examination,
        ExaminationTypeSnapshot type,
        IReadOnlyDictionary<Guid, ItemSnapshot> itemSnapshots)
    {
        Guard.AgainstNull(examination, nameof(examination));
        Guard.AgainstNull(type, nameof(type));
        Guard.AgainstNull(itemSnapshots, nameof(itemSnapshots));

        var history = new ExaminationHistory
        {
            Id = Guid.NewGuid(),
            ExaminationId = examination.Id,
            VisitId = examination.VisitId,
            ExaminationTypeId = examination.ExaminationTypeId,
            TypeCode = type.Code,
            TypeName = type.Name,
            TypeModality = type.Modality,
            TypeBodyPart = type.BodyPart,
            TypePrice = type.Price,
            TypeStandardDurationMinutes = type.StandardDurationMinutes,
            ReferringDoctor = examination.ReferringDoctor,
            ClinicalIndication = examination.ClinicalIndication,
            Priority = examination.Priority,
            ScheduledAt = examination.ScheduledAt,
            StartedAt = examination.StartedAt,
            CompletedAt = examination.CompletedAt,
            PerformedByUserId = examination.PerformedByUserId,
            Notes = examination.Notes,
            CancellationReason = examination.CancellationReason
        };

        foreach (var item in examination.Items)
        {
            var snapshot = itemSnapshots.GetValueOrDefault(item.ItemId);
            history._items.Add(ExaminationHistoryItem.Create(
                history.Id,
                item.ItemId,
                snapshot?.Name ?? string.Empty,
                snapshot?.CategoryValue ?? 0,
                item.Quantity,
                item.IsContrast,
                item.IsRequired,
                item.Notes));
        }

        return history;
    }
}

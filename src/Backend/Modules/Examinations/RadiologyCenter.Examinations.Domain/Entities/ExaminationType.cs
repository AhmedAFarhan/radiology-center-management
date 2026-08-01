using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.Examinations.Domain.Enumerations;
using RadiologyCenter.Examinations.Domain.Events;

namespace RadiologyCenter.Examinations.Domain.Entities;

public sealed class ExaminationType : SoftDeletableAggregateRoot<Guid>
{
    private readonly List<ExaminationTypeItem> _items = [];

    public string Code { get; private set; }
    public string Name { get; private set; }
    public Modality Modality { get; private set; }
    public string BodyPart { get; private set; }
    public int StandardDurationMinutes { get; private set; }
    public decimal Price { get; private set; }
    public bool RequiresPreparation { get; private set; }
    public bool RequiresConsent { get; private set; }
    public bool IsActive { get; private set; }

    public bool RequiresContrast => _items.Any(i => i.IsContrast);
    public IReadOnlyCollection<ExaminationTypeItem> Items => _items.AsReadOnly();

    private ExaminationType()
    {
        Code = null!;
        Name = null!;
        Modality = null!;
        BodyPart = null!;
    }

    public static ExaminationType Create(
        string code,
        string name,
        Modality modality,
        string bodyPart,
        int standardDurationMinutes = 0,
        decimal price = 0,
        bool requiresPreparation = false,
        bool requiresConsent = false)
    {
        Guard.AgainstNullOrWhiteSpace(code, nameof(code));
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Guard.AgainstNull(modality, nameof(modality));
        Guard.AgainstNullOrWhiteSpace(bodyPart, nameof(bodyPart));
        Guard.Against(standardDurationMinutes, d => d < 0, "Standard duration cannot be negative.");
        Guard.Against(price, p => p < 0, "Price cannot be negative.");

        var examinationType = new ExaminationType
        {
            Id = Guid.NewGuid(),
            Code = code.Trim(),
            Name = name.Trim(),
            Modality = modality,
            BodyPart = bodyPart.Trim(),
            StandardDurationMinutes = standardDurationMinutes,
            Price = price,
            RequiresPreparation = requiresPreparation,
            RequiresConsent = requiresConsent,
            IsActive = true
        };

        examinationType.RaiseDomainEvent(new ExaminationTypeCreatedEvent(examinationType.Id, examinationType.Code, examinationType.Name));
        return examinationType;
    }

    public void Update(
        string code,
        string name,
        Modality modality,
        string bodyPart,
        int standardDurationMinutes = 0,
        decimal price = 0,
        bool requiresPreparation = false,
        bool requiresConsent = false)
    {
        Guard.AgainstNullOrWhiteSpace(code, nameof(code));
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Guard.AgainstNull(modality, nameof(modality));
        Guard.AgainstNullOrWhiteSpace(bodyPart, nameof(bodyPart));
        Guard.Against(standardDurationMinutes, d => d < 0, "Standard duration cannot be negative.");
        Guard.Against(price, p => p < 0, "Price cannot be negative.");

        Code = code.Trim();
        Name = name.Trim();
        Modality = modality;
        BodyPart = bodyPart.Trim();
        StandardDurationMinutes = standardDurationMinutes;
        Price = price;
        RequiresPreparation = requiresPreparation;
        RequiresConsent = requiresConsent;

        RaiseDomainEvent(new ExaminationTypeUpdatedEvent(Id));
    }

    public ExaminationTypeItem AddItem(
        Guid itemId,
        int quantity,
        bool isContrast = false,
        bool isRequired = false,
        string? notes = null)
    {
        Guard.Against(_items.Any(i => i.ItemId == itemId), isDuplicate => isDuplicate, $"Item '{itemId}' is already in the preferences for examination type '{Code}'.");

        var item = ExaminationTypeItem.Create(Id, itemId, quantity, isContrast, isRequired, notes);
        _items.Add(item);
        return item;
    }

    public void UpdateItem(
        Guid examinationTypeItemId,
        int quantity,
        bool isContrast,
        bool isRequired,
        string? notes = null)
    {
        var item = _items.FirstOrDefault(i => i.Id == examinationTypeItemId)
            ?? throw new DomainException($"Preference item '{examinationTypeItemId}' is not on examination type '{Code}'.");
        item.Update(quantity, isContrast, isRequired, notes);
    }

    public void RemoveItem(Guid examinationTypeItemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == examinationTypeItemId)
            ?? throw new DomainException($"Preference item '{examinationTypeItemId}' is not on examination type '{Code}'.");
        _items.Remove(item);
    }

    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
    }
}

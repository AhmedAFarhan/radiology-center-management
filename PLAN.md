# Plan: Remove ExaminationHistory — Use Snapshot Fields on Examination

## Goal
Remove `ExaminationHistory` / `ExaminationHistoryItem` tables. Store frozen snapshot values on `Examination` and `ExaminationItem`. Analytics queries read from `Examination` (where `Status == Completed`).

---

## Snapshot Fields

### On `Examination` (add 5)

| Field | Type | Set when | Source |
|---|---|---|---|
| `TypePrice` | `decimal` | Book time | `ExaminationType.Price` |
| `TypeStandardDurationMinutes` | `int` | Book time | `ExaminationType.StandardDurationMinutes` |
| `RadiologistFee` | `decimal?` | Complete time | `IExaminationFeeResolver` |
| `TechnicianFee` | `decimal?` | Complete time | `IExaminationFeeResolver` |
| `ReferralFee` | `decimal?` | Complete time | `IExaminationFeeResolver` |

### On `ExaminationItem` (add 1)

| Field | Type | Set when | Source |
|---|---|---|---|
| `UnitCost` | `decimal` | Book time | `IItemSnapshotResolver` (weighted avg from StockMovement) |

---

## Implementation Steps

### Step 1: Domain — Examination.cs

**File:** `src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Domain/Entities/Examination.cs`

Add 5 properties after line 38 (before `RowVersion`):
```csharp
public decimal TypePrice { get; private set; }
public int TypeStandardDurationMinutes { get; private set; }
public decimal? RadiologistFee { get; private set; }
public decimal? TechnicianFee { get; private set; }
public decimal? ReferralFee { get; private set; }
```

Update `Create()` signature (line 50) — add params `decimal typePrice, int typeStandardDurationMinutes`:
```csharp
public static Examination Create(
    Guid patientId,
    Guid examinationTypeId,
    Guid? radiologistId,
    Guid? technicianId,
    string clinicalIndication,
    ExaminationPriority priority,
    decimal price,
    decimal typePrice,
    int typeStandardDurationMinutes,
    Guid? referralDoctorId = null,
    decimal discount = 0,
    bool isDiscountPercentage = false,
    decimal paid = 0,
    string? notes = null,
    Guid? equipmentId = null)
```

Set in initializer block (after line 92):
```csharp
TypePrice = typePrice,
TypeStandardDurationMinutes = typeStandardDurationMinutes,
```

Add method after `SetBilling` (after line 267):
```csharp
public void SetCompletionFees(decimal? radiologistFee, decimal? technicianFee, decimal? referralFee)
{
    RadiologistFee = radiologistFee;
    TechnicianFee = technicianFee;
    ReferralFee = referralFee;
}
```

### Step 2: Domain — ExaminationItem.cs

**File:** `src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Domain/Entities/ExaminationItem.cs`

Add property after line 13:
```csharp
public decimal UnitCost { get; private set; }
```

Update `Create()` — add param `decimal unitCost = 0` and set it:
```csharp
public static ExaminationItem Create(
    Guid examinationId,
    Guid itemId,
    int quantity,
    bool isContrast = false,
    bool isRequired = false,
    string? notes = null,
    decimal unitCost = 0)
{
    ...
    return new ExaminationItem
    {
        ...
        UnitCost = unitCost,
    };
}
```

### Step 3: Domain — Delete history entities

- DELETE `src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Domain/Entities/ExaminationHistory.cs`
- DELETE `src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Domain/Entities/ExaminationHistoryItem.cs`
- DELETE `src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Domain/ValueObjects/ItemSnapshot.cs`

### Step 4: Application — ExaminationCompletedEventHandler.cs

**File:** `src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Events/ExaminationCompletedEventHandler.cs`

Rewrite to:
- Remove: `IExaminationTypeDirectory`, `IItemSnapshotResolver`, `IExaminationHistoryRepository`
- Keep: `IExaminationRepository`, `IExaminationFeeResolver`, `IExaminationsUnitOfWork`
- Body: load examination → resolve fees → call `examination.SetCompletionFees(...)` → save

```csharp
public static async Task HandleAsync(
    ExaminationCompletedEvent e,
    IExaminationRepository examinationRepository,
    IExaminationFeeResolver examinationFeeResolver,
    IExaminationsUnitOfWork unitOfWork,
    CancellationToken ct)
{
    var examination = await examinationRepository.GetWithItemsAsync(e.ExaminationId, ct);
    if (examination is null)
        return;

    var fees = await examinationFeeResolver.ResolveAsync(
        examination.ExaminationTypeId,
        examination.TypePrice,
        examination.RadiologistId!.Value,
        examination.TechnicianId!.Value,
        examination.ReferralDoctorId,
        ct);

    examination.SetCompletionFees(fees?.RadiologistFee, fees?.TechnicianFee, fees?.ReferralFee);
    await unitOfWork.SaveChangesAsync(ct);
}
```

### Step 5: Application — Delete history abstractions

- DELETE `src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Abstractions/IExaminationHistoryRepository.cs`
- Keep `IItemSnapshotResolver` — still needed for book-time item cost resolution

### Step 6: Application — GetMonthlyProfitQueryHandler.cs

**File:** `src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Queries/GetMonthlyProfit/GetMonthlyProfitQueryHandler.cs`

Replace parameter `IExaminationHistoryRepository historyRepository` with `IExaminationRepository examinationRepository`.

Add `GetCompletedByRangeAsync` to `IExaminationRepository` and implement in `ExaminationRepository`.

Rewrite handler:
```csharp
public static async Task<Result<ProfitAnalyticsDto>> HandleAsync(
    GetMonthlyProfitQuery query,
    IExaminationRepository examinationRepository,
    IProfitSourceResolver profitSourceResolver,
    ITimezoneConverter timezone,
    CancellationToken ct)
{
    var today = timezone.GetLocalDate(DateTime.UtcNow);
    var fromDate = query.From?.Date ?? today.AddMonths(-1).AddDays(1).ToDateTime(TimeOnly.MinValue);
    var toDate = query.To?.Date.AddDays(1) ?? today.AddDays(1).ToDateTime(TimeOnly.MinValue);

    var fromUtc = timezone.ToUtc(fromDate);
    var toUtc = timezone.ToUtc(toDate);
    var examinations = await examinationRepository.GetCompletedByRangeAsync(fromUtc, toUtc, ct);

    var collected = examinations.Sum(e => e.Paid);
    var billed = examinations.Sum(Billable);
    var discounts = examinations.Sum(e => e.Price - Billable(e));
    var staffCaseFees = examinations.Sum(e => (e.RadiologistFee ?? 0m) + (e.TechnicianFee ?? 0m));
    var referralFees = examinations.Sum(e => e.ReferralFee ?? 0m);

    // ... rest same, but Billable takes Examination instead of ExaminationHistory
}

private static decimal Billable(Examination e) =>
    ExaminationPricing.BillableAmount(e.Price, e.Discount, e.IsDiscountPercentage);
```

### Step 7: Application — GetStaffMachineAnalyticsQueryHandler.cs

**File:** `src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Queries/GetStaffMachineAnalytics/GetStaffMachineAnalyticsQueryHandler.cs`

Replace `IExaminationHistoryRepository` with `IExaminationRepository`.

Query completed examinations in date range. For TypeName/TypeModality — use `IExaminationTypeDirectory` to build a lookup from `ExaminationTypeId`.

The grouping logic reads fees from examination properties directly.

### Step 8: Application — ExportStaffReportQueryHandler.cs

**File:** `src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Queries/ExportAnalytics/ExportStaffReportQueryHandler.cs`

Update signature: replace `IExaminationHistoryRepository` with `IExaminationRepository`. Pass to `GetStaffMachineAnalyticsQueryHandler.HandleAsync`.

### Step 9: Application — ExportProfitReportQueryHandler.cs

**File:** `src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Queries/ExportAnalytics/ExportProfitReportQueryHandler.cs`

Update signature: replace `IExaminationHistoryRepository` with `IExaminationRepository`. Pass to `GetMonthlyProfitQueryHandler.HandleAsync`.

### Step 10: Application — BookExaminationCommandHandler.cs

**File:** `src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Commands/BookExamination/BookExaminationCommandHandler.cs`

Already loads `examinationType` with `Price` and `StandardDurationMinutes`. Pass them to `Examination.Create()`:
```csharp
var examination = Examination.Create(
    command.PatientId,
    examinationType.Id,
    command.RadiologistId,
    command.TechnicianId,
    ...,
    priority,
    examinationType.Price,
    examinationType.Price,          // typePrice
    examinationType.StandardDurationMinutes,  // typeStandardDurationMinutes
    ...);
```

For item costs: inject `IItemSnapshotResolver` and resolve costs after seeding items:
```csharp
foreach (var seeded in ExaminationItemSeeding.Build(examinationType))
    examination.AddItem(seeded.ItemId, seeded.Quantity, seeded.IsContrast, seeded.IsRequired);
```

After adding items, resolve costs and set `UnitCost` on each item. Need to load items after `AddItem` returns them, or resolve costs inline.

Actually, `AddItem` returns the `ExaminationItem`. So:
```csharp
var itemCosts = await itemSnapshotResolver.ResolveAsync(
    ExaminationItemSeeding.Build(examinationType).Select(i => i.ItemId), ct);

foreach (var seeded in ExaminationItemSeeding.Build(examinationType))
{
    var item = examination.AddItem(seeded.ItemId, seeded.Quantity, seeded.IsContrast, seeded.IsRequired);
    if (itemCosts.TryGetValue(seeded.ItemId, out var snapshot))
        item.UnitCost = snapshot.UnitCost;  // Need setter or method
}
```

Problem: `ExaminationItem.UnitCost` has `private set`. Need to either:
- Make it settable via a method on `ExaminationItem`
- Or set it in `Create()` by passing `unitCost`

Best: pass `unitCost` to `AddItem` → `ExaminationItem.Create()`:
```csharp
public ExaminationItem AddItem(
    Guid itemId, int quantity, bool isContrast, bool isRequired, string? notes, decimal unitCost = 0)
{
    ...
    var item = ExaminationItem.Create(Id, itemId, quantity, isContrast, isRequired, notes, unitCost);
    ...
}
```

Then in the handler:
```csharp
var itemCosts = await itemSnapshotResolver.ResolveAsync(
    ExaminationItemSeeding.Build(examinationType).Select(i => i.ItemId), ct);

foreach (var seeded in ExaminationItemSeeding.Build(examinationType))
{
    var unitCost = itemCosts.TryGetValue(seeded.ItemId, out var s) ? s.UnitCost : 0m;
    examination.AddItem(seeded.ItemId, seeded.Quantity, seeded.IsContrast, seeded.IsRequired, unitCost: unitCost);
}
```

### Step 11: Infrastructure — ExaminationHistoryRepository.cs

DELETE `src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Infrastructure/Repositories/ExaminationHistoryRepository.cs`

### Step 12: Infrastructure — ExaminationsInfrastructureRegistration.cs

**File:** `src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Infrastructure/ExaminationsInfrastructureRegistration.cs`

Remove line 24: `services.AddScoped<IExaminationHistoryRepository, ExaminationHistoryRepository>();`

### Step 13: Infrastructure — ExaminationsDbContext.cs

**File:** `src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Infrastructure/Persistence/ExaminationsDbContext.cs`

Remove lines 11-12:
```csharp
public DbSet<ExaminationHistory> ExaminationHistories => Set<ExaminationHistory>();
public DbSet<ExaminationHistoryItem> ExaminationHistoryItems => Set<ExaminationHistoryItem>();
```

### Step 14: Infrastructure — EF Configurations

DELETE:
- `src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Infrastructure/Persistence/Configurations/ExaminationHistoryConfiguration.cs`
- `src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Infrastructure/Persistence/Configurations/ExaminationHistoryItemConfiguration.cs`

Update `ExaminationItemConfiguration.cs` — add `UnitCost` precision if not already there.

### Step 15: Infrastructure — ExaminationRepository.cs

**File:** `src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Infrastructure/Repositories/ExaminationRepository.cs`

Add `GetCompletedByRangeAsync`:
```csharp
public async Task<IReadOnlyList<Examination>> GetCompletedByRangeAsync(
    DateTime? from, DateTime? to, CancellationToken ct = default)
{
    var query = DbSet.Where(e => e.Status == ExaminationStatus.Completed);
    if (from is not null)
        query = query.Where(e => e.CompletedAt >= from);
    if (to is not null)
        query = query.Where(e => e.CompletedAt <= to);
    return await query.ToListAsync(ct);
}
```

### Step 16: Localhost — ReferralFeeStatementResolver.cs

**File:** `src/Backend/Localhost/RadiologyCenter.Localhost/Extensions/ReferralFeeStatementResolver.cs`

Replace `IExaminationHistoryRepository` with `IExaminationRepository` + `IExaminationTypeDirectory`.

Query completed examinations with `ReferralDoctorId != null && ReferralFee > 0` in date range.

For TypeName — resolve via `IExaminationTypeDirectory.GetWithItemsByIdsAsync`.

### Step 17: Localhost — PayrollFeeIncomeResolver.cs

**File:** `src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Infrastructure/Adapters/PayrollFeeIncomeResolver.cs`

Replace `IExaminationHistoryRepository` with `IExaminationRepository` + `IExaminationTypeDirectory`.

Query completed examinations where `RadiologistId == staffId || TechnicianId == staffId` in date range.

For TypeName — resolve via `IExaminationTypeDirectory`.

### Step 18: Migration

Generate migration to:
- Add columns to `Examinations.Examinations`: `TypePrice decimal(18,2) NOT NULL DEFAULT 0`, `TypeStandardDurationMinutes int NOT NULL DEFAULT 0`, `RadiologistFee decimal(18,2) NULL`, `TechnicianFee decimal(18,2) NULL`, `ReferralFee decimal(18,2) NULL`
- Add column to `Examinations.ExaminationItems`: `UnitCost decimal(18,2) NOT NULL DEFAULT 0`
- Drop tables: `Examinations.ExaminationHistoryItems`, `Examinations.ExaminationHistories`

### Step 19: Build & Verify

1. `dotnet build src/RadiologyCenter.slnx` — fix compilation errors
2. `dotnet ef migrations add RemoveExaminationHistory ...` — generate migration
3. Verify integration tests pass

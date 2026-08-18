namespace RadiologyCenter.Desktop.Models;

public sealed class ApiEnvelope<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public ApiError? Error { get; set; }
}

public sealed class ApiEnvelope
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public ApiError? Error { get; set; }
}

public sealed class ApiError
{
    public string? Code { get; set; }
    public string? Message { get; set; }
    public object? Details { get; set; }
}

public sealed record TokenResult(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    DateTime RefreshTokenExpiresAt,
    bool MustChangePassword = false);

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount);

public sealed record PatientDto(
    string Id,
    string PatientCode,
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName,
    DateTime? DateOfBirth,
    int? Age,
    string Gender,
    string PhoneNumber,
    string? Email,
    string? Address,
    string? NationalId,
    string? BloodType,
    string? Allergies,
    string? MedicalHistory,
    string? ReferringPhysician,
    bool IsActive,
    DateTime CreatedAt);

public sealed class PatientInput
{
    public string FullName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public int? Age { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? NationalId { get; set; }
    public string? BloodType { get; set; }
    public string? Allergies { get; set; }
    public string? MedicalHistory { get; set; }
    public string? ReferringPhysician { get; set; }
}

public sealed record ItemDto(
    string Id,
    string Name,
    string? Brand,
    string Category,
    string Unit,
    int ReorderLevel,
    int ReorderQuantity,
    bool LotTracked,
    string? StorageInstructions,
    bool IsActive,
    DateTime CreatedAt);

public sealed class ItemInput
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public int ReorderLevel { get; set; }
    public int ReorderQuantity { get; set; }
    public bool LotTracked { get; set; }
    public string? StorageInstructions { get; set; }
}

public sealed record ItemStockDto(
    string ItemId,
    string ItemName,
    int StockOnHand,
    IReadOnlyList<StockBatchDto> Batches);

public sealed record StockBatchDto(
    string Id,
    string ItemId,
    string LotNumber,
    DateTime? ExpiryDate,
    int QuantityReceived,
    int QuantityRemaining,
    string? SupplierId,
    bool IsExpired);

public sealed class IssueStockInput
{
    public int Quantity { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
}

public sealed record SupplierDto(
    string Id,
    string Name,
    string? ContactPerson,
    string Phone,
    string? Email,
    string? Address,
    string? TaxNumber,
    string? PaymentTerms,
    bool IsActive,
    DateTime CreatedAt);

public sealed class SupplierInput
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? TaxNumber { get; set; }
    public string? PaymentTerms { get; set; }
}

public sealed record StockMovementDto(
    string Id,
    string ItemId,
    string? StockBatchId,
    string MovementType,
    int Quantity,
    decimal? UnitCost,
    string? Reference,
    string? Notes,
    DateTime CreatedAt);

public sealed record PurchaseOrderItemDto(
    string Id,
    string ItemId,
    string ItemName,
    int QuantityOrdered,
    decimal UnitCost,
    int QuantityReceived);

public sealed record PurchaseOrderDto(
    string Id,
    string OrderNumber,
    string SupplierId,
    string SupplierName,
    string Status,
    DateTime? ExpectedDeliveryAt,
    DateTime? ReceivedAt,
    string? Notes,
    IReadOnlyList<PurchaseOrderItemDto> Items);

public sealed class PurchaseOrderLineInput
{
    public string ItemId { get; set; } = string.Empty;
    public int QuantityOrdered { get; set; }
    public decimal UnitCost { get; set; }
}

public sealed class CreatePurchaseOrderInput
{
    public string SupplierId { get; set; } = string.Empty;
    public List<PurchaseOrderLineInput> Items { get; set; } = new();
    public DateTime? ExpectedDeliveryAt { get; set; }
    public string? Notes { get; set; }
}

public sealed class ReceivePurchaseOrderLineInput
{
    public string ItemId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string LotNumber { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
}

public sealed class ReceivePurchaseOrderInput
{
    public List<ReceivePurchaseOrderLineInput> Lines { get; set; } = new();
}

public sealed record ExaminationDto(
    string Id,
    string PatientId,
    string ExaminationTypeId,
    string ExaminationTypeName,
    string? ReferralDoctorId,
    string RadiologistId,
    string TechnicianId,
    string ClinicalIndication,
    string Priority,
    string PriorityKey,
    string Status,
    string StatusKey,
    DateTime? ScheduledAt,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    string? PerformedByUserId,
    string? Notes,
    string? CancellationReason,
    decimal Price,
    decimal Discount,
    bool IsDiscountPercentage,
    decimal Paid,
    decimal Remaining,
    string? StudyInstanceUID,
    string? AccessionNumber,
    DateTime? ImagesReceivedAt,
    IReadOnlyList<ExaminationItemDto> Items);

public sealed record ExaminationItemDto(
    string Id,
    string ItemId,
    int Quantity,
    bool IsContrast,
    bool IsRequired,
    string? Notes);

public sealed record ExaminationTypeDto(
    string Id,
    string Code,
    string Name,
    string Modality,
    string BodyPart,
    int StandardDurationMinutes,
    decimal Price,
    bool RequiresPreparation,
    bool RequiresConsent,
    bool RequiresContrast,
    bool IsActive,
    IReadOnlyList<ExaminationTypeItemDto> Items,
    DateTime CreatedAt);

public sealed record ExaminationTypeItemDto(
    string Id,
    string ItemId,
    int Quantity,
    bool IsContrast,
    bool IsRequired,
    string? Notes);

public sealed class ExaminationTypeItemInput
{
    public string ItemId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public bool IsContrast { get; set; }
    public bool IsRequired { get; set; }
    public string? Notes { get; set; }
}

public sealed class ExaminationTypeInput
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Modality { get; set; } = string.Empty;
    public string BodyPart { get; set; } = string.Empty;
    public int StandardDurationMinutes { get; set; }
    public decimal Price { get; set; }
    public bool RequiresPreparation { get; set; }
    public bool RequiresConsent { get; set; }
    public List<ExaminationTypeItemInput> Items { get; set; } = new();
}

public sealed class ExaminationInput
{
    public string PatientId { get; set; } = string.Empty;
    public string ExaminationTypeId { get; set; } = string.Empty;
    public string RadiologistId { get; set; } = string.Empty;
    public string TechnicianId { get; set; } = string.Empty;
    public string? ReferralDoctorId { get; set; }
    public string ClinicalIndication { get; set; } = string.Empty;
    public string Priority { get; set; } = "Routine";
    public decimal Discount { get; set; }
    public bool IsDiscountPercentage { get; set; }
    public decimal Paid { get; set; }
    public string? Notes { get; set; }
}

public sealed class ExaminationUpdateInput
{
    public string RadiologistId { get; set; } = string.Empty;
    public string TechnicianId { get; set; } = string.Empty;
    public string? ReferralDoctorId { get; set; }
    public string ClinicalIndication { get; set; } = string.Empty;
    public string Priority { get; set; } = "Routine";
    public string? Notes { get; set; }
    public decimal? Discount { get; set; }
    public bool? IsDiscountPercentage { get; set; }
    public decimal? Paid { get; set; }
    public List<ExaminationItemInput>? Items { get; set; }
}

public sealed class ExaminationItemInput
{
    public string ItemId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public bool IsContrast { get; set; }
    public bool IsRequired { get; set; }
    public string? Notes { get; set; }
}

public sealed record UserDto(
    string Id,
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    bool IsActive,
    bool EmailConfirmed,
    bool TwoFactorEnabled,
    bool LockoutEnabled,
    DateTimeOffset? LockoutEnd,
    DateTime? LastLoginAt,
    DateTime CreatedAt)
{
    public string FullName => string.Join(' ', new[] { FirstName, LastName }.Where(p => !string.IsNullOrWhiteSpace(p)));
    public bool IsLocked => LockoutEnd is { } end && end > DateTimeOffset.UtcNow;
}

public sealed class CreateUserInput
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<string> RoleIds { get; set; } = new();
}

public sealed class UpdateUserProfileInput
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}

public sealed class UpdateUserRolesInput
{
    public List<string> RoleIds { get; set; } = new();
}

public sealed class LockUserInput
{
    public DateTimeOffset LockoutEnd { get; set; }
}

public sealed record RoleDto(
    string Id,
    string Name,
    string? Description,
    bool IsSystem,
    bool IsActive,
    DateTime CreatedAt,
    IReadOnlyCollection<string> Permissions);

public sealed class CreateRoleInput
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public sealed class UpdateRoleInput
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public sealed record PermissionDto(
    string Code,
    string Name,
    string? Description,
    string? Group);

public sealed record EquipmentDto(
    string Id,
    string Name,
    string? SerialNumber,
    string Modality,
    string Status,
    DateTime? PurchaseDate,
    bool IsActive,
    DateTime CreatedAt);

public sealed class EquipmentInput
{
    public string Name { get; set; } = string.Empty;
    public string Modality { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }
    public DateTime? PurchaseDate { get; set; }
}

public sealed record StaffDto(
    string Id,
    string UserId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName,
    string PhoneNumber,
    string Position,
    string? Department,
    string? Specialization,
    string? LicenseNumber,
    DateTime HireDate,
    bool IsActive,
    DateTime CreatedAt);

public sealed class StaffInput
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public string? Department { get; set; }
    public string? Specialization { get; set; }
    public string? LicenseNumber { get; set; }
}

public sealed record WorkShiftDto(
    string Id,
    string StaffId,
    string? EquipmentId,
    DateTime Date,
    TimeSpan StartTime,
    TimeSpan EndTime,
    string? Notes,
    DateTime CreatedAt);

public sealed class WorkShiftInput
{
    public string StaffId { get; set; } = string.Empty;
    public string? EquipmentId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Notes { get; set; }
}

public sealed record LeaveDto(
    string Id,
    string StaffId,
    string LeaveType,
    DateTime StartDate,
    DateTime EndDate,
    string? Reason,
    DateTime CreatedAt);

public sealed class LeaveInput
{
    public string StaffId { get; set; } = string.Empty;
    public string LeaveType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Reason { get; set; }
}

public sealed record ReferralDoctorDto(
    string Id,
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName,
    string Phone,
    string? Email,
    string? Specialization,
    string? Hospital,
    bool IsActive,
    DateTime CreatedAt);

public sealed class ReferralDoctorInput
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Specialization { get; set; }
    public string? Hospital { get; set; }
}

public sealed record PayRunDto(
    string Id,
    DateTime RunFrom,
    DateTime RunTo,
    string Status,
    string? ProcessedBy,
    DateTime? ProcessedAt,
    string? Notes,
    IReadOnlyList<PayslipDto> Payslips);

public sealed record PayslipDto(
    string Id,
    string PayRunId,
    string StaffId,
    decimal GrossSalary,
    int UnpaidLeaveDays,
    decimal UnpaidLeaveDeduction,
    decimal TotalEarnings,
    decimal TotalDeductions,
    decimal NetSalary,
    string? Notes,
    IReadOnlyList<PayslipComponentDto> Components);

public sealed record PayslipComponentDto(
    string Id,
    string Name,
    decimal Amount,
    bool IsDeduction);

public sealed class CreatePayRunInput
{
    public DateTime RunFrom { get; set; }
    public DateTime RunTo { get; set; }
    public string? Notes { get; set; }
}

public sealed record SalaryDto(
    string Id,
    string StaffId,
    decimal BaseSalary,
    string SalaryType,
    DateTime EffectiveDate,
    bool IsActive);

public sealed class SalaryInput
{
    public string StaffId { get; set; } = string.Empty;
    public decimal BaseSalary { get; set; }
    public string SalaryType { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
}

public sealed record SalaryComponentDto(
    string Id,
    string Name,
    string Kind,
    string? Frequency,
    bool IsPercentage,
    bool IsPerWorkDay,
    decimal DefaultValue,
    bool IsActive);

public sealed class SalaryComponentInput
{
    public string Name { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public string? Frequency { get; set; }
    public bool IsPercentage { get; set; }
    public bool IsPerWorkDay { get; set; }
    public decimal DefaultValue { get; set; }
}

public sealed record AllowanceAssignmentDto(
    string Id,
    string StaffId,
    string? SalaryComponentId,
    string Name,
    decimal Amount,
    string? Frequency,
    bool IsPerWorkDay,
    DateTime EffectiveDate,
    DateTime? EndDate,
    bool IsActive);

public sealed class AllowanceAssignmentInput
{
    public string StaffId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? SalaryComponentId { get; set; }
    public string? Frequency { get; set; }
    public bool IsPerWorkDay { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public sealed record ExaminationFeeDto(
    string Id,
    string ExaminationTypeId,
    string Role,
    decimal Amount,
    bool IsPercentage,
    bool IsActive);

public sealed class ExaminationFeeInput
{
    public string ExaminationTypeId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsPercentage { get; set; }
}

public sealed record ReferralFeeDto(
    string Id,
    string ReferralDoctorId,
    string ExaminationTypeId,
    decimal Amount,
    bool IsPercentage,
    bool IsActive);

public sealed class ReferralFeeInput
{
    public string ReferralDoctorId { get; set; } = string.Empty;
    public string ExaminationTypeId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsPercentage { get; set; }
}
public sealed record ReportDto(
    string Id,
    string ExaminationId,
    string PatientId,
    string RadiologistId,
    string Status,
    string StatusKey,
    int CurrentVersionNumber,
    DateTime? FinalizedAt,
    string? CancelReason,
    ReportVersionDto? CurrentVersion,
    string? PatientName = null,
    string? RadiologistName = null,
    string? ExaminationTypeName = null);

public sealed record ReportListItemDto(
    string Id,
    string ExaminationId,
    string PatientId,
    string RadiologistId,
    string Status,
    string StatusKey,
    int CurrentVersionNumber,
    DateTime? FinalizedAt,
    string? CancelReason,
    string? PatientName = null,
    string? RadiologistName = null,
    string? ExaminationTypeName = null);

public sealed record ReportVersionDto(
    string Id,
    int VersionNumber,
    string? AmendmentReason,
    DateTime CreatedAt,
    IReadOnlyList<ReportSectionDto> Sections,
    IReadOnlyList<ReportFindingDto> Findings);

public sealed record ReportSectionDto(
    string Id,
    string SectionType,
    string Title,
    string Body,
    int Position,
    bool IsLocked);

public sealed record ReportFindingDto(
    string Id,
    string Region,
    string Description,
    string Severity,
    int Position);

public sealed record ReportTemplateDto(
    string Id,
    string Name,
    string Modality,
    string? BodyPart,
    string? Description,
    bool IsActive,
    bool IsSystem,
    int UseCount,
    IReadOnlyList<ReportTemplateSectionDto> Sections);

public sealed record ReportTemplateSectionDto(
    string Id,
    string SectionType,
    string Title,
    string Body,
    int Position,
    bool IsLocked);

public sealed class CreateReportDraftInput
{
    public string ExaminationId { get; set; } = string.Empty;
    public string PatientId { get; set; } = string.Empty;
    public string RadiologistId { get; set; } = string.Empty;
}

public sealed class UpsertReportSectionInput
{
    public string SectionType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public int Position { get; set; }
    public bool IsLocked { get; set; }
}

public sealed class AddReportFindingInput
{
    public string Region { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public int Position { get; set; }
}

public sealed class UpdateReportFindingInput
{
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
}

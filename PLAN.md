# Plan: Backend Hardcoded Values & Plain Text Cleanup

## ✅ ALL PHASES COMPLETE

### Phase 1: Security-Critical — DONE
- Seed credentials → `appsettings.json` `Seed:Admin:*` section
- JWT secret → placeholder `<JWT_SECRET_KEY>`
- Connection string fallback → throws `InvalidOperationException`
- CORS → reads from `Cors:AllowedOrigins` config

### Phase 2: Centralize — DONE
- `TimezoneConstants` created (DefaultTimezone, WindowsTimezone)
- `CurrentUserService`, `UserTimezoneConverter`, `ClinicClock` updated
- `BrandConstants` created (CompanyName, PrimaryColor, LogoResourceName, UnknownModality)

### Phase 3: Type Safety — DONE
- Status strings → `ExaminationStatus.Completed.Name`, `PayRunStatus.X.Name`, `ClaimStatus.X.Name`
- `"Unknown"` → `BrandConstants.UnknownModality` (6 files)
- `"FixedPlusFees"` → `SalaryCalculationRule.FixedPlusFees.Name`

### Phase 4: Constants — DONE
- `ApiErrorCodes` constants class created
- `ExceptionMiddleware`, `ApiResponse` updated
- `Error.cs` left as string literals (Domain can't reference Application)

### Phase 5: PDF/Excel — DONE
- `PdfLabels` constants class created (20+ labels)
- `ExcelTheme` constants class created (3 colors)
- 3 PDF services + ExcelService updated

### Phase 6: Timezone Strategy — DONE (done previously)
### Phase 7: Domain Event Dispatch — DONE (done previously)
### Phase 8: UnitOfWork Fix — DONE (done previously)
### Phase 9: Reporting Tier 1 — DONE (done previously)

---

## Priority 1: Security-Critical (4 files)

### 1.1 Hardcoded Seed Credentials
**File:** `Identity.Infrastructure/Persistence/Seed/IdentityDbSeeder.cs`
- `AdminUserName = "admin123"` → Move to `appsettings.json` under `Seed:Admin:Username`
- `AdminPassword = "admin123"` → Move to `appsettings.json` under `Seed:Admin:Password`
- `AdminRoleName = "Administrator"` → Keep as constant (intentional)
- `"admin@radiologycenter.local"`, `"01000000000"` → Move to config

### 1.2 JWT Secret in appsettings.json
**File:** `Localhost/appsettings.json` line 18
- `"SecretKey": "a3f8c9e1b2d4f6a7c8e9f0a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0"`
- **Action:** Replace with environment variable placeholder `"<JWT_SECRET_KEY>"` and add comment. Use `IConfiguration` or User Secrets for actual value.

### 1.3 Hardcoded Connection String Fallback
**File:** `BuildingBlocks.Infrastructure/Persistence/AppDbContextFactory.cs` line 40
- `"Server=.;Database=RadiologyCenter;Trusted_Connection=True;TrustServerCertificate=True;"`
- **Action:** Throw `InvalidOperationException` with clear message instead of fallback

### 1.4 CORS Policy
**File:** `Localhost/Program.cs` lines 60-69
- `.AllowAnyHeader()`, `.AllowAnyMethod()`, `.SetIsOriginAllowed(_ => true)`
- **Action:** Read allowed origins from config, restrict in production

---

## Priority 2: Timezone Centralization (8 files)

All instances of `"Africa/Cairo"` → single config value `DefaultTimeZone` in `appsettings.json`

| File | Line | Current |
|---|---|---|
| `Identity.Domain/Entities/User.cs` | 38, 41 | Default param `"Africa/Cairo"` |
| `BuildingBlocks.Infrastructure/Services/CurrentUserService.cs` | 22 | Fallback `"Africa/Cairo"` |
| `BuildingBlocks.Infrastructure/Services/UserTimezoneConverter.cs` | 13, 48 | Fallback `"Africa/Cairo"` |
| `Examinations.Application/Scheduling/ClinicClock.cs` | 20, 23 | `"Africa/Cairo"`, `"Egypt Standard Time"` |
| `Identity.Infrastructure/Persistence/Configurations/UserConfiguration.cs` | 15 | `.HasDefaultValue("Africa/Cairo")` |

**Action:**
1. Add `"DefaultTimeZone": "Africa/Cairo"` to `appsettings.json`
2. Create `TimezoneConstants` static class in BuildingBlocks
3. Read from `IConfiguration` at runtime, use constant only for EF default value
4. Domain entity default stays as fallback (can't inject DI into domain)

---

## Priority 3: Status String Comparisons → Enumeration Objects (9 findings)

### 3.1 ClaimStatus Strings
**File:** `Examinations.Application/Queries/ExportAnalytics/GetInsuranceAnalyticsQueryHandler.cs` lines 22-26
- `"Draft"`, `"Submitted"`, `"Approved"`, `"Rejected"`, `"Paid"`
- **Action:** Use `ClaimStatus.Draft.Name`, `ClaimStatus.Submitted.Name`, etc.

### 3.2 ExaminationStatus Strings
**File:** `Examinations.Application/Queries/GetOperationalAnalytics/GetOperationalAnalyticsQueryHandler.cs` lines 21-22, 27, 52, 61
- `"Completed"`, `"Cancelled"`
- **Action:** Use `ExaminationStatus.Completed.Name`, `ExaminationStatus.Cancelled.Name`

### 3.3 PayRunStatus Strings (2 files)
**Files:**
- `Payroll.Infrastructure/Services/PayslipPdfService.cs` lines 314-318
- `Payroll.Infrastructure/Services/ReferralFeeStatementPdfService.cs` lines 206-210
- `"Draft"`, `"Computed"`, `"Approved"`, `"Paid"`, `"Rejected"`
- **Action:** Use `PayRunStatus.Draft.Name` etc. Extract shared method for color mapping.

### 3.4 ClaimStatus in InsuranceAnalyticsDataSource
**File:** `Localhost/Extensions/InsuranceAnalyticsDataSource.cs` line 113
- `"Approved"`, `"Paid"`
- **Action:** Use `ClaimStatus.Approved.Name`, `ClaimStatus.Paid.Name`

### 3.5 "FixedPlusFees" Defaults
**Files:**
- `Payroll.Infrastructure/Services/PayslipPdfService.cs` line 50
- `Payroll.Application/DTOs/PayslipPdfDto.cs` line 11
- `ResourceManagement.Application/DTOs/StaffDto.cs` line 19
- **Action:** Use `SalaryCalculationRule.FixedPlusFees.Name`

### 3.6 "Unknown" Fallback (6 files)
All use `"Unknown"` as modality fallback:
- `Examinations.Infrastructure/Adapters/PayrollFeeIncomeResolver.cs`
- `Examinations.Application/Queries/GetOperationalAnalytics/GetOperationalAnalyticsQueryHandler.cs`
- `Examinations.Application/Queries/GetFinancialAnalytics/GetFinancialAnalyticsQueryHandler.cs`
- `Examinations.Application/Queries/GetStaffMachineAnalytics/GetStaffMachineAnalyticsQueryHandler.cs`
- `Examinations.Application/Queries/GetExaminationsForCalendar/GetExaminationsForCalendarQueryHandler.cs`
- `Localhost/Extensions/ReferralFeeStatementResolver.cs`
- **Action:** Create `const string UnknownModality = "Unknown"` in a shared constants class

---

## Priority 4: Magic Numbers → Named Constants (8 findings)

### 4.1 JWT Expiration Defaults
**File:** `Identity.Infrastructure/Settings/JwtOptions.cs` lines 8-9
- `15` (minutes), `7` (days)
- **Action:** Already configurable via appsettings. Remove `= 15` / `= 7` defaults, require config.

### 4.2 Lockout Defaults
**File:** `Identity.Application/Settings/LockoutOptions.cs` lines 5-6
- `5` (attempts), `5` (minutes)
- **Action:** Same — remove defaults, require config.

### 4.3 Password Policy
**File:** `BuildingBlocks.Application/Validation/PasswordPolicyRule.cs` lines 7-8
- `MinLength = 8`, `MaxLength = 100`
- **Action:** Make configurable via `IOptions<PasswordPolicy>` or keep as `const` with documentation.

### 4.4 Pagination Defaults
**File:** `BuildingBlocks.Domain/Pagination/PaginationParams.cs` lines 5, 7-8
- `MaxPageSize = 100`, `_pageNumber = 1`, `_pageSize = 10`
- **Action:** Keep as `const` (these are reasonable defaults, not business config).

### 4.5 Aging Bucket Days
**File:** `Examinations.Application/Queries/GetFinancialAnalytics/GetFinancialAnalyticsQueryHandler.cs` lines 10-12
- `30`, `60`, `90`
- **Action:** Create `AgingBucketConfig` record, inject via DI or keep as `const` with doc comment.

### 4.6 Clinic Work Hours
**File:** `Examinations.Application/Queries/GetAvailableSlots/GetAvailableSlotsQueryHandler.cs` lines 8-9
- `DayStart = new TimeOnly(8, 0)`, `DayEnd = new TimeOnly(17, 0)`
- **Action:** Make configurable per clinic/equipment or keep as `const` with doc comment.

### 4.7 Global Search Scoring
**File:** `Localhost/Services/GlobalSearch/GlobalSearchService.cs` lines 28-31, 401-408
- `DefaultLimit = 5`, `MaxLimit = 10`, scoring weights `40`, `30`, `20`, `10`
- **Action:** Keep as `const` in the service (internal implementation detail).

### 4.8 Report Finding Max Length
**Files:** 3 files with `5000`
- **Action:** Keep as `const` (already consistent).

---

## Priority 5: Hardcoded PDF/Brand Strings (3 files, ~30 strings)

### 5.1 Company Name
**Files:**
- `Payroll.Infrastructure/Services/PayslipPdfService.cs` line 102
- `Payroll.Infrastructure/Services/ReferralFeeStatementPdfService.cs` line 86
- `Examinations.Infrastructure/Services/AnalyticsPdfService.cs` line 515
- `"RADIOLOGY CENTER"`
- **Action:** Add `"CompanyName": "RADIOLOGY CENTER"` to `appsettings.json`, inject via `IConfiguration`

### 5.2 PDF Labels
All English labels in PDFs: `"PAYSLIP"`, `"EARNINGS"`, `"DEDUCTIONS"`, etc.
- **Action:** Create `PdfLabels` static class with all labels as constants, or use localization. Since PDFs are QuestPDF (code-based), constants are simplest.

### 5.3 Primary Color
**Files:** 3 PDF services with `"#4C58E0"`
- **Action:** Add `"PrimaryColor": "#4C58E0"` to config, or create `BrandColors` constants class.

### 5.4 Logo Resource Path
**Files:** 2 PDF services with `"RadiologyCenter.Payroll.Infrastructure.Resources.logo.png"`
- **Action:** Create `const string LogoResourceName` in a shared location.

---

## Priority 6: Error/Status Code Strings (4 findings)

### 6.1 ExceptionMiddleware Error Codes
**File:** `Localhost/Middleware/ExceptionMiddleware.cs` lines 31, 41, 54, 61, 68, 82
- `"NotFound"`, `"Validation"`, `"Conflict"`, `"InternalError"`, `"DomainError"`
- **Action:** Create `ApiErrorCodes` static class in BuildingBlocks

### 6.2 ApiResponse Default Error
**File:** `BuildingBlocks.Application/Common/ApiResponse.cs` line 103
- `"Error"`
- **Action:** Use `ApiErrorCodes.Error` constant

### 6.3 Error.Factory Strings
**File:** `BuildingBlocks.Domain/Results/Error.cs` lines 28, 40
- `"Conflict"`, `"Failure"`
- **Action:** Use `ApiErrorCodes` constants

---

## Priority 7: Plain Text Validation Messages (8 files, ~20 messages)

### 7.1 BuildingBlocks Validation
- `PasswordPolicyRule.cs` line 25 — English password message
- `EgyptianPhoneNumberRule.cs` line 33 — English phone message
- **Action:** These already have error codes. The English text is a fallback. Acceptable as-is since the frontend handles localization.

### 7.2 Domain Validation Messages
- `Patient.cs` lines 149-164 — 5 messages
- `Payslip.cs` lines 36-38, 65 — 4 messages
- **Action:** These are `DomainException` messages. They have error codes. The English text is fallback. Acceptable.

### 7.3 Application Error Messages
- `LoginCommandHandler.cs` lines 23, 26, 37
- `SendNotificationCommandHandler.cs` lines 20, 32, 61, 67
- `PayslipPdfService.cs` lines 32, 35, 38
- `ReferralFeeStatementPdfService.cs` lines 32, 35, 38
- **Action:** These have error codes. The English text is fallback. Acceptable.

**Note:** Per the user's directive, error messages must never contain raw IDs/GUIDs. These messages use entity names, not IDs. ✓

---

## Priority 8: File/Resource Paths (5 findings)

### 8.1 Embedded Resource Paths
- `PayslipPdfService.cs` line 118 — `"RadiologyCenter.Payroll.Infrastructure.Resources.logo.png"`
- `ReferralFeeStatementPdfService.cs` line 102 — same
- **Action:** Create `const string LogoResourceName` in Payroll BuildingBlocks

### 8.2 Insurance Storage Path
- `InsuranceInfrastructureRegistration.cs` line 17 — `"App_Data/Insurance"`
- **Action:** Already configurable via `appsettings.json`. Fallback is acceptable.

### 8.3 Localization Resources Path
- `JsonTranslator.cs` lines 110-111 — `"Resources"`
- `Program.cs` line 187 — `"Resources"`
- **Action:** Create `const string LocalizationResourcesDir` in BuildingBlocks

### 8.4 Document Storage Directories
- `UploadPreAuthorizationDocumentCommandHandler.cs` line 24 — `"preauthorizations"`
- `UploadPolicyDocumentCommandHandler.cs` line 24 — `"policies"`
- **Action:** These are subdirectory names under the configurable root. Acceptable as constants.

---

## Priority 9: Excel Styling Colors (1 file)

**File:** `BuildingBlocks.Infrastructure/Excel/ExcelService.cs` lines 30, 84, 95, 106
- `"#F3F4F6"`, `"#9CA3AF"`, `"#3B82F6"`
- **Action:** Create `ExcelTheme` static class with named constants.

---

## Implementation Order

1. **Phase 1 (Security):** Seed credentials → config, JWT secret → env var, connection string fallback → throw, CORS → config
2. **Phase 2 (Centralize):** Timezone → single config point, Company name/color → config
3. **Phase 3 (Type Safety):** Status strings → enum references, "FixedPlusFees" → enum, "Unknown" → constant
4. **Phase 4 (Constants):** Magic numbers → named constants, error codes → shared class
5. **Phase 5 (PDF):** Labels → constants class, logo path → constant, colors → constants

---

## Files to Create

| File | Purpose |
|---|---|
| `BuildingBlocks.Application/Common/ApiErrorCodes.cs` | API error code constants |
| `BuildingBlocks.Application/Common/BrandConstants.cs` | Company name, primary color, logo path |
| `BuildingBlocks.Application/Common/UnknownConstants.cs` | `"Unknown"` fallback constant |
| `BuildingBlocks.Application/Settings/PasswordPolicyOptions.cs` | Configurable password policy |
| `BuildingBlocks.Infrastructure/Excel/ExcelTheme.cs` | Excel styling color constants |

## Files to Modify

| File | Change |
|---|---|
| `Localhost/appsettings.json` | Add `DefaultTimeZone`, `CompanyName`, `PrimaryColor`, `Seed:Admin:*`, remove JWT secret default |
| `Localhost/Program.cs` | Read CORS origins from config |
| `Identity.Infrastructure/Seed/IdentityDbSeeder.cs` | Read credentials from config |
| `Identity.Infrastructure/Settings/JwtOptions.cs` | Remove defaults |
| `Identity.Application/Settings/LockoutOptions.cs` | Remove defaults |
| `BuildingBlocks.Infrastructure/Persistence/AppDbContextFactory.cs` | Throw instead of fallback |
| `BuildingBlocks.Infrastructure/Services/CurrentUserService.cs` | Read from config |
| `BuildingBlocks.Infrastructure/Services/UserTimezoneConverter.cs` | Read from config |
| `Examinations.Application/Scheduling/ClinicClock.cs` | Read from config |
| 6 analytics handlers | Replace string comparisons with enum references |
| 3 PDF services | Use shared constants |
| 2 PDF services | Use shared logo path constant |
| `Localhost/Middleware/ExceptionMiddleware.cs` | Use `ApiErrorCodes` |
| `BuildingBlocks.Application/Common/ApiResponse.cs` | Use `ApiErrorCodes` |
| `BuildingBlocks.Domain/Results/Error.cs` | Use `ApiErrorCodes` |
| `Localhost/Extensions/InsuranceAnalyticsDataSource.cs` | Use enum references |
| 3 DTO files | Use `SalaryCalculationRule.FixedPlusFees.Name` |

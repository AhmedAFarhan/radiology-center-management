# UI/UX review and implementation — 25 October 2026

## Scope and evidence

Source pass across all 39 routes and the desktop UI inventory: 144 Razor files (including import files), 130 Razor code-behind files, 133 scoped CSS files, and 3 XAML files. The scoped CSS files are placeholders; the visual system is primarily in `wwwroot/app.css` and `Shared/AppTheme.cs`. Relevant supporting services and the reading-room partial classes were also inspected where their behavior affected the UI.

This is a source review plus isolated browser layout verification. It is **not** a claim that all screens were exercised in a signed-in MAUI session. Browser fixtures use synthetic records and the repository's CSS; they do not connect to the clinic database. No clinic records, accounts, databases, or connection settings were changed by this work.

## Implemented

- **Calendar:** visible date and date picker; actual seven-day overview; list view; automatic list layout on smaller windows; one block per examination with actual duration; separate lanes for overlaps; one scrolling surface with sticky equipment and time headers; dynamic time bounds for early/late bookings; readable status/priority text; corrected time-marker positioning in Arabic; live clock updates; inline loading/error/retry states; stale responses ignored; appointment details drawer with keyboard dismissal. The existing calendar endpoint returns active appointments, so the UI explicitly labels that scope.
- **Booking:** readable selected patient/type names; operational equipment selection; stable modality keys for Arabic queries; duration/end-time summary; past-time checks; full-interval conflict check immediately before booking; existing server-side scheduling checks remain authoritative for concurrent workstations.
- **Shared navigation and headers:** wrapping controls and titles, no unused search field when no search handler exists, labelled searches, keyboard-operable profile menu, search result activation by keyboard, recent-search arrow navigation, focus kept within results, clear access-denied content for authenticated users.
- **Forms:** controls disabled during saves across editor dialogs, duplicate-save guard in the shared dialog base, cancel remains usable when only Save is unavailable, save labels remain visible while busy, labelled password visibility controls, appropriate email/telephone inputs, sensible birth/date-range bounds, scrollable change-password content, keyboard-accessible upload inputs.
- **Clinic setup:** explicit workstation/admin choices, persistent selected state, simple step explanations, server/database guidance, optional settings collapsed, and dialog dismissal disabled during setup.
- **Dashboard:** removed the sample revenue/doctor widgets from the live page; added permission-aware quick access and links from recent examinations; removed the permanently blank referral-doctor column; metrics queried only for permitted modules; in-progress total uses a filtered count rather than counting the first page.
- **Reading room:** load errors no longer disappear behind the empty-report state; save state disables section/finding changes; readable severity when a report is read-only; failed finding submissions preserve entered text; labelled severity/search and a visible Cancel action.
- **Analytics:** expandable accessible data tables for seven chart components, wrapping period controls, meaningful all-time date display, missing table empty state, and correct disposal of period subscriptions.
- **Permissions and visual accessibility:** empty-state create buttons follow the same permissions as page actions; notification-template creation is gated; stronger contrast for secondary/info/warning colors while retaining the primary brand color; clearer detail labels; accessible error dismissal and chart regions.

## Verification

- Debug build: passed with no errors or warnings in the recorded UI build.
- Release build: passed. The first full Release compilation also surfaced two existing nullable-initialization warnings in `TranslationBase.cs`; these are outside the UI changes. The final incremental Release build passed with 0 errors and 0 warnings (`ui-review-release-final.log`).
- Startup/regression suite: **27 passed**, including six new calendar tests covering real duration, overlapping/adjacent blocks, clipping, invalid/out-of-range records, and deterministic placement.
- Browser fixtures: **10 calendar size/language combinations** (400, 600, 800, 1212, 1440 pixels; English and Arabic) and **8 shared-header/setup combinations** (400, 600, 800, 1212 pixels; English and Arabic). Assertions cover page overflow, row alignment, sticky labels/header, scrolling, time-marker placement, compact list visibility, and dialog scrolling.
- Search focus helper: keyboard focus inside/outside the result area checked in the browser.
- Theme checks: white text contrast ratios for primary, secondary, info, warning, success and error colors all exceed 4.5:1 (approximately 4.98–5.77:1).
- `git diff --check`: passed.

Local evidence is in the ignored `audit-results` directory: `ui-review.trx`, `ui-review-tests.log`, `ui-review-build.log`, `ui-review-release.log`, `ui-review-release-final.log`, `ui-layout-verification.json`, `ui-shared-verification.json`, and `ui-calendar-*.png`. Fixtures and audit scripts are also retained there for inspection.

## Remaining acceptance work before deployment

**Live follow-up, 25 October:** the actual Windows app was subsequently exercised against the existing development database. Sign-in, dashboard, calendar navigation and several module lists were checked, and two functional UI defects were found, repaired and visually rechecked. See [the live walkthrough results](live-walkthrough-2026-10-25.md). This closes part of the original caveat; it does not establish multi-workstation or imaging acceptance.

Run a signed-in MAUI walkthrough using designated test records and the clinic's actual roles: booking from two workstations, Cairo date/time behavior, first login/password change, reading-room save/sign/retry, PACS viewing, imports/uploads, payroll/cash/insurance actions, and Arabic layouts at the clinic's Windows scaling settings. These workflows were not executed against the current clinic database during this UI pass.

The calendar is an active scheduling view; completed/cancelled examination history remains in Visits. An empty grid cell is not a promise of staffing or working-shift availability. Cash-session handover still uses the existing recipient-user-ID contract; a friendly recipient lookup should be added with the appropriate access policy before relying on that optional workflow.

No deployment or installer publication was performed. Existing release packages predate these changes and need to be rebuilt and validated when deployment work resumes.

## UI inventory

The table below records the screen/component source inventory, not a claim of live interaction with each item. Shared fixes apply to the pages and dialogs that use those components.

### Analytics

| UI source | Routes |
|---|---|
| `Features/Analytics/Components/AnalyticsSkeleton.razor` | Component / layout |
| `Features/Analytics/Components/ChartCard.razor` | Component / layout |
| `Features/Analytics/Components/DonutChart.razor` | Component / layout |
| `Features/Analytics/Components/ExportButton.razor` | Component / layout |
| `Features/Analytics/Components/ListBarChart.razor` | Component / layout |
| `Features/Analytics/Components/ModalityRevenueChart.razor` | Component / layout |
| `Features/Analytics/Components/ModalityVolumeChart.razor` | Component / layout |
| `Features/Analytics/Components/PeriodSelector.razor` | Component / layout |
| `Features/Analytics/Components/RevenueTrendChart.razor` | Component / layout |
| `Features/Analytics/Components/UtilizationChart.razor` | Component / layout |
| `Features/Analytics/Components/VolumeBarChart.razor` | Component / layout |
| `Features/Analytics/Pages/AnalyticsRedirect.razor` | `/analytics` |
| `Features/Analytics/Pages/CashFlowReport.razor` | `/analytics/cash-flow` |
| `Features/Analytics/Pages/FinancialAnalytics.razor` | `/analytics/financial` |
| `Features/Analytics/Pages/InsuranceAnalytics.razor` | `/analytics/insurance` |
| `Features/Analytics/Pages/OperationalAnalytics.razor` | `/analytics/operations` |
| `Features/Analytics/Pages/OverviewAnalytics.razor` | `/analytics/overview` |
| `Features/Analytics/Pages/StaffMachineAnalytics.razor` | `/analytics/staff` |

### Auth

| UI source | Routes |
|---|---|
| `Features/Auth/Components/ChangePasswordDialog.razor` | Component / layout |
| `Features/Auth/Components/LoginDialog.razor` | Component / layout |
| `Features/Auth/Components/ResetPasswordDialog.razor` | Component / layout |

### Cash

| UI source | Routes |
|---|---|
| `Features/Cash/Components/AddCashEntryDialog.razor` | Component / layout |
| `Features/Cash/Components/CashSessionDetailDialog.razor` | Component / layout |
| `Features/Cash/Components/CloseCashSessionDialog.razor` | Component / layout |
| `Features/Cash/Components/OpenCashSessionDialog.razor` | Component / layout |
| `Features/Cash/Pages/CashSessions.razor` | `/cash/sessions` |

### Dashboard

| UI source | Routes |
|---|---|
| `Features/Dashboard/Components/AppointmentsTable.razor` | Component / layout |
| `Features/Dashboard/Components/DashboardSkeleton.razor` | Component / layout |
| `Features/Dashboard/Components/DoctorCard.razor` | Component / layout |
| `Features/Dashboard/Components/DoctorsList.razor` | Component / layout |
| `Features/Dashboard/Components/RevenueChart.razor` | Component / layout |
| `Features/Dashboard/Components/StatCard.razor` | Component / layout |
| `Features/Dashboard/Pages/Dashboard.razor` | `/dashboard` |

### Examinations

| UI source | Routes |
|---|---|
| `Features/Examinations/Components/ExaminationCalendar.razor` | `/examinations/calendar` |
| `Features/Examinations/Components/ExaminationTypeEditorDialog.razor` | Component / layout |
| `Features/Examinations/Components/ScheduleExamDialog.razor` | Component / layout |
| `Features/Examinations/Pages/Examinations.razor` | `/examinations` |

### Identity

| UI source | Routes |
|---|---|
| `Features/Identity/Components/RoleEditorDialog.razor` | Component / layout |
| `Features/Identity/Components/RolePermissionsDialog.razor` | Component / layout |
| `Features/Identity/Components/UserEditorDialog.razor` | Component / layout |
| `Features/Identity/Components/UserLockDialog.razor` | Component / layout |
| `Features/Identity/Components/UserRolesDialog.razor` | Component / layout |
| `Features/Identity/Pages/Roles.razor` | `/roles` |
| `Features/Identity/Pages/Users.razor` | `/users` |

### Insurance

| UI source | Routes |
|---|---|
| `Features/Insurance/Components/ClaimCreateDialog.razor` | Component / layout |
| `Features/Insurance/Components/ClaimDetailDialog.razor` | Component / layout |
| `Features/Insurance/Components/CompanyEditorDialog.razor` | Component / layout |
| `Features/Insurance/Components/PolicyDetailDialog.razor` | Component / layout |
| `Features/Insurance/Components/PolicyEditorDialog.razor` | Component / layout |
| `Features/Insurance/Components/PreAuthDetailDialog.razor` | Component / layout |
| `Features/Insurance/Components/PreAuthDialog.razor` | Component / layout |
| `Features/Insurance/Pages/Claims.razor` | `/insurance/claims` |
| `Features/Insurance/Pages/Companies.razor` | `/insurance/companies` |
| `Features/Insurance/Pages/InsurancePolicies.razor` | `/insurance/policies` |
| `Features/Insurance/Pages/PreAuthorizations.razor` | `/insurance/preauthorizations` |

### Inventory

| UI source | Routes |
|---|---|
| `Features/Inventory/Components/ItemEditorDialog.razor` | Component / layout |
| `Features/Inventory/Components/ItemStockDialog.razor` | Component / layout |
| `Features/Inventory/Components/PurchaseOrderEditorDialog.razor` | Component / layout |
| `Features/Inventory/Components/PurchaseOrderReceiveDialog.razor` | Component / layout |
| `Features/Inventory/Components/PurchaseOrderViewDialog.razor` | Component / layout |
| `Features/Inventory/Components/SupplierEditorDialog.razor` | Component / layout |
| `Features/Inventory/Pages/Inventory.razor` | `/inventory/items` |
| `Features/Inventory/Pages/PurchaseOrders/PurchaseOrders.razor` | `/inventory/purchase-orders` |
| `Features/Inventory/Pages/StockMovements.razor` | `/inventory/stock-movements` |
| `Features/Inventory/Pages/Suppliers.razor` | `/inventory/suppliers` |

### Notifications

| UI source | Routes |
|---|---|
| `Features/Notifications/Components/NotificationMessageDetailDialog.razor` | Component / layout |
| `Features/Notifications/Components/NotificationTemplateEditorDialog.razor` | Component / layout |
| `Features/Notifications/Components/SendNotificationDialog.razor` | Component / layout |
| `Features/Notifications/Pages/NotificationMessages.razor` | `/notifications/messages` |
| `Features/Notifications/Pages/NotificationTemplates.razor` | `/notifications/templates` |

### Patients

| UI source | Routes |
|---|---|
| `Features/Patients/Components/PatientEditorDialog.razor` | Component / layout |
| `Features/Patients/Pages/Patients.razor` | `/patients` |

### Payroll

| UI source | Routes |
|---|---|
| `Features/Payroll/Components/AddPayslipDialog.razor` | Component / layout |
| `Features/Payroll/Components/AllowanceEditorDialog.razor` | Component / layout |
| `Features/Payroll/Components/ExaminationFeeEditorDialog.razor` | Component / layout |
| `Features/Payroll/Components/PayRunDetailDialog.razor` | Component / layout |
| `Features/Payroll/Components/PayRunEditorDialog.razor` | Component / layout |
| `Features/Payroll/Components/PayslipTable.razor` | Component / layout |
| `Features/Payroll/Components/ReferralFeeEditorDialog.razor` | Component / layout |
| `Features/Payroll/Components/ReferralFeeTable.razor` | Component / layout |
| `Features/Payroll/Components/SalaryComponentEditorDialog.razor` | Component / layout |
| `Features/Payroll/Components/SalaryEditorDialog.razor` | Component / layout |
| `Features/Payroll/Pages/Allowances.razor` | `/payroll/allowances` |
| `Features/Payroll/Pages/ExaminationFees.razor` | `/payroll/examination-fees` |
| `Features/Payroll/Pages/PayRuns.razor` | `/payroll/payruns` |
| `Features/Payroll/Pages/ReferralFees.razor` | `/payroll/referral-fees` |
| `Features/Payroll/Pages/Salaries.razor` | `/payroll/salaries` |
| `Features/Payroll/Pages/SalaryComponents.razor` | `/payroll/salary-components` |

### ReadingRoom

| UI source | Routes |
|---|---|
| `Features/ReadingRoom/Components/AssignStaffDialog.razor` | Component / layout |
| `Features/ReadingRoom/Components/ReadingRoomFindingsList.razor` | Component / layout |
| `Features/ReadingRoom/Components/ReadingRoomReportEditor.razor` | Component / layout |
| `Features/ReadingRoom/Pages/ReadingRoom.razor` | `/radiologist` |

### Reports

| UI source | Routes |
|---|---|
| `Features/Reports/Components/AmendReportDialog.razor` | Component / layout |
| `Features/Reports/Components/CancelReportDialog.razor` | Component / layout |
| `Features/Reports/Components/ReportTemplatePickerDialog.razor` | Component / layout |
| `Features/Reports/Components/ReportVersionsDialog.razor` | Component / layout |

### Resources

| UI source | Routes |
|---|---|
| `Features/Resources/Components/EquipmentEditorDialog.razor` | Component / layout |
| `Features/Resources/Components/EquipmentStatusDialog.razor` | Component / layout |
| `Features/Resources/Components/LeaveEditorDialog.razor` | Component / layout |
| `Features/Resources/Components/ReferralDoctorEditorDialog.razor` | Component / layout |
| `Features/Resources/Components/StaffEditorDialog.razor` | Component / layout |
| `Features/Resources/Components/WorkShiftEditorDialog.razor` | Component / layout |
| `Features/Resources/Pages/Equipment.razor` | `/resources/equipment` |
| `Features/Resources/Pages/Leaves.razor` | `/resources/leaves` |
| `Features/Resources/Pages/ReferralDoctors.razor` | `/resources/referral-doctors` |
| `Features/Resources/Pages/Staff.razor` | `/resources/staff` |
| `Features/Resources/Pages/WorkShifts.razor` | `/resources/work-shifts` |

### Visits

| UI source | Routes |
|---|---|
| `Features/Visits/Components/CancelVisitDialog.razor` | Component / layout |
| `Features/Visits/Components/VisitEditorDialog.razor` | Component / layout |
| `Features/Visits/Components/VisitScheduleDialog.razor` | Component / layout |
| `Features/Visits/Components/VisitViewDialog.razor` | Component / layout |
| `Features/Visits/Pages/Visits.razor` | `/visits` |

### Shared

| UI source | Routes |
|---|---|
| `Shared/App.razor` | Component / layout |
| `Shared/Components/ActiveStatusChip.razor` | Component / layout |
| `Shared/Components/BootSplash.razor` | Component / layout |
| `Shared/Components/ChartDataTable.razor` | Component / layout |
| `Shared/Components/ClinicSettingsDialog.razor` | Component / layout |
| `Shared/Components/ClinicSetupPanel.razor` | Component / layout |
| `Shared/Components/ClinicUpdateDialog.razor` | Component / layout |
| `Shared/Components/ConfirmDialog.razor` | Component / layout |
| `Shared/Components/ConnectionBanner.razor` | Component / layout |
| `Shared/Components/DetailField.razor` | Component / layout |
| `Shared/Components/DialogButtons.razor` | Component / layout |
| `Shared/Components/DialogHeader.razor` | Component / layout |
| `Shared/Components/DialogSkeleton.razor` | Component / layout |
| `Shared/Components/EmptyState.razor` | Component / layout |
| `Shared/Components/ErrorState.razor` | Component / layout |
| `Shared/Components/ExcelImportDialog.razor` | Component / layout |
| `Shared/Components/LanguageDialog.razor` | Component / layout |
| `Shared/Components/LanguageSwitcher.razor` | Component / layout |
| `Shared/Components/PacsAccountDialog.razor` | Component / layout |
| `Shared/Components/PageHeader.razor` | Component / layout |
| `Shared/Components/RedirectToLogin.razor` | Component / layout |
| `Shared/Components/RowActions.razor` | Component / layout |
| `Shared/Components/StatusChip.razor` | Component / layout |
| `Shared/Components/TableNoRecords.razor` | Component / layout |
| `Shared/Layout/GlobalSearch.razor` | Component / layout |
| `Shared/Layout/LandingLayout.razor` | Component / layout |
| `Shared/Layout/MainLayout.razor` | Component / layout |
| `Shared/Layout/NavMenu.razor` | Component / layout |
| `Shared/Pages/Landing.razor` | `/` |
| `Shared/Pages/NotFound.razor` | `/not-found` |


# Radiology Center production readiness review

Original audit on 9 October 2026. The API/localhost architecture described in this historical audit has since been replaced by the approved in-process MAUI/shared-SQL architecture. This document preserves the findings and evidence **before repairs**. Subsequent source changes and verification are recorded in [the repair status](production-repair-status.md); follow [the LAN deployment guide](lan-deployment.md) for the intended clinic setup. The original review below identified release blockers and did not itself change application source.

## Review coverage

All 2,280 Git-tracked files were inventoried. Content scans covered 2,253 text files; 27 binary assets were inventoried. Application files contain 101,955 lines, excluding generated-file classification. The full inventory records paths, hashes, classifications and scan signals. This is full repository coverage for automated triage, with focused manual tracing of critical workflows; it is not a claim that every line of every file received a manual correctness proof.

Manual tracing covered API startup and configuration, shared persistence and events, all module mutation-handler summaries, authentication and permissions, calendar read/write flows, examination lifecycle, cash reconciliation, stock issue, report versioning, PACS linking, payroll boundaries, insurance documents, desktop service initialization and release packaging. Generated clients, migrations, resource files and repository support files received mechanical consistency and signal scans. Live desktop interaction, clean-machine installation, load testing and backup restoration remain unverified.

The findings below contain 28 grouped issues: 2 P0 blockers, 21 P1 issues and 5 P2 issues. Reproduced failures, direct source defects and concurrency risks are labeled separately. P0 means a critical release blocker, P1 requires correction before ordinary production use, and P2 requires remediation or an explicit acceptance decision.

## Validation results

The backend Release build passed. The Windows desktop Release build passed with 183 warnings. The full solution build and an initial test compilation exhausted local disk capacity; redundant generated integration-test build outputs were removed, preserving source and evidence. The first sandbox examination run failed on SQL TLS access and is superseded by the host run below.

Module results are raw observed outcomes, not independent bug counts. The identity suite changed the shared administrator and produced 43 login-lock failures; Insurance, Reports and Payroll then failed entirely during administrator login. Their feature behavior was not validated by those runs. Examinations has 26 failures in setup using the stale X-Ray modality value instead of XRay. Catalog printed a final result, but its TRX was truncated when the disk filled.

| Module | Passed | Failed | Interpretation |
| --- | ---: | ---: | --- |
| Examinations | 7 | 26 | 26 failures in type-creation setup; custom probes cover key real failures |
| Cash | 26 | 0 | Sequential suite passes; does not cover identified races |
| Identity | 21 | 55 | 43 locked-login failures and additional fixture or contract errors |
| Insurance | 0 | 36 | All blocked at login with HTTP 423 |
| Inventory | 36 | 9 | Purchase-order and stock paths fail; includes contract errors |
| Notification | 20 | 1 | One validation expectation fails |
| Patients | 12 | 11 | Creation readback failures corroborate missing persistence |
| Payroll | 0 | 57 | All blocked at login with HTTP 423 |
| Reports | 0 | 55 | All blocked at login with HTTP 423 |
| ResourceManagement | 52 | 1 | One contract or validation expectation fails |
| Catalog | 21 | 9 | Console result only; TRX incomplete |

### API probe evidence

| Operation | Observed result |
| --- | --- |
| Create patient then read it | HTTP 200 then HTTP 404 |
| Create examination type then read it | HTTP 200 then HTTP 404 |
| Book with missing patient and random equipment IDs | HTTP 200 and persisted appointment |
| Record payment then read examination | HTTP 200; Paid still zero; cash insert present in SQL log |
| Cancel then read examination | HTTP 200; status still Scheduled |
| Calendar for multiple patients | HTTP 500 and concurrent DbContext exception |
| Query local workday after booking at 16:00 Cairo | HTTP 200 with an empty result |
| Negotiate notifications without a token | HTTP 200 |

Evidence files are probe-evidence.json and probe-runtime.log. The probe used synthetic data; login tokens were removed from saved evidence. The unbounded interval case was not executed.

### Database side effects and recovery

The test fixture override failed to isolate the application connection. A startup inspection that deliberately aborted before SQL access resolved the EF database to RadiologyCenter. A subsequent read-only check found the known audit booking there. Testing inserted synthetic data and affected the shared seeded administrator. Further mutation tests were stopped. An attempted transaction to remove the six probe bookings, their two catalog fixtures and linked cash entries, and restore pre-audit administrator access was rejected by automatic approval review; explicit user approval is pending. No recovery transaction has run.

The existing CodexReview databases have different seeded identities and empty baseline module tables. They must not be treated as verified backups of the configured database or used for a blanket restore. Other integration-test-created records require individual identification before cleanup. database-impact-comparison.log contains counts only; it is not a pre-audit baseline proof. Preserve the database while recovery is reviewed.

## Findings and corrective work

### Finding 1 Transaction commits omit saving tracked changes

**P0 — Reproduced and source confirmed.** Ten mutation handlers call CommitAsync without SaveChangesAsync. CommitAsync only commits SQL; repository additions and entity changes remain in the EF change tracker. Patient and examination-type creation returned HTTP 200, followed immediately by HTTP 404 on readback. A payment returned HTTP 200 and inserted a cash entry while Paid remained zero; cancellation returned HTTP 200 while status remained Scheduled. This can collect the same debt repeatedly and corrupt reconciliation. The other handlers with this pattern are patient import, type import, purchase-order creation, purchase-order receipt, stock issue and claim settlement.

**Corrective work.** Make saving and committing explicit and consistent. Save every enlisted module context before commit and verify rollback across cash and examinations. Add readback assertions and failure-injection tests for every affected handler. Wolverine EF transaction integration alone does not opt handlers into transactional middleware; its documented opt-in is an attribute or AutoApplyTransactions.

**Source.** [UnitOfWork.cs line 56](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/BuildingBlocks/RadiologyCenter.BuildingBlocks.Infrastructure/Persistence/UnitOfWork.cs:56>); [RecordExaminationPaymentCommandHandler.cs line 42](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Commands/RecordExaminationPayment/RecordExaminationPaymentCommandHandler.cs:42>); [CreatePatientCommandHandler.cs line 49](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Patients/RadiologyCenter.Patients.Application/Commands/CreatePatient/CreatePatientCommandHandler.cs:49>).

The middleware distinction is documented in [Wolverine EF Core transactional middleware](https://wolverinefx.net/guide/durability/efcore/transactional-middleware).

### Finding 2 Shipped bootstrap credentials and JWT signing key grant full access

**P0 — Source confirmed.** The seeder always creates the administrator with the known default admin123 credential and restores its administrator role. It does not consume the Seed credentials in appsettings, and startup always runs it. A shared symmetric JWT signing key is shipped in configuration; anyone who can read the packaged backend can sign administrator claims. A credential setting in configuration currently gives a misleading impression that changing it changes bootstrap behavior.

**Corrective work.** Replace default seeding with controlled first-run provisioning, require a password change, and fail startup when production secrets are missing or known defaults are present. Rotate the signing key. Choose an architecture where privileged signing material belongs to a controlled backend host, rather than every ordinary workstation installation.

**Source.** [IdentityDbSeeder.cs line 13](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Identity/RadiologyCenter.Identity.Infrastructure/Persistence/Seed/IdentityDbSeeder.cs:13>); [DatabaseExtensions.cs line 39](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Localhost/RadiologyCenter.Localhost/Extensions/DatabaseExtensions.cs:39>); [TokenService.cs line 60](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Identity/RadiologyCenter.Identity.Infrastructure/Services/TokenService.cs:60>); [appsettings.json line 21](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Localhost/RadiologyCenter.Localhost/appsettings.json:21>).

### Finding 3 Role creation can manufacture an unrestricted administrator role

**P1 — Source confirmed.** CreateRole accepts a client-supplied IsSystem flag under Roles.Create permission. TokenService treats any assigned IsSystem role as administrator and emits every permission. CreateUser accepts arbitrary existing RoleIds under Users.Create, without the separate role-management permission. Delegated user or role management can therefore cross the intended permission boundary and create an account with administrator access.

**Corrective work.** Reserve administrator provisioning and assignment for an explicit server-side administrator policy. Remove IsSystem from normal client input, distinguish system-managed roles from administrator privileges, and enforce role-assignment authorization during user creation as well as dedicated assignment endpoints.

**Source.** [CreateRoleCommand.cs line 3](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Identity/RadiologyCenter.Identity.Application/Commands/CreateRole/CreateRoleCommand.cs:3>); [CreateRoleCommandHandler.cs line 14](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Identity/RadiologyCenter.Identity.Application/Commands/CreateRole/CreateRoleCommandHandler.cs:14>); [TokenService.cs line 48](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Identity/RadiologyCenter.Identity.Infrastructure/Services/TokenService.cs:48>); [CreateUserCommandHandler.cs line 27](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Identity/RadiologyCenter.Identity.Application/Commands/CreateUser/CreateUserCommandHandler.cs:27>).

### Finding 4 Anonymous notification connections can receive patient information

**P1 — Anonymous negotiation reproduced and source confirmed.** The notification hub explicitly allows anonymous access, accepts arbitrary group names, and broadcasts to Clients.All. Checked-in notifications include patient identity and clinical indication. An unauthenticated negotiation returned HTTP 200. CORS also accepts any origin when the configured list is empty, including the default configuration. Localhost binding does not provide application-level access control against other local processes or browser origins.

**Corrective work.** Authenticate the hub, supply the desktop access token, authorize subscriptions on the server, and send clinical payloads only to permitted users or groups. Configure a concrete origin allowlist and fail closed when it is absent.

**Source.** [Program.cs line 181](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Localhost/RadiologyCenter.Localhost/Program.cs:181>); [NotificationService.cs line 17](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/BuildingBlocks/RadiologyCenter.BuildingBlocks.Infrastructure/RealTime/NotificationService.cs:17>); [NotificationHub.cs line 7](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/BuildingBlocks/RadiologyCenter.BuildingBlocks.Infrastructure/RealTime/NotificationHub.cs:7>); [ExaminationCheckedInEventHandler.cs line 1](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Events/ExaminationCheckedInEventHandler.cs:1>).

### Finding 5 Orthanc permits remote unauthenticated HTTP access

**P1 — Source confirmed with network reachability condition.** The generated Orthanc configuration sets AuthenticationEnabled=false and RemoteAccessAllowed=true on HTTP port 18042. If that port is reachable, the application supplies no credential boundary for DICOM studies or Orthanc management operations. The backend being bound to localhost does not constrain this separate service.

**Corrective work.** Restrict the Orthanc HTTP management interface to loopback or an authenticated controlled endpoint. Define authenticated modality access separately and verify the actual listener and firewall rules on a clean installation.

**Source.** [PacsService.cs line 244](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Desktop/RadiologyCenter.Desktop/Services/PacsService.cs:244>); [PacsService.cs line 245](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Desktop/RadiologyCenter.Desktop/Services/PacsService.cs:245>).

### Finding 6 Calendar booking cannot submit its current form

**P1 — Source confirmed without live desktop interaction.** The submit button is in DialogActions outside EditForm and has neither a form association nor an OnClick callback. Independently, EditContext validates ScheduleExamFormModel, whose required ScheduledDate and ScheduledTime remain null: controls bind separate backing fields. Even a direct SubmitAsync invocation fails validation. The Emergency priority value also differs from the backend enum Stat and will be rejected after the form is repaired.

**Corrective work.** Bind controls to the validated form model and place or explicitly associate the submit button with the form. Display field validation. Generate priority options from the shared contract. Verify actual dialog submission in the packaged MAUI app.

**Source.** [ScheduleExamDialog.razor line 11](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Desktop/RadiologyCenter.Desktop/Features/Examinations/Components/ScheduleExamDialog.razor:11>); [ScheduleExamDialog.razor line 115](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Desktop/RadiologyCenter.Desktop/Features/Examinations/Components/ScheduleExamDialog.razor:115>); [ScheduleExamDialog.razor.cs line 47](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Desktop/RadiologyCenter.Desktop/Features/Examinations/Components/ScheduleExamDialog.razor.cs:47>); [FormModels.cs line 34](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Desktop/RadiologyCenter.Desktop/Features/Examinations/Models/FormModels.cs:34>).

### Finding 7 Clicking a calendar row loses its equipment identity

**P1 — Source confirmed.** ResourceRow and SlotCell carry no equipment ID. OnSlotClick fills the dialog from the global equipment filter, rather than the clicked row. In the all-equipment view, that filter is null, so the booking receives no equipment and then disappears because resource rows exclude unassigned examinations. The clicked row modality is also lost, allowing the wrong type to be selected for a machine.

**Corrective work.** Carry resource ID and modality into every slot click, use them to prefill booking, and validate compatibility on the backend. Provide an intentional representation for unassigned examinations.

**Source.** [ExaminationCalendar.razor.cs line 151](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Desktop/RadiologyCenter.Desktop/Features/Examinations/Components/ExaminationCalendar.razor.cs:151>); [ExaminationCalendar.razor.cs line 232](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Desktop/RadiologyCenter.Desktop/Features/Examinations/Components/ExaminationCalendar.razor.cs:232>); [ExaminationCalendar.razor.cs line 138](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Desktop/RadiologyCenter.Desktop/Features/Examinations/Components/ExaminationCalendar.razor.cs:138>).

### Finding 8 Calendar reads and writes use incompatible time conventions

**P1 — Reproduced and source confirmed.** Calendar day bounds are converted from workstation-local time to UTC. Booking persists the received ScheduledAt without equivalent normalization, and SQL readback loses the offset. A saved 16:00 Cairo booking was absent from the UTC query range for the same local 08:00–17:00 workday. Availability, past-date validation and the current-time indicator also use different local, user and UTC assumptions.

**Corrective work.** Define one clinic-time contract. Convert at explicit boundaries, persist UTC instants consistently, and display them in the configured clinic zone. Apply the same convention to create, book, reschedule, availability, analytics and payroll; test Cairo DST boundaries and workstations in other zones.

**Source.** [ExaminationCalendar.razor.cs line 71](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Desktop/RadiologyCenter.Desktop/Features/Examinations/Components/ExaminationCalendar.razor.cs:71>); [BookExaminationCommandHandler.cs line 77](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Commands/BookExamination/BookExaminationCommandHandler.cs:77>); [UpdateExaminationCommandHandler.cs line 95](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Commands/UpdateExamination/UpdateExaminationCommandHandler.cs:95>).

### Finding 9 Calendar patient lookup executes parallel queries on one DbContext

**P1 — Reproduced HTTP 500.** GetExaminationsForCalendar uses Task.WhenAll for patient resolution through scoped services backed by one PatientsDbContext. A calendar containing two distinct patients returned HTTP 500; the log reports a second operation started on the same context. Empty-calendar tests cannot detect this failure.

**Corrective work.** Fetch patient identities in one batched query, or await queries sequentially. Separate contexts are another option if concurrency is necessary. EF Core explicitly does not support concurrent operations on one DbContext.

**Source.** [GetExaminationsForCalendarQueryHandler.cs line 41](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Queries/GetExaminationsForCalendar/GetExaminationsForCalendarQueryHandler.cs:41>); [PatientInfoResolver.cs line 1](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Adapters/PatientInfoResolver.cs:1>).

See [EF Core guidance on avoiding DbContext threading issues](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/#avoiding-dbcontext-threading-issues).

### Finding 10 Invalid availability intervals can run an unbounded loop

**P1 — Source confirmed and intentionally not executed.** The availability endpoint accepts an integer interval with no positive-range validation. The handler advances current by that interval inside a while loop. Zero never advances, and negative values move backward while continually appending slots. Cancellation is not checked inside the loop. An authorized malformed request can consume memory and block the local API.

**Corrective work.** Validate a positive bounded interval and a bounded date range before querying. Add cancellation checks and a maximum generated-slot count. Verify zero, negative, excessive and normal intervals without running the current unbounded code.

**Source.** [GetAvailableSlotsQueryHandler.cs line 32](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Queries/GetAvailableSlots/GetAvailableSlotsQueryHandler.cs:32>); [ExaminationsController.cs line 201](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Localhost/RadiologyCenter.Localhost/Controllers/Examinations/ExaminationsController.cs:201>).

### Finding 11 Scheduling paths disagree on duration and conflict enforcement

**P1 — Source confirmed.** Book only checks overlap when EquipmentId is supplied, bypassing radiologist-only conflicts. General examination creation and update call Schedule without duration, making ScheduledEnd null; range queries require an end and omit those appointments. Update checks a fixed 30-minute interval rather than the actual examination duration, while staff reassignment does not recheck conflicts. A type with zero duration uses 30 minutes for conflict checking but persists no end.

**Corrective work.** Use a single scheduling operation with positive duration and consistent status, resource and interval rules. Route all create, book, update, reschedule and assignment paths through it. Backfill existing scheduled records with missing ends before enabling the calendar.

**Source.** [BookExaminationCommandHandler.cs line 35](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Commands/BookExamination/BookExaminationCommandHandler.cs:35>); [CreateExaminationCommandHandler.cs line 66](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Commands/CreateExamination/CreateExaminationCommandHandler.cs:66>); [UpdateExaminationCommandHandler.cs line 135](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Commands/UpdateExamination/UpdateExaminationCommandHandler.cs:135>); [Examination.cs line 203](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Domain/Entities/Examination.cs:203>).

### Finding 12 Concurrent booking can reserve the same resource twice

**P1 — Concurrency risk established by source flow and not load reproduced.** Conflict detection reads existing appointments and then inserts a new appointment without locking a stable resource row or using serializable range protection. Two concurrent clients can both observe a free interval and both save. RowVersion protects updates to existing rows and cannot prevent two independent inserts.

**Corrective work.** Serialize booking per resource using database locks or an equivalent transactional reservation mechanism, with a clear conflict response. Cover simultaneous bookings, reschedules and staff assignments in a real SQL Server concurrency test.

**Source.** [BookExaminationCommandHandler.cs line 37](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Commands/BookExamination/BookExaminationCommandHandler.cs:37>); [BookExaminationCommandHandler.cs line 80](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Commands/BookExamination/BookExaminationCommandHandler.cs:80>); [ExaminationOverlapChecker.cs line 1](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Scheduling/ExaminationOverlapChecker.cs:1>).

### Finding 13 Bookings accept nonexistent patient and equipment identifiers

**P1 — Reproduced and source confirmed.** The probe booked an examination using the patient ID returned by the failed patient creation and a random nonexistent equipment ID. The appointment persisted successfully. The handler resolves the examination type but does not verify patient existence, equipment existence, active state, machine modality or staff eligibility.

**Corrective work.** Validate cross-module references and resource eligibility through explicit directories before saving. Validate again for reassignment and patient changes, and return actionable errors for inactive or incompatible records.

**Source.** [BookExaminationCommandHandler.cs line 53](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Commands/BookExamination/BookExaminationCommandHandler.cs:53>); [UpdateExaminationCommandHandler.cs line 26](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Commands/UpdateExamination/UpdateExaminationCommandHandler.cs:26>).

### Finding 14 The reading worklist silently drops records beyond page one

**P1 — Source confirmed.** ReadingRoom loads the first 100 examinations across all statuses and only then filters CheckedIn in memory. Checked-in examinations outside that first page never appear. It also reads only the first 100 reports for badges, so existing reports can look new. The current queue excludes InProgress and Completed entirely, which needs reconciliation with the intended reading and reporting workflow.

**Corrective work.** Filter eligible workflow states on the server before pagination, provide paging or a purpose-built worklist, and retrieve report status for the visible examination IDs. Test a dataset above 100 records and the full checked-in to reporting lifecycle.

**Source.** [ReadingRoom.Queue.cs line 38](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Desktop/RadiologyCenter.Desktop/Features/ReadingRoom/Pages/ReadingRoom.Queue.cs:38>); [ReadingRoom.Queue.cs line 40](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Desktop/RadiologyCenter.Desktop/Features/ReadingRoom/Pages/ReadingRoom.Queue.cs:40>).

### Finding 15 Calendar selectors cannot find patients beyond the first 100

**P1 — Source confirmed.** ScheduleExamDialog requests 500 patients and types but the global maximum page size is 100. Searches operate only on that locally loaded first page. Calendar equipment loading requests 200 and has the same cap. Filtering active records after pagination can shrink these lists further and omit valid patients, types or equipment.

**Corrective work.** Use server-side autocomplete with active and modality filters before pagination. Load every equipment page when the calendar requires every resource, and honor total count rather than relying on oversized page requests.

**Source.** [ScheduleExamDialog.razor.cs line 55](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Desktop/RadiologyCenter.Desktop/Features/Examinations/Components/ScheduleExamDialog.razor.cs:55>); [PaginationParams.cs line 5](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/BuildingBlocks/RadiologyCenter.BuildingBlocks.Domain/Pagination/PaginationParams.cs:5>); [ExaminationCalendar.razor.cs line 52](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Desktop/RadiologyCenter.Desktop/Features/Examinations/Components/ExaminationCalendar.razor.cs:52>).

### Finding 16 PACS linking can attach the wrong examination or patient study

**P1 — Source confirmed with multiple-candidate condition.** ReadingRoom passes no accession number when linking images. PacsSync selects the newest study for the patient when no accession matches, regardless of examination date or modality. With no patient-code match it falls back to normalized patient name, which is not a unique identifier. Multiple examinations can receive the same unrelated study, and duplicate names can cross patient boundaries.

**Corrective work.** Require verified patient identity and examination accession matching, check modality and study metadata, and show a selection or refusal when candidates are ambiguous. Do not silently attach the newest study or a same-name patient study.

**Source.** [ReadingRoom.Queue.cs line 140](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Desktop/RadiologyCenter.Desktop/Features/ReadingRoom/Pages/ReadingRoom.Queue.cs:140>); [PacsSyncService.cs line 63](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Desktop/RadiologyCenter.Desktop/Services/PacsSyncService.cs:63>); [PacsSyncService.cs line 100](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Desktop/RadiologyCenter.Desktop/Services/PacsSyncService.cs:100>).

### Finding 17 Report creation trusts a patient ID unrelated to the examination

**P1 — Source confirmed.** CreateReportDraft verifies examination status but copies caller-supplied PatientId and RadiologistId directly into the clinical report. No check ties the report patient to the examination patient. The same mismatch risk applies to radiologist assignment and authorization.

**Corrective work.** Resolve the examination patient and responsible clinician on the server. Derive immutable report identity from that record and enforce clinical author permissions. Test mismatched and nonexistent identifiers.

**Source.** [CreateReportDraftCommandHandler.cs line 22](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Reports/RadiologyCenter.Reports.Application/Commands/CreateReportDraft/CreateReportDraftCommandHandler.cs:22>); [RadiologyReport.cs line 42](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Reports/RadiologyCenter.Reports.Domain/Entities/RadiologyReport.cs:42>).

### Finding 18 Concurrent report editing can alter finalized content

**P1 — Concurrency risk established by source flow and not race reproduced.** Report editability is checked on the loaded Draft aggregate, but reports and child content have no concurrency token. An editor can load a draft, another request can finalize it, and the first request can subsequently save changed sections into the finalized version. Concurrent amendments and content updates also lack version conflict protection.

**Corrective work.** Introduce report-wide revision checks that cover child changes, and enforce draft state in the same write transaction. Return a conflict when another editor or finalizer has advanced the revision; test finalize-versus-edit and simultaneous amendments.

**Source.** [RadiologyReport.cs line 71](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Reports/RadiologyCenter.Reports.Domain/Entities/RadiologyReport.cs:71>); [RadiologyReportConfiguration.cs line 20](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Reports/RadiologyCenter.Reports.Infrastructure/Persistence/Configurations/RadiologyReportConfiguration.cs:20>).

### Finding 19 Payment balances can be edited without corresponding cash entries

**P1 — Source confirmed.** CreateExamination accepts initial Paid and UpdateExamination calls SetBilling with arbitrary Paid. Those paths bypass the cash recorder used by RecordExaminationPayment. A user with examination creation or update permission can alter financial balances without a corresponding payment or refund ledger entry, even after the missing-save defect is fixed.

**Corrective work.** Route payment changes through authorized financial commands and immutable ledger entries. Derive or reconcile examination balances from those entries, and reserve discount adjustments for an explicit permission with an audit reason.

**Source.** [UpdateExaminationCommandHandler.cs line 55](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Commands/UpdateExamination/UpdateExaminationCommandHandler.cs:55>); [CreateExaminationCommandHandler.cs line 47](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Commands/CreateExamination/CreateExaminationCommandHandler.cs:47>); [RecordExaminationPaymentCommandHandler.cs line 33](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Commands/RecordExaminationPayment/RecordExaminationPaymentCommandHandler.cs:33>).

### Finding 20 Expired inventory batches remain eligible for issue

**P1 — Source confirmed and masked by the current missing-save defect.** Available-batch queries select only positive remaining quantity and order by earliest expiry, without excluding expired batches. StockBatch.Issue validates quantity but not expiry. A batch valid at receipt can expire later and then be selected first for issue, including medical consumables.

**Corrective work.** Exclude expired or quarantined stock using a defined clinic date rule, retain expiry checks in the issuing domain operation, and expose expired quantity separately. Test stock that becomes expired after receipt.

**Source.** [StockBatchRepository.cs line 21](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Inventory/RadiologyCenter.Inventory.Infrastructure/Repositories/StockBatchRepository.cs:21>); [StockBatch.cs line 46](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Inventory/RadiologyCenter.Inventory.Domain/Entities/StockBatch.cs:46>).

### Finding 21 Cash entries can race with session closure

**P1 — Concurrency risk established by source flow and not race reproduced.** AddCashEntry reads an open session and inserts an entry without locking or updating that session. CloseCashSession computes movements and closes it separately. An entry can commit after the handover was calculated, invalidating the closed total. Auto-opened sessions and handover successors also check for an existing open session without a unique database constraint, allowing simultaneous requests to create multiple open sessions.

**Corrective work.** Lock a stable session or user reservation within the entry and close transactions, and enforce one open session per user in the database. Verify entry-versus-close and simultaneous auto-open races.

**Source.** [AddCashEntryCommandHandler.cs line 28](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Cash/RadiologyCenter.Cash.Application/Commands/Sessions/AddCashEntry/AddCashEntryCommandHandler.cs:28>); [CloseCashSessionCommandHandler.cs line 33](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Cash/RadiologyCenter.Cash.Application/Commands/Sessions/CloseCashSession/CloseCashSessionCommandHandler.cs:33>); [CashSessionConfiguration.cs line 29](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Cash/RadiologyCenter.Cash.Infrastructure/Persistence/Configurations/CashSessionConfiguration.cs:29>); [PaymentCashEntryRecorder.cs line 84](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Localhost/RadiologyCenter.Localhost/Extensions/PaymentCashEntryRecorder.cs:84>).

### Finding 22 Integration test configuration does not isolate the database

**P1 — Resolved EF target and database side effects reproduced.** CustomWebApplicationFactory declares RadiologyCenter_IntegrationTests through ConfigureAppConfiguration, but module registration captures the connection before that override applies. A probe that aborted before SQL access printed Captured EF database: RadiologyCenter. A read-only SQL check located the synthetic booking in RadiologyCenter and not the existing review databases. Mutation tests must therefore be considered capable of modifying the application database. Identity tests also change and lock the shared administrator, cascading HTTP 423 failures into later suites.

**Corrective work.** Override DbContext registrations themselves or provide configuration before application service registration. Require a unique test-only database name and assert the resolved connection before opening it. Use independent accounts for password and lockout tests, reset fixtures deterministically, and refuse any destructive test cleanup against the application database.

**Source.** [CustomWebApplicationFactory.cs line 17](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/BuildingBlocks/RadiologyCenter.IntegrationTests.Shared/CustomWebApplicationFactory.cs:17>); [IdentityInfrastructureRegistration.cs line 26](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Identity/RadiologyCenter.Identity.Infrastructure/IdentityInfrastructureRegistration.cs:26>); [TestBase.cs line 22](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/BuildingBlocks/RadiologyCenter.IntegrationTests.Shared/TestBase.cs:22>); [AuthTests.cs line 133](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Identity/RadiologyCenter.Identity.IntegrationTests/AuthTests.cs:133>).

### Finding 23 Payroll excludes completion fees on the selected final day

**P2 — Source confirmed.** PayRunEditor submits RunTo at midnight on the last selected date. Completion projection queries use CompletedAt <= to, excluding examinations completed later that final day. ComputePayRun passes the same range to payroll calculators. This differs from the inclusive date-range expectation of the editor and can underpay fees.

**Corrective work.** Represent payroll dates as dates and translate them to a clinic-zone interval with an exclusive start of the following day. Test an examination at noon on the final day and around timezone changes. Confirm proration and mid-period salary rules separately with the business owner.

**Source.** [PayRunEditorDialog.razor.cs line 54](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Desktop/RadiologyCenter.Desktop/Features/Payroll/Components/PayRunEditorDialog.razor.cs:54>); [ExaminationRepository.cs line 72](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Infrastructure/Repositories/ExaminationRepository.cs:72>); [ComputePayRunCommandHandler.cs line 30](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Payroll/RadiologyCenter.Payroll.Application/Commands/ComputePayRun/ComputePayRunCommandHandler.cs:30>).

### Finding 24 Insurance upload changes data using a read permission

**P2 — Source confirmed.** UploadDocument is protected by InsurancePolicies.Read rather than a write or upload permission. A read-only policy user can add files and consume storage. Delete correctly uses a deletion permission, so the upload policy is inconsistent with the surrounding API.

**Corrective work.** Protect upload with an explicit mutation permission and verify read-only users can view or download but cannot upload. Apply equivalent checks to every document endpoint.

**Source.** [InsurancePoliciesController.cs line 102](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Localhost/RadiologyCenter.Localhost/Controllers/Insurance/InsurancePoliciesController.cs:102>).

### Finding 25 Insurance document storage is tied to the installation working directory

**P1 — Source confirmed with upgrade and launch-location condition.** Document storage resolves a relative App_Data/Insurance directory from the process working directory. The packaged backend runs from the desktop installation, so changing launch location or replacing a version directory can change the storage root while database references remain unchanged. Filesystem writes and deletes also occur separately from database persistence, allowing orphaned or missing documents after failures.

**Corrective work.** Use a stable configured data directory outside versioned installation folders, define access permissions, and back up files together with SQL and PACS data. Add compensating file operations and verify upgrade, interrupted upload/delete and restore workflows.

**Source.** [InsuranceDocumentStorage.cs line 12](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Insurance/RadiologyCenter.Insurance.Infrastructure/Services/InsuranceDocumentStorage.cs:12>); [appsettings.json line 17](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Localhost/RadiologyCenter.Localhost/appsettings.json:17>).

### Finding 26 Clean installation prerequisites and readiness are incomplete

**P2 — Source confirmed and clean-machine install not executed.** The release packages desktop and backend, but the default SQL connection requires a separately installed SQL Server and startup permissions to migrate every schema. PACS requires Orthanc from a bundle, environment path or Program Files; the release workflow does not supply it. Health checks register no dependency checks, so the health endpoint cannot establish database readiness after startup. README supplies no installation, backup or recovery procedure.

**Corrective work.** Define the workstation and shared-server topology, provision SQL and PACS dependencies, separate migration privileges from routine use where practical, and add operational documentation. Verify a signed installer on a clean Windows machine and a full backup/restore including attachments and DICOM data.

**Source.** [release.yml line 52](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/.github/workflows/release.yml:52>); [PacsService.cs line 203](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Desktop/RadiologyCenter.Desktop/Services/PacsService.cs:203>); [Program.cs line 85](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Localhost/RadiologyCenter.Localhost/Program.cs:85>); [README.md line 1](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/README.md:1>).

### Finding 27 Real-time reading-room updates use the wrong default endpoint

**P2 — Source confirmed.** ReadingRoom defaults SignalR to localhost:5000 while the bundled API uses localhost:5224. Errors are swallowed. RealTimeNotificationService leaves a failed connection allocated so StartAsync cannot retry, and repeated On registrations create duplicate callbacks without a component unsubscribe path. Navigation and restart can therefore leave stale or duplicate queue updates.

**Corrective work.** Share endpoint configuration with normal API requests, authenticate connections, recover failed initial starts, and return disposable subscriptions. Dispatch component updates through the Blazor synchronization context and test navigation, reconnect and backend restart.

**Source.** [ReadingRoom.razor.cs line 132](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Desktop/RadiologyCenter.Desktop/Features/ReadingRoom/Pages/ReadingRoom.razor.cs:132>); [RealTimeNotificationService.cs line 15](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Desktop/RadiologyCenter.Desktop/Services/RealTimeNotificationService.cs:15>); [RealTimeNotificationService.cs line 51](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Desktop/RadiologyCenter.Desktop/Services/RealTimeNotificationService.cs:51>).

### Finding 28 The release workflow can publish without validating application behavior

**P2 — Source confirmed.** Release CI restores and publishes but runs no integration tests. Manual workflow dispatch derives a package and assembly version from the branch name, which is invalid on ordinary branches such as main. Desktop compilation succeeds with 183 warnings, including six unexpected RightContent Razor elements and unsupported MudBlazor parameters. Calendar date and slot interaction uses clickable divs without keyboard controls.

**Corrective work.** Require a validated semantic version input for manual releases, gate packaging on isolated tests and representative workflow checks, and resolve actionable Razor and component warnings. Use semantic buttons or keyboard handlers with accessible names for calendar interactions.

**Source.** [release.yml line 34](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/.github/workflows/release.yml:34>); [ExaminationCalendar.razor line 95](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Desktop/RadiologyCenter.Desktop/Features/Examinations/Components/ExaminationCalendar.razor:95>).

## Missing save audit

These ten handlers use the commit-without-save pattern. The runtime probes verified four corresponding operations; the others share the same persistence failure mechanism.

- [CreateExaminationTypeCommandHandler.cs line 1](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Catalog/RadiologyCenter.Catalog.Application/Commands/CreateExaminationType/CreateExaminationTypeCommandHandler.cs:1>)

- [ImportExaminationTypesCommandHandler.cs line 1](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Catalog/RadiologyCenter.Catalog.Application/Commands/ImportExaminationTypes/ImportExaminationTypesCommandHandler.cs:1>)

- [CancelExaminationCommandHandler.cs line 1](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Commands/CancelExamination/CancelExaminationCommandHandler.cs:1>)

- [RecordExaminationPaymentCommandHandler.cs line 1](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Examinations/RadiologyCenter.Examinations.Application/Commands/RecordExaminationPayment/RecordExaminationPaymentCommandHandler.cs:1>)

- [RecordClaimSettlementCommandHandler.cs line 1](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Insurance/RadiologyCenter.Insurance.Application/Commands/Claims/RecordClaimSettlement/RecordClaimSettlementCommandHandler.cs:1>)

- [CreatePurchaseOrderCommandHandler.cs line 1](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Inventory/RadiologyCenter.Inventory.Application/Commands/CreatePurchaseOrder/CreatePurchaseOrderCommandHandler.cs:1>)

- [IssueStockCommandHandler.cs line 1](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Inventory/RadiologyCenter.Inventory.Application/Commands/IssueStock/IssueStockCommandHandler.cs:1>)

- [ReceivePurchaseOrderCommandHandler.cs line 1](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Inventory/RadiologyCenter.Inventory.Application/Commands/ReceivePurchaseOrder/ReceivePurchaseOrderCommandHandler.cs:1>)

- [CreatePatientCommandHandler.cs line 1](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Patients/RadiologyCenter.Patients.Application/Commands/CreatePatient/CreatePatientCommandHandler.cs:1>)

- [ImportPatientsCommandHandler.cs line 1](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/src/Backend/Modules/Patients/RadiologyCenter.Patients.Application/Commands/ImportPatients/ImportPatientsCommandHandler.cs:1>)

## Required follow-up verification

Domain-event delivery needs a focused integration test. SavingChanges publishes through IDbContextOutbox, while FlushAsync exists without a call site and handler transaction opt-in is absent. Check-in notifications and completion fee resolution depend on those events. This is an unresolved delivery concern, rather than a reproduced claim that every event is lost. Verify commit, rollback, process interruption and replay without duplicate financial effects.

Payroll proration, mid-period salary changes, allowance effective dates, booking hours, breaks, staff leave and modality eligibility need explicit business rules. The current calendar hardcodes 08:00–17:00 and 30-minute cells; correct scheduling must account for actual examination durations and applicable resource availability. Calendar responsive layout, keyboard interaction and validation visibility require live desktop verification after the functional defects are fixed.

Resource leave and shift checks and pay-run overlap checks use read-then-write validation. Revisit them with the booking and cash concurrency work; sequential passing tests do not prove protection against simultaneous requests. Report author ownership, finalized output and examination lifecycle need a complete clinical acceptance run once draft identity and revision protection are repaired.

## Repair order before deployment

1. Resolve the database side effects and fix test isolation before executing more mutation tests.

2. Repair explicit saves and cross-module financial transactions. Add persisted readback and rollback checks to the ten affected commands.

3. Replace insecure bootstrap secrets, close the administrator permission bypass, and secure notification and PACS access.

4. Complete the calendar form and resource binding, standardize time and duration, batch patient lookup, and validate intervals and resource eligibility.

5. Protect concurrent bookings, report finalization and cash closure. Repair worklist paging, PACS study matching, report identity and expired stock handling.

6. Fix payroll date ranges and durable document storage, then run the clinical and financial workflow against isolated realistic data above 100 records.

7. Validate a clean Windows installation, update, dependency outage and complete restore before packaging a production release.

## Evidence files

- [file-inventory.json](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/audit-results/file-inventory.json>)

- [handler-summary.json](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/audit-results/handler-summary.json>)

- [findings.json](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/audit-results/findings.json>)

- [test-summary.json](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/audit-results/test-summary.json>)

- [probe-evidence.json](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/audit-results/probe-evidence.json>)

- [probe-runtime.log](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/audit-results/probe-runtime.log>)

- [test-target-inspection.log](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/audit-results/test-target-inspection.log>)

- [database-target-evidence.log](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/audit-results/database-target-evidence.log>)

- [database-impact-comparison.log](<C:/Users/Ahmed Abdou/source/repos/radiology-center-management-system/audit-results/database-impact-comparison.log>)

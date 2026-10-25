# Live Windows walkthrough — 25 October 2026

## Scope

Actual MAUI/Blazor Windows application interaction on the development PC, using the current source and the existing `RadiologyCenter` database. This was a read-only clinical smoke test, not a complete clinic acceptance test. Normal sign-in creates authentication/session records; no patients, examinations, equipment, reports, financial entries or accounts were created, edited, reset or deleted.

The first Release launch had no saved clinic connection and correctly opened setup. The Debug build used the existing private development configuration. Credentials provided by the user were entered only into the local sign-in form and are not recorded here.

## Observed environment

| Check | Result |
|---|---|
| Development SQL target | Default instance on `DESKTOP-PL8IF8N`, database `RadiologyCenter` |
| Database | ONLINE; PARTIAL containment; 48 migration records; desktop security version 1 |
| Existing data | One application user and one contained desktop SQL account; no patients or examinations |
| Local database access | Successful Windows authentication through shared memory |
| Default SQL instance TCP | Disabled in SQL Server configuration; configured port 1433 |
| Strict encrypted TCP probe | Connection refused before TLS negotiation; certificate validity therefore unverified |
| Other local SQL instance | `ABLOCALHOST` has TCP enabled on dynamic port 57059; it is not the configured application database target |
| Local Orthanc service | Stopped |
| Release connection profile | No saved clinic profile for this Windows account |

These are development-PC findings. They do not establish the configuration of a separate clinic server or connectivity from another workstation. SQL protocols, services, firewall rules and connection settings were not changed.

## Live results

| Screen / action | Result and limits |
|---|---|
| Startup | Release reached setup; Debug reached welcome; blue branded splash observed during rebuilt startup |
| Setup | Arabic workstation/admin choices render; blank server rejected; admin setup action disabled until acknowledgement; no setup/upgrade submitted |
| Sign-in | Existing administrator signed in successfully twice; dashboard appeared without bouncing back to login |
| Dashboard | Actual zero counts loaded; no sample revenue data |
| Calendar | Day/week/list switching works; Arabic date and week range correct for 25 October; empty equipment/appointment states shown |
| Booking dialog | Opens and scrolls; empty required patient prevents submission; cancelled without a booking |
| Patients / Visits / Examination Types | Lists loaded and appropriate empty states shown |
| Reading Room | Empty worklist, report and viewer verified after correcting a false error state |
| Analytics overview | Zero-value metrics loaded; no export or financial operation submitted |
| Cash sessions / insurance claims / payroll runs | Lists loaded and appropriate empty states shown; no transactions submitted |
| Language | English calendar layout checked; Arabic restored |

The window was checked at approximately 1200 × 730 pixels. This does not cover every clinic monitor or Windows scaling setting. Successful admin navigation is not proof of least-privileged role enforcement.

## Defects fixed during this walkthrough

1. **Calendar page overflow:** the hidden temporary details drawer remained mounted and expanded the page horizontally in Arabic. Mount it only when an appointment is selected and details are open. Rebuilt app confirmed no whole-window horizontal scrollbar; the week grid retains its own horizontal scrolling.
2. **False Reading Room error:** `Error="_reportError"` passed a literal string instead of the state value. Corrected to `Error="@_reportError"`. Rebuilt app now shows the proper no-report prompt rather than an error when no study is selected.
3. **Dark image viewer readability:** empty-view text and icon had insufficient contrast. Added light foreground colors scoped to the dark viewer; visually verified in the rebuilt app.
4. **Text encoding:** corrected the welcome-page copyright symbol and the insurance attachment separator. Copyright verified in the rebuilt app; attachment formatting compiled but not exercised with a real attachment.
5. **Import icons:** Patients and Examination Types imports used the download icon. Changed to upload; compiled.
6. **Analytics export:** localized default label/tooltip, kept the label while busy, and disabled format selection during export. Compiled; actual file export was not exercised.

## Verification evidence

- Fresh Debug build: 0 warnings, 0 errors (`audit-results/live-walkthrough-fixed.log`).
- Release build: 0 warnings, 0 errors (`audit-results/live-walkthrough-release.log`).
- Startup/calendar regression suite: 27 passed, 0 failed (`audit-results/live-walkthrough.trx`, `live-walkthrough-tests.log`). These tests do not create a clinic database.
- `git diff --check`: passed.
- Live runtime logs examined after navigation contained no Error/Critical entries. Existing EF model-scanning/query-order and Wolverine diagnostics remain warnings; this is not a claim of warning-free runtime logs.
- Native window observations are recorded in the task's computer-use outputs. The earlier synthetic browser fixtures are separate evidence and are not presented as real clinic execution.

## Still required before deployment

1. Configure and verify the intended clinic SQL host over LAN: reachable TCP listener, trusted TLS certificate matching the shared server name, and the same exported clinic profile on each workstation. The development PC's default SQL instance currently cannot serve this TCP connection.
2. Exercise the Release app's contained-account sign-in and permissions against that server. The successful sign-ins above used the Debug development authentication path.
3. Use designated test data to complete patient registration, equipment/examination setup, booking, check-in, report editing/finalization and financial workflows. Empty-list checks cannot prove these write paths.
4. Use two physical workstations to verify shared visibility, competing bookings, connection loss/recovery and clinic time behavior. No second workstation was available in this session.
5. Configure the real PACS endpoint/accounts and verify study receipt, linking and viewing with the clinic's imaging equipment. No live study was available; local Orthanc was stopped.
6. Verify backup restore, actual role restrictions, first-login password change, exports/imports/attachments and Windows scaling. The existing admin did not require a password change during this walkthrough.
7. Build/sign the installer and verify installation/update on a clean workstation. No installer was rebuilt or published here.

Minor observed localization gaps remain: profile validation reports an English server-address message in Arabic setup, and WebView native required-field validation appears in English. Neither prevented the empty-field validation from working.

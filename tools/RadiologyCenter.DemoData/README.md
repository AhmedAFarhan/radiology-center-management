# Clinic demo dataset

Developer-only console utility. It adds labeled synthetic records through the application's in-process operations. It is not referenced by the desktop project, does not run at startup, and is not part of the installer.

It requires an existing, ready local SQL database, Windows authentication with SQL administrator access, an exact expected database name, and the private `appsettings.Development.json` path. The first run checks that the clinical store is empty, takes a new COPY_ONLY backup and verifies its checksum. No database is created, migrated, cleared or restored. The existing administrator's password/account state is fingerprinted before and after.

From the repository root:

```powershell
dotnet run --project tools/RadiologyCenter.DemoData -- `
  --apply `
  --config "C:/full/path/to/src/Desktop/RadiologyCenter.Desktop/appsettings.Development.json" `
  --database RadiologyCenter `
  --output "C:/full/path/to/audit-results/demo-data-2026-10-25"
```

Keep the output folder. The journal records each completed step; rerun the same command and output path to resume an interrupted run. Do not remove the journal to start again against populated data. Existing non-demo records are never deleted. A backup is a recovery artifact; restoring it later would also discard changes made after the backup, so restoration is deliberately not automatic.

Generated temporary passwords are stored in `test-accounts.json` inside that private output folder, which must remain outside version control. The eight new `demo.*` application users require a password change at first login. The normal run grants **no SQL sign-in access** to them. SQL identity provisioning is a separate opt-in (`--provision-test-sql-accounts`) and must be authorized for those accounts before use; existing principals are skipped rather than having their passwords reset.

Most writes use the application's validated operations, including payments, inventory receipt, scheduling, reports and insurance decisions. Notification messages are local EF/domain fixtures and never invoke a delivery provider. They are marked as synthetic, use reserved `.invalid` addresses and include no real contact data. Database identity provisioning, when separately authorized, uses the existing `Security.ProvisionDesktopUser` procedure.

Calendar dates are anchored to the first seed run in Cairo time. A completed dataset is not automatically shifted forward on later runs. PACS images are not fabricated or linked to nonexistent studies. See the generated-data guide in `docs/demo-data-2026-10-25.md` for coverage and remaining live integration work.

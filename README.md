# EGcare radiology center management

EGcare is one Windows MAUI/Blazor desktop app installed on the clinic's admin PC and workstations. Business operations run inside each app and use the shared SQL Server database on the admin PC. Images use the central Orthanc archive. Routine use and updates from the clinic share work on the local network.

Follow the [LAN installation guide](docs/lan-deployment.md) for SQL certificates, first setup, staff accounts, images, updates and Visual Studio development. The [repair and verification status](docs/production-repair-status.md) records local checks and the acceptance/recovery work still required before clinical use.

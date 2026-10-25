using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using RadiologyCenter.Clinic;
using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.Identity.Domain;
using RadiologyCenter.Notification.Domain.Entities;
using RadiologyCenter.Notification.Domain.Enumerations;
using RadiologyCenter.Notification.Infrastructure.Persistence;

public sealed class DemoSeeder(ClinicRuntime runtime, SqlConnection connection, string token,
    SeedJournal journal, Action saveJournal, string output, bool provisionAccounts = false)
{
    private readonly DateTime _day = journal.CairoDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified);
    private readonly JsonSerializerOptions _json = ClinicDispatcher.JsonOptions;
    private readonly Dictionary<string, Guid> _ids = new();
    private Guid Id(string key) => _ids.TryGetValue(key, out var id) ? id : journal.Steps[key]!["id"]!.GetValue<Guid>();

    private async Task<Guid?> FindId(string table, string column, object value)
    {
        await using var command = connection.CreateCommand();
        var qualifiedTable = string.Join(".", table.Split('.').Select(part => "[" + part.Replace("]", "]]") + "]"));
        command.CommandText = $"SELECT Id FROM {qualifiedTable} WHERE [{column.Replace("]", "]]")}] = @value";
        command.Parameters.AddWithValue("@value", value);
        return await command.ExecuteScalarAsync() is Guid id ? id : null;
    }

    private async Task<JsonNode?> Call(string kind, string path, object? body = null, ClinicUpload? upload = null,
        IReadOnlyDictionary<string, string>? fields = null)
    {
        var response = await runtime.Dispatcher.DispatchAsync(new(kind, path,
            body is null ? null : JsonSerializer.Serialize(body, _json), fields, upload, token));
        if (response.Value is ClinicFile file) return new JsonObject { ["bytes"] = file.Content.Length, ["fileName"] = file.FileName };
        var envelope = JsonSerializer.SerializeToNode(response.Value, _json);
        if (!response.IsSuccess || envelope?["success"]?.GetValue<bool>() == false)
            throw new InvalidOperationException($"{kind} {path} ({response.StatusCode}): {envelope?.ToJsonString()}");
        return envelope?["data"]?.DeepClone();
    }

    private async Task<JsonNode?> Step(string key, string kind, string path, object? body = null,
        Func<Task<JsonNode?>>? resolve = null, ClinicUpload? upload = null, IReadOnlyDictionary<string, string>? fields = null)
    {
        if (journal.Steps.TryGetValue(key, out var prior)) return prior?.DeepClone();
        var data = await Call(kind, path, body, upload, fields);
        if (resolve is not null) data = await resolve();
        journal.Steps.Add(key, data?.DeepClone());
        saveJournal();
        return data;
    }

    private async Task<Guid> Create(string key, string path, object body, string? table = null, string? column = null, object? value = null)
    {
        async Task<JsonNode?> Resolve()
        {
            if (table is null || column is null || value is null) throw new InvalidOperationException("Missing lookup.");
            var id = await FindId(table, column, value) ?? throw new InvalidOperationException($"Could not find {key} after creation.");
            return new JsonObject { ["id"] = id };
        }
        // Resolve a committed creation if the process stopped between commit and journalling.
        if (!journal.Steps.ContainsKey(key) && table is not null && column is not null && value is not null
            && await FindId(table, column, value) is Guid existing)
        {
            journal.Steps[key] = new JsonObject { ["id"] = existing }; saveJournal();
        }
        var data = await Step(key, "POST", path, body, table is null ? null : Resolve);
        var result = data?["id"]?.GetValue<Guid>() ?? throw new InvalidOperationException($"{key} did not return an id.");
        _ids[key] = result;
        return result;
    }

    public async Task RunAsync()
    {
        await IdentityAsync();
        await ResourcesAsync();
        await InventoryAsync();
        await PatientsAsync();
        await FeesAsync();
        await ExaminationsAsync();
        await ReportsAsync();
        await InsuranceAsync();
        await PayrollAsync();
        await CashAsync();
        await NotificationsAsync();
        await VerifyAsync();
    }

    private async Task IdentityAsync()
    {
        Console.WriteLine("Preparing DEMO roles and individual test accounts...");
        string[] clinicalRead = ["patients.read", "examinations.read", "examinations.types.read", "resources.equipment.read", "resources.staff.read"];
        var roles = new Dictionary<string, string[]>
        {
            ["Reception"] = [..clinicalRead, "patients.create", "patients.update", "examinations.create", "examinations.update", "examinations.cancel", "resources.referrals.read"],
            ["Radiologist"] = [..clinicalRead, "examinations.perform", "examinations.update", "reports.read", "reports.create", "reports.update", "reports.export"],
            ["Technician"] = [..clinicalRead, "examinations.perform", "examinations.update", "inventory.items.read"],
            ["Cashier"] = [..clinicalRead, "examinations.update", "cash.sessions.read", "cash.sessions.open", "cash.sessions.close", "cash.entries.add", "cash.handovers.read", "cash.handovers.approve"],
            ["Insurance"] = Permissions.All.Select(p => p.Code).Where(p => p.StartsWith("insurance.")).Concat(clinicalRead).ToArray(),
            ["Manager"] = Permissions.All.Select(p => p.Code).Where(p => p.StartsWith("payroll.") || p.StartsWith("analytics.") || p.StartsWith("resources.") || p.StartsWith("inventory.")).Concat(clinicalRead).ToArray()
        };
        var available = Permissions.All.Select(p => p.Code).ToHashSet();
        foreach (var (name, permissions) in roles)
        {
            var roleId = await Create("role-" + name, "roles", new { Name = "DEMO " + name, Description = "Synthetic training role; remove before production." },
                "Identity.AspNetRoles", "Name", "DEMO " + name);
            foreach (var permission in permissions.Distinct())
            {
                if (!available.Contains(permission)) continue;
                await Step($"role-{name}-{permission}", "POST", $"roles/{roleId}/permissions", new { PermissionCode = permission });
            }
        }
        var credentialsPath = Path.Combine(output, "test-accounts.json");
        var credentials = File.Exists(credentialsPath)
            ? JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(credentialsPath))!
            : new Dictionary<string, string>();
        (string User, string Role, string First, string Last)[] users =
        [
            ("demo.reception", "Reception", "DEMO", "Reception"),
            ("demo.radiologist", "Radiologist", "DEMO", "Radiologist One"),
            ("demo.radiologist2", "Radiologist", "DEMO", "Radiologist Two"),
            ("demo.technician", "Technician", "DEMO", "Technician One"),
            ("demo.technician2", "Technician", "DEMO", "Technician Two"),
            ("demo.cashier", "Cashier", "DEMO", "Cashier"),
            ("demo.insurance", "Insurance", "DEMO", "Insurance"),
            ("demo.manager", "Manager", "DEMO", "Manager")
        ];
        for (var i = 0; i < users.Length; i++)
        {
            var user = users[i];
            if (!credentials.ContainsKey(user.User))
            {
                credentials[user.User] = "Demo1!" + Convert.ToHexString(RandomNumberGenerator.GetBytes(12)).ToLowerInvariant();
                File.WriteAllText(credentialsPath, JsonSerializer.Serialize(credentials, new JsonSerializerOptions { WriteIndented = true }));
            }
            await Create(user.User, "users", new { UserName = user.User, Email = user.User + "@example.invalid", FirstName = user.First,
                LastName = user.Last, PhoneNumber = $"01000000{100 + i}", Password = credentials[user.User], RoleIds = new[] { Id("role-" + user.Role) } },
                "Identity.AspNetUsers", "UserName", user.User);
            var provisionKey = "sql-account-" + user.User;
            if (provisionAccounts && !journal.Steps.ContainsKey(provisionKey))
            {
                await using var check = connection.CreateCommand();
                check.CommandText = "SELECT COUNT(*) FROM [Security].[DesktopAccounts] WHERE UserId=@id";
                check.Parameters.AddWithValue("@id", Id(user.User));
                if (Convert.ToInt32(await check.ExecuteScalarAsync()) == 0)
                {
                    await using var provision = connection.CreateCommand();
                    provision.CommandText = "[Security].[ProvisionDesktopUser]";
                    provision.CommandType = System.Data.CommandType.StoredProcedure;
                    provision.Parameters.AddWithValue("@UserId", Id(user.User));
                    provision.Parameters.AddWithValue("@Password", credentials[user.User]);
                    await provision.ExecuteNonQueryAsync();
                }
                journal.Steps[provisionKey] = JsonValue.Create(true); saveJournal();
            }
        }
    }

    private async Task ResourcesAsync()
    {
        Console.WriteLine("Preparing equipment, staff, shifts and referral doctors...");
        (string Key, string Name, string Modality)[] machines =
        [ ("xray", "DEMO X-Ray Room 1", "XRay"), ("ct", "DEMO CT Room", "CT"), ("mri", "DEMO MRI Room", "MRI"),
          ("us", "DEMO Ultrasound Room", "Ultrasound"), ("maintenance", "DEMO X-Ray Maintenance", "XRay"), ("retired", "DEMO Retired CT", "CT") ];
        foreach (var machine in machines)
            await Create("equipment-" + machine.Key, "resources/equipment", new { machine.Name, machine.Modality, SerialNumber = "DEMO-" + machine.Key.ToUpperInvariant(), PurchaseDate = _day.AddYears(-2) },
                "ResourceManagement.Equipment", "SerialNumber", "DEMO-" + machine.Key.ToUpperInvariant());
        await Step("maintenance-status", "POST", $"resources/equipment/{Id("equipment-maintenance")}/status", new { Status = "UnderMaintenance" });
        await Step("retired-status", "POST", $"resources/equipment/{Id("equipment-retired")}/status", new { Status = "Retired" });
        (string User, string Name, string Position)[] staff =
        [ ("demo.radiologist", "DEMO د. أحمد أشعة", "Radiologist"), ("demo.radiologist2", "DEMO Dr Sara Radiology", "Radiologist"),
          ("demo.technician", "DEMO فني محمد", "Technician"), ("demo.technician2", "DEMO Technician Mona", "Technician"),
          ("demo.reception", "DEMO استقبال هدى", "Receptionist"), ("demo.cashier", "DEMO Cashier", "Other"),
          ("demo.insurance", "DEMO Insurance Clerk", "Other"), ("demo.manager", "DEMO Manager", "Other") ];
        for (var i = 0; i < staff.Length; i++)
        {
            var person = staff[i];
            var staffId = await Create("staff-" + i, "resources/staff", new { UserId = Id(person.User), FullName = person.Name,
                PhoneNumber = $"01000000{200 + i}", person.Position, HireDate = _day.AddYears(-1), Department = "DEMO Radiology", LicenseNumber = "DEMO-LIC-" + i },
                "ResourceManagement.Staff", "UserId", Id(person.User));
            for (var day = 0; day < 5; day++)
                await Create($"shift-{i}-{day}", "resources/work-shifts", new { StaffId = staffId, Date = _day.AddDays(day), StartTime = "08:00:00", EndTime = "23:59:00", Notes = "DEMO training shift" });
        }
        await Create("leave-annual", "resources/leaves", new { StaffId = Id("staff-3"), LeaveType = "Annual", StartDate = _day.AddDays(8), EndDate = _day.AddDays(10), Reason = "DEMO planned annual leave" });
        await Create("leave-sick", "resources/leaves", new { StaffId = Id("staff-4"), LeaveType = "Sick", StartDate = _day.AddDays(-5), EndDate = _day.AddDays(-4), Reason = "DEMO past sick leave" });
        for (var i = 0; i < 3; i++)
            await Create("referral-" + i, "resources/referral-doctors", new { FullName = "DEMO Referral Doctor " + (i + 1), Phone = $"01000000{300 + i}",
                Email = $"referral{i}@example.invalid", Specialization = i == 0 ? "Orthopedics" : "Internal medicine", Hospital = "DEMO Clinic" },
                "ResourceManagement.ReferralDoctors", "Email", $"referral{i}@example.invalid");
        (string Name, string Modality, string BodyPart, int Duration, decimal Price)[] types =
        [ ("DEMO Chest X-Ray", "XRay", "Chest", 30, 250), ("DEMO CT Brain", "CT", "Head", 45, 1200),
          ("DEMO MRI Spine", "MRI", "Spine", 60, 2200), ("DEMO Abdominal Ultrasound", "Ultrasound", "Abdomen", 20, 500),
          ("DEMO CT Contrast Abdomen", "CT", "Abdomen", 60, 1800), ("DEMO Knee X-Ray", "XRay", "Extremities", 20, 300) ];
        for (var i = 0; i < types.Length; i++)
        {
            var type = types[i];
            await Create("type-" + i, "catalog/examination-types", new { type.Name, Code = "DEMO-EX-" + i, type.Modality, type.BodyPart,
                StandardDurationMinutes = type.Duration, type.Price, RequiresContrast = i == 4, RequiresConsent = i == 4,
                RequiresPreparation = i is 3 or 4, PreparationInstructions = i is 3 or 4 ? "DEMO instructions only; not clinical advice." : null },
                "Catalog.CatalogExaminationTypes", "Name", type.Name);
        }
    }

    private async Task InventoryAsync()
    {
        Console.WriteLine("Preparing stock, batches and purchase-order states...");
        (string Name, string Category, string Unit)[] items =
        [ ("DEMO Syringe 10 ml", "MedicalSupply", "Piece"), ("DEMO Iodinated Contrast", "ContrastMedia", "Vial"),
          ("DEMO Exam Gloves", "Consumable", "Box"), ("DEMO Ultrasound Gel", "Consumable", "Bottle"),
          ("DEMO Imaging Film", "Consumable", "Sheet"), ("DEMO Low Stock Cannula", "MedicalSupply", "Piece") ];
        for (var i = 0; i < items.Length; i++)
            await Create("item-" + i, "inventory/items", new { items[i].Name, items[i].Category, items[i].Unit, Brand = "DEMO", ReorderLevel = 20,
                ReorderQuantity = 100, LotTracked = true, StorageInstructions = "DEMO training stock. Not for patient use." }, "Inventory.Items", "Name", items[i].Name);
        for (var i = 0; i < 2; i++)
            await Create("supplier-" + i, "inventory/suppliers", new { Name = "DEMO Medical Supplier " + (i + 1), Phone = $"01000000{400 + i}",
                Email = $"supplier{i}@example.invalid", ContactPerson = "DEMO Contact", Address = "DEMO Address", TaxNumber = "DEMO-TAX-" + i },
                "Inventory.Suppliers", "Name", "DEMO Medical Supplier " + (i + 1));
        for (var order = 0; order < 4; order++)
        {
            var lines = Enumerable.Range(0, items.Length).Select(i => new { ItemId = Id("item-" + i), QuantityOrdered = i == 5 ? 5 : 100, UnitCost = 5m + i * 10m }).ToArray();
            var orderId = await Create("po-" + order, "inventory/purchase-orders", new { SupplierId = Id("supplier-" + order % 2), Items = lines,
                ExpectedDeliveryAt = _day.AddDays(order + 1), Notes = "DEMO purchase order " + order }, "Inventory.PurchaseOrders", "Notes", "DEMO purchase order " + order);
            if (order == 0 || order == 2 || order == 3) await Step("po-place-" + order, "POST", $"inventory/purchase-orders/{orderId}/place");
            if (order == 0)
                await Step("po-receive", "POST", $"inventory/purchase-orders/{orderId}/receive", new { Lines = lines.Select((line, i) => new { line.ItemId, Quantity = line.QuantityOrdered,
                    LotNumber = "DEMO-LOT-" + i, ExpiryDate = _day.AddMonths(i == 1 ? 1 : 12) }).ToArray() });
            if (order == 3) await Step("po-cancel", "POST", $"inventory/purchase-orders/{orderId}/cancel");
        }
        await Step("stock-issue", "POST", $"inventory/items/{Id("item-0")}/issue", new { Quantity = 3, Reference = "DEMO-TRAINING", Notes = "DEMO stock-issue scenario" });
        await Create("type-consumable", $"examinations/examination-types/{Id("type-0")}/items", new { ItemId = Id("item-4"), Quantity = 1, IsRequired = true, Notes = "DEMO film use" });
        await Create("type-contrast", $"examinations/examination-types/{Id("type-4")}/items", new { ItemId = Id("item-1"), Quantity = 1, IsContrast = true, IsRequired = true, Notes = "DEMO contrast use" });
    }

    private async Task PatientsAsync()
    {
        Console.WriteLine("Preparing 24 clearly labelled synthetic patients...");
        string[] names = ["أحمد محمد علي", "منى حسن إبراهيم", "سارة خالد محمود", "عمر عادل حسن", "نور أحمد سعيد", "ليلى مصطفى عمر",
            "يوسف محمد فؤاد", "مريم سامي كمال", "آدم حسام علي", "هنا محمد طارق", "كريم أحمد مراد", "ريم عادل زكي",
            "Adam Example", "Sara Training", "Omar Sample", "Mona Fictional", "Youssef Demo", "Nour Test",
            "Ali Example", "Laila Training", "Khaled Sample", "Mariam Fictional", "Hassan Demo", "Rana Test"];
        for (var i = 0; i < names.Length; i++)
        {
            var email = $"patient{i + 1:00}@example.invalid";
            await Create("patient-" + i, "patients", new { FullName = $"DEMO {i + 1:00} {names[i]}", Gender = i % 2 == 0 ? "Male" : "Female",
                DateOfBirth = _day.AddYears(-(i < 3 ? 5 + i * 4 : 20 + i * 2)).AddDays(-i), PhoneNumber = $"0100000{5000 + i}", Email = email,
                Address = "DEMO synthetic address — Cairo", Allergies = i == 4 ? "DEMO allergy scenario; no real patient." : null,
                MedicalHistory = "SYNTHETIC TRAINING RECORD — not a real person." }, "Patients.Patients", "Email", email);
        }
        await Step("patient-inactive", "POST", $"patients/{Id("patient-23")}/deactivate");
    }

    private async Task FeesAsync()
    {
        Console.WriteLine("Preparing salary and examination/referral fee rules...");
        for (var i = 0; i < 6; i++)
        {
            await Create("fee-rad-" + i, "payroll/examination-fees", new { ExaminationTypeId = Id("type-" + i), Role = "Radiologist", Amount = 15m, IsPercentage = true });
            await Create("fee-tech-" + i, "payroll/examination-fees", new { ExaminationTypeId = Id("type-" + i), Role = "Technician", Amount = 25m });
            await Create("fee-ref-" + i, "payroll/referral-fees", new { ExaminationTypeId = Id("type-" + i), ReferralDoctorId = Id("referral-0"), Amount = 5m, IsPercentage = true });
        }
        await Create("component-earning", "payroll/salary-components", new { Name = "DEMO Transport Allowance", Kind = "Earning", DefaultValue = 500m, Frequency = "Monthly" },
            "Payroll.SalaryComponents", "Name", "DEMO Transport Allowance");
        await Create("component-deduction", "payroll/salary-components", new { Name = "DEMO Training Deduction", Kind = "Deduction", DefaultValue = 100m, Frequency = "Monthly" },
            "Payroll.SalaryComponents", "Name", "DEMO Training Deduction");
        for (var i = 0; i < 8; i++)
        {
            await Create("salary-" + i, "payroll/salaries", new { StaffId = Id("staff-" + i), BaseSalary = i < 2 ? 20000m : 8000m, SalaryType = "Monthly", EffectiveDate = _day.AddMonths(-6) });
            await Create("allowance-" + i, "payroll/allowances", new { StaffId = Id("staff-" + i), Name = "DEMO Transport " + i, Amount = 500m,
                EffectiveDate = _day.AddMonths(-6), SalaryComponentId = Id("component-earning"), Frequency = "Monthly" });
        }
    }

    private async Task ExaminationsAsync()
    {
        Console.WriteLine("Preparing calendar appointments and clinical/payment states...");
        var open = await Call("GET", "cash/sessions/my-open");
        if (open?["id"] is not null && !journal.Steps.ContainsKey("cash-open"))
            throw new InvalidOperationException("An existing cash session was found. Review it before adding DEMO payments.");
        await Create("cash-open", "cash/sessions", new { OpeningFloat = 500m, Notes = "DEMO training cash session — no real money" });
        string[] machines = ["xray", "ct", "mri", "us"];
        var first = journal.StartedAt.AddHours(1);
        first = new DateTime((first.Ticks / TimeSpan.TicksPerHour + 1) * TimeSpan.TicksPerHour, DateTimeKind.Utc);
        for (var day = 0; day < 4; day++)
        for (var j = 0; j < 8; j++)
        {
            var index = day * 8 + j;
            var equipment = j % 4;
            var start = day == 0 ? first.AddMinutes(90 * (j / 4)) : CairoUtc(_day.AddDays(day).AddHours(9).AddMinutes(90 * (j / 4)));
            var key = "booking-" + index;
            var marker = "DEMO calendar scenario " + index;
            await Create(key, "examinations/book", new { PatientId = Id("patient-" + index % 23), ExaminationTypeId = Id("type-" + equipment),
                EquipmentId = Id("equipment-" + machines[equipment]), ScheduledAt = start, ClinicalIndication = marker,
                Priority = index % 7 == 0 ? "Urgent" : index % 11 == 0 ? "Stat" : "Routine", Notes = "DEMO training appointment. Not a real patient." },
                "Examinations.Examinations", "ClinicalIndication", marker);
        }
        await Step("booking-cancel", "POST", $"examinations/{Id("booking-7")}/cancel", new { Reason = "DEMO cancellation scenario" });
        await Step("booking-partial-pay", "POST", $"examinations/{Id("booking-0")}/payments", new { Amount = 100m, Description = "DEMO partial payment" });
        for (var i = 0; i < 10; i++)
        {
            var type = i % 4;
            var price = new[] { 250m, 1200m, 2200m, 500m }[type];
            var marker = "DEMO clinical/report scenario " + i;
            await Create("clinical-" + i, "examinations", new { PatientId = Id("patient-" + i), ExaminationTypeId = Id("type-" + type),
                RadiologistId = Id("staff-" + i % 2), TechnicianId = Id("staff-" + (2 + i % 2)), EquipmentId = Id("equipment-" + machines[type]),
                ReferralDoctorId = Id("referral-0"), ClinicalIndication = marker, Priority = i == 2 ? "Urgent" : "Routine", Status = "CheckedIn",
                Paid = i == 9 ? 0m : price, Notes = "DEMO synthetic visit; reporting practice only." }, "Examinations.Examinations", "ClinicalIndication", marker);
            if (i < 8) await Step("clinical-start-" + i, "POST", $"examinations/{Id("clinical-" + i)}/start");
            if (i < 6) await Step("clinical-complete-" + i, "POST", $"examinations/{Id("clinical-" + i)}/complete");
        }
    }

    private async Task ReportsAsync()
    {
        Console.WriteLine("Preparing report templates, drafts, finalized reports and amendments...");
        for (var i = 0; i < 4; i++)
            await Create("template-" + i, "reports/templates", new { Name = "DEMO Training Template " + i, Modality = new[] { "XRay", "CT", "MRI", "Ultrasound" }[i],
                Description = "SYNTHETIC training template. Not for clinical use.", Sections = new[]
                {
                    new { SectionType = "Technique", Title = "Technique", Body = "DEMO training examination technique.", Position = 0, IsLocked = false },
                    new { SectionType = "Findings", Title = "Findings", Body = "DEMO ONLY — enter fictional training findings here.", Position = 1, IsLocked = false },
                    new { SectionType = "Impression", Title = "Impression", Body = "DEMO ONLY — not a clinical diagnosis.", Position = 2, IsLocked = false }
                } }, "Reports.ReportTemplates", "Name", "DEMO Training Template " + i);
        for (var i = 0; i < 8; i++)
        {
            var report = await Create("report-" + i, "reports", new { ExaminationId = Id("clinical-" + i), PatientId = Id("patient-" + i), RadiologistId = Id("staff-" + i % 2) },
                "Reports.RadiologyReports", "ExaminationId", Id("clinical-" + i));
            await ReportStep("report-template-" + i, report, "POST", "apply-template", new { TemplateId = Id("template-" + i % 4) });
            await ReportStep("report-finding-" + i, report, "POST", "findings", new { Region = "DEMO region", Description = "Synthetic finding for UI testing only; not a medical conclusion.", Severity = i % 2 == 0 ? "Mild" : "Moderate" });
            if (i < 4) await ReportStep("report-final-" + i, report, "POST", "finalize");
            if (i == 1) await ReportStep("report-amend", report, "POST", "amend", new { Reason = "DEMO amendment/version-history scenario" });
            if (i == 5) await ReportStep("report-cancel", report, "POST", "cancel", new { Reason = "DEMO cancelled-report scenario" });
        }
    }

    private async Task ReportStep(string key, Guid report, string kind, string action, object? body = null)
    {
        if (journal.Steps.ContainsKey(key)) return;
        var current = await Call("GET", $"reports/{report}");
        var payload = JsonSerializer.SerializeToNode(body, _json) as JsonObject ?? new JsonObject();
        payload["expectedRevision"] = current!["revision"]!.GetValue<int>();
        await Step(key, kind, $"reports/{report}/{action}", payload);
    }

    private async Task InsuranceAsync()
    {
        Console.WriteLine("Preparing insurance policies, documents, authorizations and claims...");
        for (var i = 0; i < 2; i++)
            await Create("company-" + i, "insurance/companies", new { Name = "DEMO Insurance " + (i + 1), TaxId = "DEMO-INS-" + i,
                Phone = $"01000000{600 + i}", Email = $"insurance{i}@example.invalid", Address = "DEMO training insurer" }, "Insurance.InsuranceCompanies", "Name", "DEMO Insurance " + (i + 1));
        for (var i = 0; i < 8; i++)
        {
            var policy = await Create("policy-" + i, "insurance/policies", new { CompanyId = Id("company-" + i % 2), PatientId = Id("patient-" + i),
                PolicyNumber = "DEMO-POL-" + i, CoveragePercent = i % 2 == 0 ? 75m : 100m, EffectiveFrom = _day.AddMonths(-2),
                EffectiveTo = _day.AddYears(1), IsGovernment = i % 2 == 1 }, "Insurance.InsurancePolicies", "PolicyNumber", "DEMO-POL-" + i);
            var amount = new[] { 250m, 1200m, 2200m, 500m }[i % 4];
            var auth = await Create("preauth-" + i, "insurance/preauthorizations", new { ExaminationId = Id("clinical-" + i), PatientId = Id("patient-" + i), PolicyId = policy, EstimatedAmount = amount },
                "Insurance.PreAuthorizations", "ExaminationId", Id("clinical-" + i));
            if (i % 2 == 1 || i == 6)
                await Step("auth-document-" + i, "POST", $"insurance/preauthorizations/{auth}/documents", upload: new ClinicUpload("File", $"DEMO-approval-{i}.pdf", "application/pdf", TrainingPdf()), fields: new Dictionary<string, string> { ["Type"] = "GovernmentApproval" });
            if (i < 6)
            {
                await Step("preauth-approve-" + i, "POST", $"insurance/preauthorizations/{auth}/decide", new { Decision = "Approve", ApprovedAmount = amount });
                var claim = await Create("claim-" + i, "insurance/claims", new { ExaminationId = Id("clinical-" + i), PatientId = Id("patient-" + i), PolicyId = policy,
                    PreAuthorizationId = auth, BilledAmount = amount }, "Insurance.Claims", "ExaminationId", Id("clinical-" + i));
                if (i > 0) await Step("claim-submit-" + i, "POST", $"insurance/claims/{claim}/submit");
                if (i >= 2)
                    await Step("claim-decide-" + i, "POST", $"insurance/claims/{claim}/adjudicate", i == 3
                        ? new { Decision = "Reject", RejectionCode = "CoverageExcluded", RejectionReason = "DEMO rejection practice" } as object
                        : new { Decision = "Approve", ApprovedAmount = amount * .75m });
                if (i >= 4)
                    await Step("claim-settle-" + i, "POST", $"insurance/claims/{claim}/settlements", new { Method = "BankTransfer", Amount = amount * (i == 4 ? .25m : .75m), Reference = "DEMO-BANK-" + i });
            }
            if (i == 7) await Step("preauth-deny", "POST", $"insurance/preauthorizations/{auth}/decide", new { Decision = "Deny", RejectionReason = "DEMO denied authorization" });
            if (i < 2)
            {
                var pdf = TrainingPdf();
                await Step("policy-document-" + i, "POST", $"insurance/policies/{policy}/documents", upload: new ClinicUpload("File", $"DEMO-policy-{i}.pdf", "application/pdf", pdf), fields: new Dictionary<string, string> { ["Type"] = "InsuranceCard" });
            }
        }
    }

    private async Task PayrollAsync()
    {
        Console.WriteLine("Preparing payroll runs and payslips...");
        var month = new DateTime(_day.Year, _day.Month, 1);
        for (var i = 0; i < 3; i++)
        {
            var from = month.AddMonths(i - 1);
            var run = await Create("payrun-" + i, "payroll/payruns", new { RunFrom = from, RunTo = from.AddMonths(1).AddDays(-1), Notes = "DEMO payroll scenario " + i },
                "Payroll.PayRuns", "Notes", "DEMO payroll scenario " + i);
            for (var staff = 0; staff < 8; staff++) await Step($"payslip-{i}-{staff}", "POST", $"payroll/payruns/{run}/payslips", new { StaffId = Id("staff-" + staff) });
            if (i < 2) await Step("payrun-compute-" + i, "POST", $"payroll/payruns/{run}/compute");
            if (i == 0) await Step("payrun-approve", "POST", $"payroll/payruns/{run}/approve");
        }
    }

    private async Task NotificationsAsync()
    {
        Console.WriteLine("Preparing notification templates and local-only fixture messages...");
        await Create("notification-template", "notifications/templates", new { Code = "DEMO_APPOINTMENT", Name = "DEMO Appointment Reminder",
            Subject = "DEMO appointment {Date}", Body = "DEMO training only: {Patient} has an appointment on {Date}." }, "Notification.NotificationTemplates", "Code", "DEMO_APPOINTMENT");
        if (!journal.Steps.ContainsKey("notification-fixtures"))
        {
            await using var scope = runtime.Services.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
            for (var i = 0; i < 3; i++)
            {
                var reference = "DEMO-LOCAL-NOTIFICATION-" + i;
                if (await context.NotificationMessages.AnyAsync(m => m.ReferenceId == reference)) continue;
                var message = NotificationMessage.Create($"training{i}@example.invalid", NotificationChannel.Email,
                    "DEMO notification fixture " + i, "Synthetic local record. No message was sent.", "DEMO_APPOINTMENT", reference);
                // Store local fixtures directly: no send operation/provider is invoked.
                if (i == 0) message.MarkFailed("DEMO simulated failure — no delivery attempted.");
                context.NotificationMessages.Add(message);
            }
            await context.SaveChangesAsync();
            journal.Steps["notification-fixtures"] = JsonValue.Create(true); saveJournal();
        }
    }

    private async Task CashAsync()
    {
        Console.WriteLine("Preparing cash expenses, closed-session handover and an open training session...");
        var session = Id("cash-open");
        await Step("cash-expense", "POST", $"cash/sessions/{session}/entries", new { Direction = "Out", Reason = "Payout", Amount = 50m,
            ReferenceId = "DEMO-EXPENSE", Description = "DEMO training supplies expense — no real money" });
        if (!journal.Steps.ContainsKey("cash-close"))
        {
            var details = await Call("GET", $"cash/sessions/{session}");
            await Step("cash-close", "POST", $"cash/sessions/{session}/close", new { CountedTotal = details!["balance"]!.GetValue<decimal>(),
                ReceivingUserId = Id("demo.cashier"), ReceivingOpeningFloat = 300m, Notes = "DEMO pending handover for approval practice" });
        }
        await Create("cash-current", "cash/sessions", new { OpeningFloat = 500m, Notes = "DEMO current training session — no real money" },
            "Cash.CashSessions", "Notes", "DEMO current training session — no real money");
    }

    private async Task VerifyAsync()
    {
        Console.WriteLine("Verifying populated lists, calendar, reading queue and analytics...");
        var results = new JsonObject();
        string[] lists = ["patients", "examinations", "resources/equipment", "resources/staff", "resources/work-shifts", "resources/leaves", "resources/referral-doctors",
            "catalog/examination-types", "inventory/items", "inventory/suppliers", "inventory/purchase-orders", "reports", "reports/templates", "insurance/companies",
            "insurance/policies", "insurance/preauthorizations", "insurance/claims", "cash/sessions", "cash/handovers", "payroll/payruns", "payroll/salaries", "payroll/salary-components",
            "payroll/allowances", "payroll/examination-fees", "payroll/referral-fees", "notifications/templates", "notifications/messages", "roles", "users"];
        foreach (var list in lists)
        {
            var data = list == "insurance/companies" ? await Call("GET", list)
                : await Call("POST", list + "/all", new { PageNumber = 1, PageSize = 100 });
            var count = data is JsonArray array ? array.Count : data?["totalCount"]?.GetValue<int>() ?? data?["items"]?.AsArray().Count ?? 0;
            if (count == 0) throw new InvalidOperationException("The seeded list was unexpectedly empty: " + list);
            results[list] = count;
        }
        var start = Uri.EscapeDataString(CairoUtc(_day).ToString("O"));
        var end = Uri.EscapeDataString(CairoUtc(_day.AddDays(7)).ToString("O"));
        var calendar = await Call("GET", $"examinations/calendar?startDate={start}&endDate={end}");
        results["calendarAppointments"] = calendar?.AsArray().Count;
        var queue = await Call("POST", "examinations/reading-queue", new { PageNumber = 1, PageSize = 100, IncludeFinalized = true });
        results["readingQueue"] = queue?["totalCount"]?.DeepClone();
        foreach (var report in new[] { "financial", "operational", "staff-machine", "profit", "insurance", "cash-flow" })
        {
            await Call("GET", $"analytics/{report}?from={Uri.EscapeDataString(CairoUtc(_day.AddMonths(-1)).ToString("O"))}&to={end}");
            results["analytics/" + report] = "passed";
        }
        File.WriteAllText(Path.Combine(output, "verification.json"), results.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine(results.ToJsonString());
    }

    private static DateTime CairoUtc(DateTime local) => TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(local, DateTimeKind.Unspecified), TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"));

    private static byte[] TrainingPdf()
    {
        const string stream = "BT /F1 16 Tf 50 760 Td (DEMO TRAINING DOCUMENT - NOT VALID FOR CLINICAL USE) Tj ET";
        string[] objects = ["<< /Type /Catalog /Pages 2 0 R >>", "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
            "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >>",
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>", $"<< /Length {stream.Length} >>\nstream\n{stream}\nendstream"];
        var pdf = new StringBuilder("%PDF-1.4\n");
        var offsets = new List<int>();
        for (var i = 0; i < objects.Length; i++) { offsets.Add(pdf.Length); pdf.Append($"{i + 1} 0 obj\n{objects[i]}\nendobj\n"); }
        var xref = pdf.Length; pdf.Append($"xref\n0 {objects.Length + 1}\n0000000000 65535 f \n");
        foreach (var offset in offsets) pdf.Append($"{offset:0000000000} 00000 n \n");
        pdf.Append($"trailer\n<< /Size {objects.Length + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF\n");
        return Encoding.ASCII.GetBytes(pdf.ToString());
    }
}

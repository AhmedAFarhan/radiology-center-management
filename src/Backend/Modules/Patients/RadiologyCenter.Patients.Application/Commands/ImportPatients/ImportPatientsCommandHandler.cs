using RadiologyCenter.BuildingBlocks.Application.Abstractions.Services;
using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Excel;
using RadiologyCenter.BuildingBlocks.Application.Localization;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Patients.Application.Abstractions;
using RadiologyCenter.Patients.Application.Commands.Common;
using RadiologyCenter.Patients.Application.Commands.CreatePatient;
using RadiologyCenter.Patients.Domain.Entities;
using RadiologyCenter.Patients.Domain.Enumerations;

namespace RadiologyCenter.Patients.Application.Commands.ImportPatients;

/// <summary>
/// Best-effort import of patients from an uploaded template workbook.
/// Rows are validated with the standard <see cref="CreatePatientCommandValidator"/>
/// rules; a row is skipped (and reported) when its National ID already
/// exists or appears twice within the file.
/// </summary>
public static class ImportPatientsCommandHandler
{
    public static async Task<Result<ExcelImportResult>> HandleAsync(
        ImportPatientsCommand command,
        IValidator<CreatePatientCommand> validator,
        IPatientRepository patientRepository,
        INumberSequenceGenerator numberSequenceGenerator,
        IPatientsUnitOfWork unitOfWork,
        IExcelService excelService,
        CancellationToken ct)
    {
        if (command.FileContent.Length > ExcelImportLimits.MaxFileBytes)
            return Result.Failure<ExcelImportResult>(Error.Validation("Excel.FileTooLarge", Translate("Excel.FileTooLarge")));

        ParsedWorkbook parsed;
        try
        {
            using var stream = new MemoryStream(command.FileContent);
            parsed = excelService.ReadTemplate(stream, HeaderCodes);
        }
        catch (ExcelImportException ex)
        {
            return Result.Failure<ExcelImportResult>(Error.Validation(ex.Code, TranslateArgs(ex.Code, ex.Args)));
        }

        if (!string.IsNullOrWhiteSpace(parsed.TemplateVersion) && parsed.TemplateVersion != ExcelImportLimits.TemplateVersion)
            return Result.Failure<ExcelImportResult>(Error.Validation("Excel.WrongVersion", Translate("Excel.WrongVersion")));

        var existingNationalIds = (await patientRepository.GetAllAsync(ct))
            .Where(p => !string.IsNullOrWhiteSpace(p.NationalId))
            .Select(p => Normalize(p.NationalId))
            .ToHashSet();

        var errors = new List<ExcelRowError>();
        var pending = new List<CreatePatientCommand>();
        var seenInFile = new HashSet<string>();

        foreach (var row in parsed.Rows)
        {
            var fullName = row.Value(HeaderCode.FullName)?.Trim() ?? string.Empty;
            var gender = row.Value(HeaderCode.Gender)?.Trim() ?? string.Empty;
            var dateOfBirthText = row.Value(HeaderCode.DateOfBirth)?.Trim();
            var ageText = row.Value(HeaderCode.Age)?.Trim();
            var phoneNumber = row.Value(HeaderCode.PhoneNumber)?.Trim() ?? string.Empty;

            DateTime? dateOfBirth = null;
            if (DateTime.TryParse(dateOfBirthText, out var parsedDate))
                dateOfBirth = parsedDate.Date;
            else if (!string.IsNullOrWhiteSpace(dateOfBirthText))
            {
                errors.Add(new ExcelRowError(row.RowNumber, "Excel.Error.RowInvalid", $"{row.RowNumber}: {Translate("Excel.Error.InvalidDate")}"));
                continue;
            }

            int? age = null;
            if (int.TryParse(ageText, out var parsedAge))
                age = parsedAge;

            var candidate = new CreatePatientCommand(
                fullName,
                gender,
                dateOfBirth,
                age,
                phoneNumber,
                row.Value(HeaderCode.Email)?.Trim(),
                row.Value(HeaderCode.Address)?.Trim(),
                row.Value(HeaderCode.NationalId)?.Trim(),
                row.Value(HeaderCode.BloodType)?.Trim(),
                row.Value(HeaderCode.Allergies)?.Trim(),
                row.Value(HeaderCode.MedicalHistory)?.Trim());

            var validation = await validator.ValidateAsync(candidate, ct);
            var rowErrors = validation.Errors.Select(e => Translator.LocalizeCode(e.ErrorCode, e.ErrorMessage)).ToList();

            var nationalId = candidate.NationalId;
            if (!string.IsNullOrWhiteSpace(nationalId))
            {
                var normalizedId = Normalize(nationalId);
                if (existingNationalIds.Contains(normalizedId) || seenInFile.Contains(normalizedId))
                    rowErrors.Add(Translate("Excel.Error.DuplicateNationalId"));
                else
                    seenInFile.Add(normalizedId);
            }

            if (rowErrors.Count > 0)
                errors.Add(new ExcelRowError(row.RowNumber, "Excel.Error.RowInvalid", string.Join(" · ", rowErrors)));
            else
                pending.Add(candidate);
        }

        if (pending.Count > 0)
        {
            await using var transaction = await unitOfWork.BeginTransactionAsync(ct);
            foreach (var candidate in pending)
            {
                var patientCode = await numberSequenceGenerator.GenerateNextAsync(
                    "Patient",
                    "PTN",
                    4,
                    transaction.DbTransaction,
                    ct);

                await patientRepository.AddAsync(
                    Patient.Create(
                        patientCode,
                        candidate.FullName,
                        Gender.FromName<Gender>(candidate.Gender),
                        candidate.DateOfBirth,
                        candidate.Age,
                        candidate.PhoneNumber,
                        candidate.Email,
                        candidate.Address,
                        candidate.NationalId,
                        candidate.BloodType is not null ? BloodType.FromName<BloodType>(candidate.BloodType) : null,
                        candidate.Allergies,
                        candidate.MedicalHistory),
                    ct);
            }

            await transaction.CommitAsync(ct);
        }

        return Result.Success(new ExcelImportResult(parsed.Rows.Count, pending.Count, errors));
    }

    private static class HeaderCode
    {
        public const string FullName = "Excel.Patient.FullName";
        public const string Gender = "Excel.Patient.Gender";
        public const string DateOfBirth = "Excel.Patient.DateOfBirth";
        public const string Age = "Excel.Patient.Age";
        public const string PhoneNumber = "Excel.Patient.PhoneNumber";
        public const string Email = "Excel.Patient.Email";
        public const string Address = "Excel.Patient.Address";
        public const string NationalId = "Excel.Patient.NationalId";
        public const string BloodType = "Excel.Patient.BloodType";
        public const string Allergies = "Excel.Patient.Allergies";
        public const string MedicalHistory = "Excel.Patient.MedicalHistory";
    }

    private static IReadOnlyList<string> HeaderCodes { get; } =
    [
        HeaderCode.FullName,
        HeaderCode.Gender,
        HeaderCode.DateOfBirth,
        HeaderCode.Age,
        HeaderCode.PhoneNumber,
        HeaderCode.Email,
        HeaderCode.Address,
        HeaderCode.NationalId,
        HeaderCode.BloodType,
        HeaderCode.Allergies,
        HeaderCode.MedicalHistory,
    ];

    private static string Normalize(string? value) => value?.Trim().ToLowerInvariant() ?? string.Empty;

    private static string Translate(string code) => Translator.LocalizeCode(code, code);

    private static string TranslateArgs(string code, object[] args)
    {
        var template = Translate(code);
        try
        {
            return string.Format(template, args);
        }
        catch (FormatException)
        {
            return template;
        }
    }
}

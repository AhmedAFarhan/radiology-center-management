using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Excel;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Patients.Domain.Enumerations;

namespace RadiologyCenter.Patients.Application.Queries.GetPatientsImportTemplate;

public static class GetPatientsImportTemplateQueryHandler
{
    public static Result<FileContentDto> Handle(
        GetPatientsImportTemplateQuery query,
        IExcelService excelService)
    {
        var content = excelService.CreateTemplate(
            "Patients",
            [
                ("Excel.Patient.FullName", "Full Name"),
                ("Excel.Patient.Gender", "Gender"),
                ("Excel.Patient.DateOfBirth", "Date of Birth (YYYY-MM-DD)"),
                ("Excel.Patient.Age", "Age"),
                ("Excel.Patient.PhoneNumber", "Phone"),
                ("Excel.Patient.Email", "Email"),
                ("Excel.Patient.Address", "Address"),
                ("Excel.Patient.NationalId", "National ID"),
                ("Excel.Patient.BloodType", "Blood Type"),
                ("Excel.Patient.Allergies", "Allergies"),
                ("Excel.Patient.MedicalHistory", "Medical History"),
                ("Excel.Patient.ReferringPhysician", "Referring Physician"),
            ],
            sampleRow: ["Ahmed Mohamed Ali", "Male", "1990-05-15", 35, "01012345678", "ahmed@example.com", "Cairo", "29801011234567", "A+", "None", "No known conditions", "Dr. Samer Ali"],
            referenceSheets:
            [
                ("Gender", [.. Gender.GetAll<Gender>().Select(g => g.Name)]),
                ("BloodType", [.. BloodType.GetAll<BloodType>().Select(b => b.Name)]),
            ],
            instructions:
            [
                ("Excel.Patient.Instructions.AutoCode", "Patient Code is generated automatically by the system - do not add it to the file."),
                ("Excel.Instructions.OneRowPerRecord", "Each row becomes one patient record. Do not edit or reorder the header row."),
                ("Excel.Patient.Instructions.AllowedValues", "Gender must be Male or Female; Blood Type one of A+, A-, B+, B-, AB+, AB-, O+, O- (both optional)."),
                ("Excel.Instructions.DateFormat", "Dates must use the YYYY-MM-DD format. Fill either Date of Birth or Age."),
                ("Excel.Patient.Instructions.DuplicateRule", "Rows whose National ID already exists are skipped and reported at the end."),
                ("Excel.Instructions.OptionalEmpty", "Leave optional columns empty when not applicable."),
                ("Excel.Instructions.FreshTemplate", "Always fill this downloaded template; older templates may be rejected."),
            ]);

        return Result.Success(new FileContentDto(content, "patients-import-template.xlsx", ExcelContentTypes.Xlsx));
    }
}

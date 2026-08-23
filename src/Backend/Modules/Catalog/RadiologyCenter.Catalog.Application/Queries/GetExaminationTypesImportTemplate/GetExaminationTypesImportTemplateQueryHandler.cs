using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Excel;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Catalog.Domain.Enumerations;

namespace RadiologyCenter.Catalog.Application.Queries.GetExaminationTypesImportTemplate;

public static class GetExaminationTypesImportTemplateQueryHandler
{
    public static Result<FileContentDto> Handle(
        GetExaminationTypesImportTemplateQuery query,
        IExcelService excelService)
    {
        var content = excelService.CreateTemplate(
            "ExaminationTypes",
            [
                ("Excel.ExamType.Name", "Name"),
                ("Excel.ExamType.Modality", "Modality"),
                ("Excel.ExamType.BodyPart", "Body Part"),
                ("Excel.ExamType.Duration", "Duration (min)"),
                ("Excel.ExamType.Price", "Price"),
                ("Excel.ExamType.RequiresPreparation", "Requires Preparation"),
                ("Excel.ExamType.RequiresConsent", "Requires Consent"),
            ],
            sampleRow: ["Chest X-Ray", Modality.XRay.Name, "Chest", 10, 150, "No", "No"],
            referenceSheets:
            [
                ("Modality", [.. Modality.GetAll<Modality>().Select(m => m.Name)]),
            ],
            instructions:
            [
                ("Excel.ExamType.Instructions.AutoCode", "Examination Type code is generated automatically by the system - do not add it to the file."),
                ("Excel.Instructions.OneRowPerRecord", "Each row becomes one examination type. Do not edit or reorder the header row."),
                ("Excel.ExamType.Instructions.AllowedValues", "Modality must exactly match a value from the hidden Modality sheet (e.g. XRay, CT, MRI)."),
                ("Excel.Instructions.YesNoFormat", "The Requires Preparation / Requires Consent columns accept only Yes or No."),
                ("Excel.Instructions.OptionalEmpty", "Leave optional columns empty when not applicable."),
                ("Excel.Instructions.FreshTemplate", "Always fill this downloaded template; older templates may be rejected."),
            ]);

        return Result.Success(new FileContentDto(content, "examination-types-import-template.xlsx", ExcelContentTypes.Xlsx));
    }
}

using Microsoft.Data.SqlClient;
using RadiologyCenter.BuildingBlocks.Application.Localization;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.Clinic.Extensions;
using RadiologyCenter.Examinations.Domain.Enumerations;
using RadiologyCenter.Reports.Domain.Enumerations;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Examinations;

public sealed record ReadingQueueRequest(int PageNumber=1,int PageSize=50,string? Search=null,bool IncludeFinalized=false);
public sealed record ReadingQueueItem(Guid Id,Guid PatientId,Guid ExaminationTypeId,string PatientName,string PatientCode,
    string ExamName,string StatusKey,DateTime? ScheduledAt,DateTime? CompletedAt,string Priority,string PriorityKey,
    string Indication,Guid? AssignedRadiologistId,Guid? AssignedTechnicianId,string ReportStatus,string ReportStatusKey,
    string? StudyInstanceUid,string? AccessionNumber);
public sealed record ReadingQueuePage(IReadOnlyList<ReadingQueueItem> Items,int TotalCount,int PendingCount,int ImagedCount,int PageNumber,int PageSize);

/// <summary>Read a bounded worklist with patient and report summaries in two SQL result sets.</summary>
public sealed class ReadingQueueOperations(IConfiguration configuration) : ClinicOperations
{
    [ClinicPermission(ExaminationsReadCode)]
    [ClinicPermission(ReportsReadCode)]
    [ClinicOperation("POST","examinations/reading-queue")]
    public async Task<ClinicResult> GetAsync([FromBody] ReadingQueueRequest request,CancellationToken ct)
    {
        if(request.PageNumber is <1 or >100000 || request.PageSize is <1 or >100 || request.Search?.Length>200)
            return BadRequest(new { Message="Use a valid worklist page and a search of at most 200 characters." });
        const string query="""
            WITH Queue AS (
                SELECT e.Id,e.PatientId,e.ExaminationTypeId,CONCAT_WS(' ',p.FirstName,NULLIF(p.MiddleName,''),p.LastName) AS PatientName,
                    p.PatientCode,t.Name AS ExamName,e.Status,e.ScheduledAt,e.CompletedAt,e.Priority,e.ClinicalIndication,
                    e.RadiologistId,e.TechnicianId,r.Status AS ReportStatus,e.StudyInstanceUID,e.AccessionNumber,e.CreatedAt
                FROM [Examinations].[Examinations] e
                JOIN [Patients].[Patients] p ON p.Id=e.PatientId AND p.IsDeleted=0
                JOIN [Catalog].[CatalogExaminationTypes] t ON t.Id=e.ExaminationTypeId
                LEFT JOIN [Reports].[RadiologyReports] r ON r.ExaminationId=e.Id
                WHERE e.IsDeleted=0 AND e.Status IN (3,4,5) AND (@history=1 OR r.Status IS NULL OR r.Status<>2)
            )
            """;
        const string filter="WHERE (@search=N'' OR PatientName LIKE @search ESCAPE '~' OR PatientCode LIKE @search ESCAPE '~' OR ExamName LIKE @search ESCAPE '~')";
        await using var db=new SqlConnection(configuration.GetConnectionString("DefaultConnection")); await db.OpenAsync(ct);
        await using var command=db.CreateCommand();
        command.CommandText=query+"SELECT COUNT(*),COALESCE(SUM(CASE WHEN ReportStatus IS NULL OR ReportStatus=1 THEN 1 ELSE 0 END),0),COALESCE(SUM(CASE WHEN StudyInstanceUID IS NOT NULL AND StudyInstanceUID<>'' THEN 1 ELSE 0 END),0) FROM Queue "+filter+"; "+query+
            "SELECT * FROM Queue "+filter+" ORDER BY Priority DESC,COALESCE(ScheduledAt,CompletedAt,CreatedAt),Id OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;";
        var search=request.Search?.Trim()??"";
        command.Parameters.AddWithValue("@search",search.Length==0?"":"%"+search.Replace("~","~~").Replace("%","~%").Replace("_","~_").Replace("[","~[")+"%");
        command.Parameters.AddWithValue("@history",request.IncludeFinalized); command.Parameters.AddWithValue("@skip",(request.PageNumber-1)*request.PageSize);
        command.Parameters.AddWithValue("@take",request.PageSize);
        await using var reader=await command.ExecuteReaderAsync(ct); await reader.ReadAsync(ct);
        var total=reader.GetInt32(0);var pending=reader.GetInt32(1);var imaged=reader.GetInt32(2);
        await reader.NextResultAsync(ct); var items=new List<ReadingQueueItem>();
        while(await reader.ReadAsync(ct))
        {
            string? Text(string column)=>reader.IsDBNull(reader.GetOrdinal(column))?null:reader.GetString(reader.GetOrdinal(column));
            Guid? Id(string column)=>reader.IsDBNull(reader.GetOrdinal(column))?null:reader.GetGuid(reader.GetOrdinal(column));
            DateTime? Date(string column)=>reader.IsDBNull(reader.GetOrdinal(column))?null:DateTime.SpecifyKind(reader.GetDateTime(reader.GetOrdinal(column)),DateTimeKind.Utc);
            var priority=ExaminationPriority.FromValue<ExaminationPriority>(reader.GetInt32(reader.GetOrdinal("Priority")));
            var status=ExaminationStatus.FromValue<ExaminationStatus>(reader.GetInt32(reader.GetOrdinal("Status")));
            var report=reader.IsDBNull(reader.GetOrdinal("ReportStatus"))?null:ReportStatus.FromValue<ReportStatus>(reader.GetInt32(reader.GetOrdinal("ReportStatus")));
            items.Add(new(Id("Id")!.Value,Id("PatientId")!.Value,Id("ExaminationTypeId")!.Value,Text("PatientName")!,Text("PatientCode")!,
                Text("ExamName")!,status.Name,Date("ScheduledAt"),Date("CompletedAt"),priority.LocalizedName(),priority.Name,
                Text("ClinicalIndication")!,Id("RadiologistId"),Id("TechnicianId"),report?.LocalizedName()??"New",report?.Name??"New",
                Text("StudyInstanceUID"),Text("AccessionNumber")));
        }
        return Result.Success(new ReadingQueuePage(items,total,pending,imaged,request.PageNumber,request.PageSize)).ToActionResult();
    }
}

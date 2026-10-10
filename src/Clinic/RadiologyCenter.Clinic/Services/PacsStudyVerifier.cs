using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Clinic.Services;

public sealed class PacsStudyVerifier(IHttpClientFactory clients, IConfiguration configuration) : IPacsStudyVerifier
{
    public async Task<Result> VerifyAsync(string studyUid, string patientCode, string accession, CancellationToken ct)
    {
        if (!Uri.TryCreate(configuration["Pacs:OrthancUrl"], UriKind.Absolute, out var root) || (root.Scheme != "https" && !(root.Scheme == "http" && root.IsLoopback))
            || string.IsNullOrWhiteSpace(configuration["Pacs:Password"]) || string.IsNullOrWhiteSpace(configuration["Pacs:UserName"]))
            return Result.Failure(Error.Conflict("Examinations.PacsUnavailable", "The clinic image server is not configured."));
        using var request = new HttpRequestMessage(HttpMethod.Get,
            root.AbsoluteUri.TrimEnd('/') + "/dicom-web/studies?StudyInstanceUID=" + Uri.EscapeDataString(studyUid)
            + "&PatientID=" + Uri.EscapeDataString(patientCode) + "&AccessionNumber=" + Uri.EscapeDataString(accession));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(
            configuration["Pacs:UserName"] + ":" + configuration["Pacs:Password"])));
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/dicom+json"));
        try
        {
            using var client = clients.CreateClient("Pacs");
            using var response = await client.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();
            await using var stream = await response.Content.ReadAsStreamAsync(ct);
            using var json = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
            if (json.RootElement.ValueKind == JsonValueKind.Array && json.RootElement.GetArrayLength() == 1)
            {
                var study = json.RootElement[0];
                if (Tag(study, "0020000D") == studyUid && Tag(study, "00100020") == patientCode
                    && Tag(study, "00080050") == accession) return Result.Success();
            }
            return Result.Failure(Error.Validation("Examinations.PacsIdentityMismatch", "The study must uniquely match this patient and examination accession."));
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException or TaskCanceledException)
        {
            return Result.Failure(Error.Conflict("Examinations.PacsUnavailable", "The clinic image server could not verify this study. Try again when it is available."));
        }
    }

    private static string? Tag(JsonElement study, string name) =>
        study.ValueKind == JsonValueKind.Object && study.TryGetProperty(name, out var tag)
        && tag.ValueKind == JsonValueKind.Object && tag.TryGetProperty("Value", out var values)
        && values.ValueKind == JsonValueKind.Array && values.GetArrayLength() == 1 && values[0].ValueKind == JsonValueKind.String ? values[0].GetString() : null;
}

using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using RadiologyCenter.Clinic.Services;

namespace Tests;

public class PacsStudyVerifierTests
{
    private const string MatchingStudy = """
        {"0020000D":{"Value":["1.2.3"]},"00100020":{"Value":["P-100"]},"00080050":{"Value":["EX-2026-00000001"]}}
        """;

    [Fact]
    public async Task Verify_UniqueMatchingStudy_Succeeds()
    {
        var result = await Verifier("[" + MatchingStudy + "]").VerifyAsync("1.2.3", "P-100", "EX-2026-00000001", default);
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("[]")]
    [InlineData("[{},{}]")]
    [InlineData("[null]")]
    [InlineData("[{\"00100020\":{\"Value\":[100]}}]")]
    public async Task Verify_MissingAmbiguousOrMalformedIdentity_Rejects(string json)
    {
        var result = await Verifier(json).VerifyAsync("1.2.3", "P-100", "EX-2026-00000001", default);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task Verify_WrongPatient_RejectsEvenWhenStudyAndAccessionMatch()
    {
        var result = await Verifier("[" + MatchingStudy.Replace("P-100", "P-200") + "]")
            .VerifyAsync("1.2.3", "P-100", "EX-2026-00000001", default);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task Verify_WrongAccession_RejectsEvenWhenPatientAndStudyMatch()
    {
        var result = await Verifier("[" + MatchingStudy + "]").VerifyAsync("1.2.3", "P-100", "EX-2026-00000002", default);
        Assert.True(result.IsFailure);
    }

    private static PacsStudyVerifier Verifier(string json) => new(new StubClients(json),
        new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Pacs:OrthancUrl"] = "http://127.0.0.1:18042", ["Pacs:UserName"] = "test", ["Pacs:Password"] = "test-only"
        }).Build());

    private sealed class StubClients(string json) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(new StubHandler(json));
    }

    private sealed class StubHandler(string json) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json, Encoding.UTF8, "application/dicom+json") });
    }
}

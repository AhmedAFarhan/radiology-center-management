using System.Security.Claims;
using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Clinic.Dispatching;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ClinicOperationAttribute(string kind, string name) : Attribute
{
    public string Kind { get; } = kind;
    public string Name { get; } = name;
}
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class ClinicPermissionAttribute(string code) : Attribute { public string Code { get; } = code; }
[AttributeUsage(AttributeTargets.Method)]
public sealed class RequireSignInAttribute : Attribute;
[AttributeUsage(AttributeTargets.Method)]
public sealed class AllowUnauthenticatedAttribute : Attribute;
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromBodyAttribute : Attribute;
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromQueryAttribute : Attribute;
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromFormAttribute : Attribute;
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromServicesAttribute : Attribute;

public sealed record ClinicFile(byte[] Content, string ContentType, string FileName);
public sealed record ClinicResult(int StatusCode, object? Value = null)
{
    public bool IsSuccess => StatusCode is >= 200 and < 300;
}
public sealed record ClinicUpload(string Name, string FileName, string ContentType, byte[] Content);
public sealed record ClinicRequest(string Kind, string Operation, string? Body = null,
    IReadOnlyDictionary<string, string>? Fields = null, ClinicUpload? Upload = null,
    string? AccessToken = null, string Culture = "en");

public abstract class ClinicOperations
{
    public ClaimsPrincipal User { get; internal set; } = new(new ClaimsIdentity());
    protected ClinicResult File(byte[] content, string contentType, string name) => new(200, new ClinicFile(content, contentType, name));
    protected ClinicResult NotFound() => new(404, ApiResponse.Fail("The requested record could not be found."));
    protected ClinicResult BadRequest(object value)
    {
        var message = value.GetType().GetProperty("Message")?.GetValue(value)?.ToString() ?? "Invalid request.";
        return new(400, ApiResponse.Fail(message));
    }
}

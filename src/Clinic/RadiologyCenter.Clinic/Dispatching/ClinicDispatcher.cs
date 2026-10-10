using System.Globalization;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Localization;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.Clinic.Security;

namespace RadiologyCenter.Clinic.Dispatching;

/// <summary>Calls application operations in the current process. No HTTP server or network client is involved.</summary>
public sealed class ClinicDispatcher(IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<ClinicDispatcher> logger, ClinicSessionGuard session)
{
    public static JsonSerializerOptions JsonOptions { get; } = new()
    {
        PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };
    public static IReadOnlyList<(Type Service, MethodInfo Method, ClinicOperationAttribute Operation)> Operations { get; } =
        typeof(ClinicDispatcher).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ClinicOperations)))
            .SelectMany(t => t.GetMethods().Select(m => (Service: t, Method: m, Operation: m.GetCustomAttribute<ClinicOperationAttribute>())))
            .Where(x => x.Operation is not null).Select(x => (x.Service, x.Method, x.Operation!)).ToArray();

    public async Task<ClinicResult> DispatchAsync(ClinicRequest request, CancellationToken ct = default)
    {
        using var operation = session.EnterOperation();
        if (operation is null) return Failure(401, "The clinic session is closing. Sign in again to reconnect.");
        var oldCulture = CultureInfo.CurrentCulture;
        var oldUiCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(request.Culture == "ar" ? "ar" : "en");
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;
            var uri = new Uri("http://operation/" + request.Operation.TrimStart('/'));
            var path = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/'));
            if (path.StartsWith("api/", StringComparison.OrdinalIgnoreCase)) path = path[4..];
            var match = Operations.Where(x => x.Operation.Kind.Equals(request.Kind, StringComparison.OrdinalIgnoreCase))
                .Select(x => (Entry: x, Values: Match(x.Operation.Name, path)))
                .Where(x => x.Values is not null).OrderBy(x => x.Entry.Operation.Name.Count(c => c == '{'))
                .ThenByDescending(x => x.Entry.Operation.Name.Length).FirstOrDefault();
            if (match.Values is null) return Failure(404, "The requested clinic operation does not exist.");
            await using var scope = scopeFactory.CreateAsyncScope();
            if(configuration.GetValue<bool>("DatabaseAuthorization:Required")
                && !await scope.ServiceProvider.GetRequiredService<ClinicSessionGuard>().IsActiveAsync(ct))
                return Failure(401,"The clinic database session was interrupted. Sign in again to reconnect.");
            var currentUser = scope.ServiceProvider.GetRequiredService<ClinicCurrentUser>();
            currentUser.Principal = configuration.GetValue<bool>("DatabaseAuthorization:Required")
                ? await scope.ServiceProvider.GetRequiredService<DatabasePrincipalReader>().ReadAsync(ct)
                : ValidateDevelopmentToken(request.AccessToken);
            if(configuration.GetValue<bool>("DatabaseAuthorization:Required") && currentUser.IsAuthenticated
                && currentUser.Principal.FindFirst("sqlCredentialVersion")?.Value
                    != scope.ServiceProvider.GetRequiredService<ClinicSessionGuard>().CredentialVersion)
                return Failure(401,"Your password was changed. Sign in again with the current password.");
            var anonymous = match.Entry.Method.IsDefined(typeof(AllowUnauthenticatedAttribute));
            if (configuration.GetValue<bool>("DatabaseAuthorization:Required") && path.StartsWith("auth/",StringComparison.OrdinalIgnoreCase))
                return Failure(403,"Sign-in and password changes use your individual SQL identity.");
            if (!anonymous && !currentUser.IsAuthenticated) return Failure(401, "Sign in to continue.");
            if (currentUser.Principal.HasClaim("mustChangePassword","true")) return Failure(403,"Change your initial password before using clinic records.");
            if (match.Entry.Method.GetCustomAttributes<ClinicPermissionAttribute>().Any(permission => !currentUser.HasPermission(permission.Code)))
                return Failure(403, "You do not have permission to perform this operation.");
            var service = (ClinicOperations)ActivatorUtilities.CreateInstance(scope.ServiceProvider, match.Entry.Service);
            service.User = currentUser.Principal;
            var query = ParseQuery(uri.Query);
            var arguments = match.Entry.Method.GetParameters().Select(p => Bind(p, request, match.Values, query, scope.ServiceProvider, ct)).ToArray();
            object? value;
            try { value = match.Entry.Method.Invoke(service, arguments); }
            catch (TargetInvocationException ex) when (ex.InnerException is not null)
            { ExceptionDispatchInfo.Capture(ex.InnerException).Throw(); throw; }
            if (value is Task task)
            {
                await task;
                value = task.GetType().GetProperty("Result")?.GetValue(task);
            }
            return value as ClinicResult ?? Failure(500, "The clinic operation returned an invalid result.");
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
        catch (FluentValidation.ValidationException ex)
        {
            var details = ex.Errors.Select(e => new { e.PropertyName, e.ErrorCode, ErrorMessage = Translator.LocalizeCode(e.ErrorCode, e.ErrorMessage) });
            return new(400, ApiResponse.Fail("Validation failed.", new ApiError { Code = ApiErrorCodes.Validation, Message = "Validation failed.", Details = details }));
        }
        catch (NotFoundException ex) { return Failure(404, Translator.LocalizeCode(ex.Code ?? string.Empty, ex.Message)); }
        catch (ConcurrencyException ex) { return Failure(409, Translator.LocalizeCode(ex.Code ?? string.Empty, ex.Message)); }
        catch (BusinessRuleViolationException ex) { return Failure(409, Translator.LocalizeCode(ex.Code ?? string.Empty, ex.Message)); }
        catch (ValidationException ex) { return Failure(400, ex.Message); }
        catch (DomainException ex) { return Failure(400, Translator.LocalizeCode(ex.Code ?? string.Empty, ex.Message)); }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        { return Failure(409, "A record with the same unique value already exists. Refresh and try again."); }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number:229 or 230 or 33504 })
        { return Failure(403,"Your database account does not permit this operation."); }
        catch (SqlException ex) when (ex.Number is 18456 or 18488 or 18487)
        { return Failure(401,"The clinic credentials were changed, locked or expired. Sign in again."); }
        catch (SqlException ex) when (ex.Number is 229 or 230 or 916 or 33504 or 51003) { return Failure(403, "Your database account does not permit this operation."); }
        catch (Exception ex) when (ex is JsonException or FormatException or ArgumentException)
        { return Failure(400, "The supplied values are invalid."); }
        catch (Exception ex)
        {
            logger.LogError("Clinic operation failed ({FailureType}, SQL {SqlNumber}). {Stack}",
                ex.GetType().Name, (ex as SqlException ?? ex.InnerException as SqlException)?.Number, ex.StackTrace);
            return Failure(500, "The operation could not be completed. Check the clinic database connection and try again.");
        }
        finally { CultureInfo.CurrentCulture = oldCulture; CultureInfo.CurrentUICulture = oldUiCulture; }
    }

    private ClaimsPrincipal ValidateDevelopmentToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token)) return new(new ClaimsIdentity());
        try
        {
            var options = new TokenValidationParameters
            {
                ValidateIssuer = true, ValidIssuer = configuration["Jwt:Issuer"],
                ValidateAudience = true, ValidAudience = configuration["Jwt:Audience"],
                ValidateLifetime = true, ValidateIssuerSigningKey = true, ClockSkew = TimeSpan.FromSeconds(30),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!))
            };
            return new JwtSecurityTokenHandler().ValidateToken(token, options, out _);
        }
        catch (SecurityTokenException) { return new(new ClaimsIdentity()); }
    }

    private static Dictionary<string, string>? Match(string template, string path)
    {
        var names = new List<string>();
        var pattern = new StringBuilder();
        var position = 0;
        foreach (Match parameter in Regex.Matches(template, @"\{([^}:]+)(?::([^}]+))?\}"))
        {
            pattern.Append(Regex.Escape(template[position..parameter.Index]));
            names.Add(parameter.Groups[1].Value);
            pattern.Append(parameter.Groups[2].Value == "guid" ? "([0-9a-fA-F-]{36})" : "([^/]+)");
            position = parameter.Index + parameter.Length;
        }
        pattern.Append(Regex.Escape(template[position..]));
        var match = Regex.Match(path, "^" + pattern + "$", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));
        if (!match.Success) return null;
        return names.Select((name, index) => (name, value: match.Groups[index + 1].Value)).ToDictionary(x => x.name, x => x.value, StringComparer.OrdinalIgnoreCase);
    }

    private static Dictionary<string, string> ParseQuery(string query) => query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries)
        .Select(x => x.Split('=', 2)).GroupBy(x => Uri.UnescapeDataString(x[0]), StringComparer.OrdinalIgnoreCase)
        .ToDictionary(x => x.Key, x => Uri.UnescapeDataString(x.Last().Length == 2 ? x.Last()[1].Replace('+', ' ') : ""), StringComparer.OrdinalIgnoreCase);

    private static object? Bind(ParameterInfo p, ClinicRequest request, IReadOnlyDictionary<string, string> route,
        IReadOnlyDictionary<string, string> query, IServiceProvider services, CancellationToken ct)
    {
        if (p.ParameterType == typeof(CancellationToken)) return ct;
        if (p.IsDefined(typeof(FromServicesAttribute))) return services.GetRequiredService(p.ParameterType);
        if (p.IsDefined(typeof(FromBodyAttribute))) return JsonSerializer.Deserialize(request.Body ?? "{}", p.ParameterType, JsonOptions);
        if (p.IsDefined(typeof(FromFormAttribute)))
        {
            var form = Activator.CreateInstance(p.ParameterType)!;
            foreach (var property in p.ParameterType.GetProperties().Where(x => x.CanWrite))
            {
                if (typeof(IFormFile).IsAssignableFrom(property.PropertyType) && request.Upload is { } upload)
                    property.SetValue(form, new FormFile(new MemoryStream(upload.Content, writable: false), 0, upload.Content.Length, upload.Name, upload.FileName)
                    { Headers = new HeaderDictionary(), ContentType = upload.ContentType });
                else if (request.Fields is not null && request.Fields.TryGetValue(property.Name, out var field))
                    property.SetValue(form, ConvertValue(field, property.PropertyType));
            }
            return form;
        }
        if (route.TryGetValue(p.Name!, out var value) || query.TryGetValue(p.Name!, out value)) return ConvertValue(value, p.ParameterType);
        if (p.HasDefaultValue) return p.DefaultValue;
        if (Nullable.GetUnderlyingType(p.ParameterType) is not null || !p.ParameterType.IsValueType) return null;
        throw new ArgumentException("A required operation value is missing.");
    }

    private static object? ConvertValue(string value, Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        if (type == typeof(string)) return value;
        if (type == typeof(Guid)) return Guid.Parse(value);
        if (type.IsEnum) return Enum.Parse(type, value, ignoreCase: true);
        return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
    }
    private static ClinicResult Failure(int code, string message) => new(code, ApiResponse.Fail(message,
        new ApiError { Code = code.ToString(CultureInfo.InvariantCulture), Message = message }));
}

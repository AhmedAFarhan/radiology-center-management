using System.Net;
using System.Text.Json;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;

namespace RadiologyCenter.Localhost.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found");
            await WriteResponse(context, HttpStatusCode.NotFound, new { ex.EntityName, ex.Key, ex.Message });
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed");
            await WriteResponse(context, HttpStatusCode.BadRequest, new { ex.Errors, ex.Message });
        }
        catch (BusinessRuleViolationException ex)
        {
            _logger.LogWarning(ex, "Business rule violated");
            await WriteResponse(context, HttpStatusCode.Conflict, new { ex.Rule, ex.Message });
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain exception");
            await WriteResponse(context, HttpStatusCode.BadRequest, new { ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteResponse(context, HttpStatusCode.InternalServerError, new { Message = "An unexpected error occurred." });
        }
    }

    private static async Task WriteResponse(HttpContext context, HttpStatusCode statusCode, object body)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}

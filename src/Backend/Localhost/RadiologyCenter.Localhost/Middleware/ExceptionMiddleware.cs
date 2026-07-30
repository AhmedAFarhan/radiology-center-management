using System.Net;
using RadiologyCenter.BuildingBlocks.Application.Common;
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
            await WriteResponse(context, HttpStatusCode.NotFound,
                ApiResponse.Fail(ex.Message, ApiError.FromException(ex, "NotFound")));
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed");
            await WriteResponse(context, HttpStatusCode.BadRequest,
                ApiResponse.Fail(ex.Message, new ApiError { Code = "Validation", Message = ex.Message, Details = ex.Errors }));
        }
        catch (BusinessRuleViolationException ex)
        {
            _logger.LogWarning(ex, "Business rule violated");
            await WriteResponse(context, HttpStatusCode.Conflict,
                ApiResponse.Fail(ex.Message, ApiError.FromException(ex, "Conflict")));
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain exception");
            await WriteResponse(context, HttpStatusCode.BadRequest,
                ApiResponse.Fail(ex.Message, ApiError.FromException(ex, "DomainError")));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteResponse(context, HttpStatusCode.InternalServerError,
                ApiResponse.Fail("An unexpected error occurred.", ApiError.FromException(ex, "InternalError")));
        }
    }

    private static async Task WriteResponse(HttpContext context, HttpStatusCode statusCode, ApiResponse response)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsJsonAsync(response);
    }
}

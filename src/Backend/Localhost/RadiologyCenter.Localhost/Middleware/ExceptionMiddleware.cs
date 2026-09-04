using System.Net;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Localization;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.BuildingBlocks.Domain.Localization;

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
            var message = Translator.LocalizeCode(ex.Code, ex.Message);
            await WriteResponse(context, HttpStatusCode.NotFound,
                ApiResponse.Fail(message, ApiError.FromException(ex, ApiErrorCodes.NotFound, message)));
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed");
            var message = Translator.LocalizeCode(MessageCodes.Shared.ValidationFailed, ex.Message);
            var details = ex.Errors.ToDictionary(
                kvp => kvp.Key,
                kvp => (object)kvp.Value.ToArray());
            await WriteResponse(context, HttpStatusCode.BadRequest,
                ApiResponse.Fail(message, new ApiError { Code = ApiErrorCodes.Validation, Message = message, Details = details }));
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning(ex, "Fluent validation failed");
            var details = ex.Errors.Select(e => new
            {
                e.PropertyName,
                e.ErrorCode,
                ErrorMessage = Translator.LocalizeCode(e.ErrorCode, e.ErrorMessage),
            });
            var message = Translator.LocalizeCode(MessageCodes.Shared.ValidationFailed, ex.Message);
            await WriteResponse(context, HttpStatusCode.BadRequest,
                ApiResponse.Fail(message, new ApiError { Code = ApiErrorCodes.Validation, Message = message, Details = details }));
        }
        catch (BusinessRuleViolationException ex)
        {
            _logger.LogWarning(ex, "Business rule violated");
            var message = Translator.LocalizeCode(ex.Code, ex.Message);
            await WriteResponse(context, HttpStatusCode.Conflict,
                ApiResponse.Fail(message, ApiError.FromException(ex, ex.Code ?? ApiErrorCodes.Conflict, message)));
        }
        catch (ConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict");
            var message = Translator.LocalizeCode(ex.Code, ex.Message);
            await WriteResponse(context, HttpStatusCode.Conflict,
                ApiResponse.Fail(message, ApiError.FromException(ex, ApiErrorCodes.Conflict, message)));
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain exception");
            var message = Translator.LocalizeCode(ex.Code, ex.Message);
            await WriteResponse(context, HttpStatusCode.BadRequest,
                ApiResponse.Fail(message, ApiError.FromException(ex, ex.Code ?? ApiErrorCodes.DomainError, message)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            var message = Translator.LocalizeCode(MessageCodes.Shared.UnexpectedError);
            await WriteResponse(context, HttpStatusCode.InternalServerError,
                ApiResponse.Fail(message, new ApiError { Code = ApiErrorCodes.InternalError, Message = message }));
        }
    }

    private static async Task WriteResponse(HttpContext context, HttpStatusCode statusCode, ApiResponse response)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsJsonAsync(response);
    }
}

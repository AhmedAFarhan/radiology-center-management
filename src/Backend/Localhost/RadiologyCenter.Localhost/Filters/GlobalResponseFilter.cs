using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Localhost.Filters;

public class GlobalResponseFilter : IAsyncResultFilter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult)
        {
            var statusCode = objectResult.StatusCode ?? context.HttpContext.Response.StatusCode;

            if (statusCode >= 200 && statusCode < 300 && objectResult.Value is not ApiResponse and not ApiResponse<object>)
            {
                var wrapped = ApiResponse.Ok(objectResult.Value);
                objectResult.Value = wrapped;
                objectResult.DeclaredType = wrapped.GetType();
            }
        }

        await next();
    }
}

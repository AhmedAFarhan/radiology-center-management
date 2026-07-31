using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Results;

namespace RadiologyCenter.Localhost.Controllers.Identity;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result) =>
        result.IsSuccess
            ? new OkObjectResult(ApiResponse.FromResult(result))
            : result.Error?.Code switch
            {
                "NotFound" => new NotFoundObjectResult(ApiResponse.FromResult(result)),
                "Unauthorized" => new UnauthorizedObjectResult(ApiResponse.FromResult(result)),
                "Conflict" => new ConflictObjectResult(ApiResponse.FromResult(result)),
                _ => new BadRequestObjectResult(ApiResponse.FromResult(result))
            };

    public static IActionResult ToActionResult<T>(this Result<T> result) =>
        result.IsSuccess
            ? new OkObjectResult(ApiResponse.FromResult(result))
            : result.Error?.Code switch
            {
                "NotFound" => new NotFoundObjectResult(ApiResponse.FromResult(result)),
                "Unauthorized" => new UnauthorizedObjectResult(ApiResponse.FromResult(result)),
                "Conflict" => new ConflictObjectResult(ApiResponse.FromResult(result)),
                _ => new BadRequestObjectResult(ApiResponse.FromResult(result))
            };
}

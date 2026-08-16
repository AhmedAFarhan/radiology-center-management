using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Localization;
using RadiologyCenter.BuildingBlocks.Domain.Results;

namespace RadiologyCenter.Localhost.Extensions;

public static class ResultExtensions
{
    private static ApiResponse Localize(Result result) =>
        ApiResponse.FromResult(result, localized: true);

    public static IActionResult ToActionResult(this Result result) =>
        result.IsSuccess
            ? new OkObjectResult(ApiResponse.FromResult(result))
            : result.Error?.Kind switch
            {
                ErrorKind.NotFound => new NotFoundObjectResult(Localize(result)),
                ErrorKind.Unauthorized => new UnauthorizedObjectResult(Localize(result)),
                ErrorKind.Forbidden => new ObjectResult(Localize(result)) { StatusCode = StatusCodes.Status403Forbidden },
                ErrorKind.Conflict => new ConflictObjectResult(Localize(result)),
                ErrorKind.LockedOut => new ObjectResult(Localize(result)) { StatusCode = StatusCodes.Status423Locked },
                _ => new BadRequestObjectResult(Localize(result))
            };

    public static IActionResult ToActionResult<T>(this Result<T> result) =>
        result.IsSuccess
            ? new OkObjectResult(ApiResponse.FromResult(result))
            : result.Error?.Kind switch
            {
                ErrorKind.NotFound => new NotFoundObjectResult(Localize(result)),
                ErrorKind.Unauthorized => new UnauthorizedObjectResult(Localize(result)),
                ErrorKind.Forbidden => new ObjectResult(Localize(result)) { StatusCode = StatusCodes.Status403Forbidden },
                ErrorKind.Conflict => new ConflictObjectResult(Localize(result)),
                ErrorKind.LockedOut => new ObjectResult(Localize(result)) { StatusCode = StatusCodes.Status423Locked },
                _ => new BadRequestObjectResult(Localize(result))
            };
}

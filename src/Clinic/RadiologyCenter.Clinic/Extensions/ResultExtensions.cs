using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Results;

namespace RadiologyCenter.Clinic.Extensions;

public static class ResultExtensions
{
    private static int Status(Result result) => result.IsSuccess ? 200 : result.Error?.Kind switch
    {
        ErrorKind.NotFound => 404, ErrorKind.Unauthorized => 401, ErrorKind.Forbidden => 403,
        ErrorKind.Conflict => 409, ErrorKind.LockedOut => 423, _ => 400
    };
    public static ClinicResult ToActionResult(this Result result) => new(Status(result), ApiResponse.FromResult(result, localized: true));
    public static ClinicResult ToActionResult<T>(this Result<T> result) => new(Status(result), ApiResponse.FromResult(result, localized: true));
}

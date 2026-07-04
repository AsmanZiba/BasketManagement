using Microsoft.AspNetCore.Mvc;
using BasketManagement.Application.Common;

namespace BasketManagement.API.Extensions;
public static class ApiResultExtensions
{
    public static IActionResult ToApiResult<T>(this ServiceResult<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(new ApiResult { IsSuccess = true, Data = result.Data });
        else
            return new BadRequestObjectResult(new ApiResult { IsSuccess = false, ErrorMessage = result.ErrorMessage, ErrorCode = result.ErrorCode });
    }

    public static IActionResult ToApiResult(this ServiceResult result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(new ApiResult { IsSuccess = true });
        else
            return new BadRequestObjectResult(new ApiResult { IsSuccess = false, ErrorMessage = result.ErrorMessage, ErrorCode = result.ErrorCode });
    }
}
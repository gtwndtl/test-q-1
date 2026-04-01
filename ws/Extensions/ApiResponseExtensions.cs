using Microsoft.AspNetCore.Mvc;
using ws.Core;

namespace ws.Extensions;

public static class ApiResponseExtensions
{
    public static IActionResult Success<T>(this ControllerBase controller, T data, string? message = null)
    {
        return controller.Ok(ApiResponse<T>.Ok(data, message));
    }

    public static IActionResult Success(this ControllerBase controller, string? message = null)
    {
        return controller.Ok(new ApiResponse<object> { Success = true, Message = message });
    }

    public static IActionResult SuccessWithExtraFields<T>(this ControllerBase controller, T data, string message, Dictionary<string, object> extra)
    {
        var response = ApiResponse<T>.Ok(data, message);
        response.Extra = extra;
        return controller.Ok(response);
    }

    public static IActionResult Fail(this ControllerBase controller, string message)
    {
        return controller.BadRequest(ApiResponse<object>.Fail(message));
    }
}

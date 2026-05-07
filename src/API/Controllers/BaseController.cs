using Interrapidisimo.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Interrapidisimo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[EnableRateLimiting("api")]
public abstract class BaseController : ControllerBase
{
    protected IActionResult OkResult<T>(T data, string? message = null) =>
        Ok(ApiResponse<T>.Ok(data, message, HttpContext.TraceIdentifier));

    protected IActionResult CreatedResult<T>(T data, string routeName, object routeValues, string? message = null) =>
        CreatedAtRoute(routeName, routeValues, ApiResponse<T>.Ok(data, message, HttpContext.TraceIdentifier));

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess) return Ok(ApiResponse<T>.Ok(result.Value!, traceId: HttpContext.TraceIdentifier));

        return result.ErrorCode switch
        {
            "NOT_FOUND" => NotFound(ApiResponse<T>.Fail(result.Error!, HttpContext.TraceIdentifier)),
            "CONFLICT" => Conflict(ApiResponse<T>.Fail(result.Error!, HttpContext.TraceIdentifier)),
            "BUSINESS_RULE" => UnprocessableEntity(ApiResponse<T>.Fail(result.Error!, HttpContext.TraceIdentifier)),
            _ => BadRequest(ApiResponse<T>.Fail(result.Errors.Any() ? result.Errors : [result.Error!], HttpContext.TraceIdentifier))
        };
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess) return NoContent();

        return result.ErrorCode switch
        {
            "NOT_FOUND" => NotFound(ApiResponse.Fail(result.Error!, HttpContext.TraceIdentifier)),
            "CONFLICT" => Conflict(ApiResponse.Fail(result.Error!, HttpContext.TraceIdentifier)),
            "BUSINESS_RULE" => UnprocessableEntity(ApiResponse.Fail(result.Error!, HttpContext.TraceIdentifier)),
            _ => BadRequest(ApiResponse.Fail(result.Errors.Any() ? result.Errors : [result.Error!], HttpContext.TraceIdentifier))
        };
    }
}

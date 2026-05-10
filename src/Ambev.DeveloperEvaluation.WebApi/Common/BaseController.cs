using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
    protected int GetCurrentUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new NullReferenceException());

    protected string GetCurrentUserEmail() =>
        User.FindFirst(ClaimTypes.Email)?.Value ?? throw new NullReferenceException();

    protected IActionResult Ok<T>(T data) =>
            base.Ok(new ApiResponseWithData<T> { Data = data, Success = true });

    protected IActionResult Created<T>(string routeName, object routeValues, T data) =>
        base.CreatedAtRoute(routeName, routeValues, new ApiResponseWithData<T> { Data = data, Success = true });

    protected IActionResult BadRequest(string message) =>
        base.BadRequest(new ApiResponse { Message = message, Success = false });

    protected IActionResult NotFound(string message = "Resource not found") =>
        base.NotFound(new ApiResponse { Message = message, Success = false });

    protected IActionResult OkPaginated<T>(PaginatedList<T> pagedList) =>
            base.Ok(new PaginatedResponse<T>
            {
                Data = pagedList,
                CurrentPage = pagedList.CurrentPage,
                TotalPages = pagedList.TotalPages,
                TotalItems = pagedList.TotalItems,
                Success = true
            });

    protected QueryParameters GetQueryParameters()
    {
        var query = Request.Query;

        var filters = query
            .Where(parameter => !IsReservedQueryParameter(parameter.Key))
            .ToDictionary(
                parameter => parameter.Key,
                parameter => (string?)parameter.Value.FirstOrDefault(),
                StringComparer.OrdinalIgnoreCase);

        return new QueryParameters
        {
            Page = TryGetIntQueryValue("_page", 1),
            Size = TryGetIntQueryValue("_size", 10),
            Order = query.TryGetValue("_order", out var order) ? order.FirstOrDefault() : null,
            Filters = filters
        };
    }

    private int TryGetIntQueryValue(string key, int defaultValue)
    {
        if (!Request.Query.TryGetValue(key, out var value))
            return defaultValue;

        return int.TryParse(value.FirstOrDefault(), out var parsedValue)
            ? parsedValue
            : defaultValue;
    }

    private static bool IsReservedQueryParameter(string key)
    {
        return key.Equals("_page", StringComparison.OrdinalIgnoreCase)
               || key.Equals("_size", StringComparison.OrdinalIgnoreCase)
               || key.Equals("_order", StringComparison.OrdinalIgnoreCase);
    }
}

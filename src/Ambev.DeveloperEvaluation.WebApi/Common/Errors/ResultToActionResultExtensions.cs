using Ambev.DeveloperEvaluation.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Common.Errors;

public static class ResultToActionResultExtensions
{
    public static IActionResult ToActionResult<T>(
        this Result<T> result,
        Func<T, IActionResult> onSuccess)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(onSuccess);

        return result.IsSuccess
            ? onSuccess(result.Value)
            : ToErrorActionResult(result.Error);
    }

    public static ObjectResult ToErrorActionResult(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return new ObjectResult(ToErrorResponse(error))
        {
            StatusCode = GetStatusCode(error)
        };
    }

    private static ErrorResponse ToErrorResponse(Error error)
    {
        return new ErrorResponse
        {
            Type = error.Type,
            Error = error.ErrorMessage,
            Detail = error.Detail
        };
    }

    private static int GetStatusCode(Error error)
    {
        return error.Type switch
        {
            "ValidationError" => StatusCodes.Status400BadRequest,
            "DomainRuleViolation" => StatusCodes.Status400BadRequest,
            "ResourceNotFound" => StatusCodes.Status404NotFound,
            "Conflict" => StatusCodes.Status409Conflict,
            "UnexpectedError" => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Ambev.DeveloperEvaluation.Common.Results;
using Ambev.DeveloperEvaluation.WebApi.Common.Errors;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

[ExcludeFromCodeCoverage]
public sealed class GlobalExceptionMiddleware
{
    private const string UnexpectedErrorMessage = "Unexpected error";
    private const string UnexpectedErrorDetail = "An unexpected error occurred while processing the request.";

    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unexpected error while processing request.");
            await HandleUnexpectedExceptionAsync(context);
        }
    }

    private static Task HandleUnexpectedExceptionAsync(HttpContext context)
    {
        var error = Error.Unexpected(UnexpectedErrorMessage, UnexpectedErrorDetail);
        var response = new ErrorResponse
        {
            Type = error.Type,
            Error = error.ErrorMessage,
            Detail = error.Detail
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}

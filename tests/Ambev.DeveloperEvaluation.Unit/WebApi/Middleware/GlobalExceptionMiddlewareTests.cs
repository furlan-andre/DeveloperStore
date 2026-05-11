using System.Text.Json;
using Ambev.DeveloperEvaluation.WebApi.Common.Errors;
using Ambev.DeveloperEvaluation.WebApi.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Middleware;

public class GlobalExceptionMiddlewareTests
{
    [Fact(DisplayName = "Should return unexpected error response when unhandled exception occurs")]
    public async Task Given_UnhandledException_When_InvokingMiddleware_Then_ShouldReturnUnexpectedError()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var logger = Substitute.For<ILogger<GlobalExceptionMiddleware>>();
        
        var middleware = new GlobalExceptionMiddleware(
            _ => throw new InvalidOperationException("Database unavailable."),
            logger);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        context.Response.Body.Position = 0;

        using var reader = new StreamReader(context.Response.Body);
        var body = await reader.ReadToEndAsync();
        var response = JsonSerializer.Deserialize<ErrorResponse>(
            body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        response.Should().NotBeNull();
        response!.Type.Should().Be("UnexpectedError");
        response.Error.Should().Be("Unexpected error");
        response.Detail.Should().Be("An unexpected error occurred while processing the request.");
    }
}

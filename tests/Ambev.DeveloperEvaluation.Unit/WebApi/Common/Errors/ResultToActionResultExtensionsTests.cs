using Ambev.DeveloperEvaluation.Common.Results;
using Ambev.DeveloperEvaluation.WebApi.Common.Errors;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Common.Errors;

public class ResultToActionResultExtensionsTests
{
    [Fact(DisplayName = "Should return success action result when result succeeds")]
    public void Given_SuccessResult_When_ConvertingToActionResult_Then_ShouldReturnSuccessAction()
    {
        var value = "created";
        var result = Result<string>.Success(value);

        var actionResult = result.ToActionResult(response => new OkObjectResult(response));

        var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(value);
    }

    [Theory(DisplayName = "Should map error type to expected status code")]
    [InlineData("ValidationError", StatusCodes.Status400BadRequest)]
    [InlineData("DomainRuleViolation", StatusCodes.Status400BadRequest)]
    [InlineData("ResourceNotFound", StatusCodes.Status404NotFound)]
    [InlineData("Conflict", StatusCodes.Status409Conflict)]
    [InlineData("UnexpectedError", StatusCodes.Status500InternalServerError)]
    public void Given_FailureResult_When_ConvertingToActionResult_Then_ShouldReturnErrorResponse(
        string errorType,
        int expectedStatusCode)
    {
        var errorMessage = "Error summary";
        var detail = "Error detail";
        var error = new Error(errorType, errorMessage, detail);
        var result = Result<string>.Failure(error);

        var actionResult = result.ToActionResult(response => new OkObjectResult(response));

        var objectResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
        var errorResponse = objectResult.Value.Should().BeOfType<ErrorResponse>().Subject;

        objectResult.StatusCode.Should().Be(expectedStatusCode);
        errorResponse.Type.Should().Be(errorType);
        errorResponse.Error.Should().Be(errorMessage);
        errorResponse.Detail.Should().Be(detail);
    }
}

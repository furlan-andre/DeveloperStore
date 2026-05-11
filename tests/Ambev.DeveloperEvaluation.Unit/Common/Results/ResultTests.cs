using Ambev.DeveloperEvaluation.Common.Results;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Common.Results;

public class ResultTests
{
    [Fact(DisplayName = "Should create success result with value")]
    public void Given_Value_When_CreatingSuccess_Then_ShouldContainValue()
    {
        var value = "success";

        var result = Result<string>.Success(value);

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be(value);
    }

    [Fact(DisplayName = "Should create failure result with error")]
    public void Given_Error_When_CreatingFailure_Then_ShouldContainError()
    {
        var error = Error.ResourceNotFound("Sale not found", "The sale does not exist.");

        var result = Result<string>.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact(DisplayName = "Should throw when reading value from failed result")]
    public void Given_Failure_When_ReadingValue_Then_ShouldThrow()
    {
        var error = Error.Validation("Invalid input data", "SaleNumber is required.");
        var result = Result<string>.Failure(error);

        var action = () => result.Value;

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact(DisplayName = "Should throw when reading error from successful result")]
    public void Given_Success_When_ReadingError_Then_ShouldThrow()
    {
        var result = Result<string>.Success("success");

        var action = () => result.Error;

        action.Should().Throw<InvalidOperationException>();
    }
}

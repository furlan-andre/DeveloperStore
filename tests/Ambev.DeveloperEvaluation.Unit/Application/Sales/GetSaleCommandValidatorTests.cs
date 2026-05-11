using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class GetSaleCommandValidatorTests
{
    private readonly GetSaleCommandValidator _validator = new();

    [Fact(DisplayName = "Should validate valid get sale command")]
    public void Given_ValidCommand_When_Validating_Then_ShouldBeValid()
    {
        var command = new GetSaleCommandTestBuilder().Build();

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Should reject get sale command when id is empty")]
    public void Given_EmptyId_When_Validating_Then_ShouldBeInvalid()
    {
        var emptyId = Guid.Empty;
        var command = new GetSaleCommandTestBuilder()
            .WithId(emptyId)
            .Build();

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(GetSaleCommand.Id));
    }
}

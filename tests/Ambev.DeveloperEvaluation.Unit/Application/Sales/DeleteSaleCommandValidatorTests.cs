using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class DeleteSaleCommandValidatorTests
{
    private readonly DeleteSaleCommandValidator _validator = new();

    [Fact(DisplayName = "Should validate valid delete sale command")]
    public void Given_ValidCommand_When_Validating_Then_ShouldBeValid()
    {
        var command = new DeleteSaleCommandTestBuilder().Build();

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Should reject delete sale command when id is empty")]
    public void Given_EmptyId_When_Validating_Then_ShouldBeInvalid()
    {
        var emptyId = Guid.Empty;
        var command = new DeleteSaleCommandTestBuilder()
            .WithId(emptyId)
            .Build();

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(DeleteSaleCommand.Id));
    }
}

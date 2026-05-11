using Ambev.DeveloperEvaluation.Application.Sales.UpdateSaleItem;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class UpdateSaleItemCommandValidatorTests
{
    private readonly UpdateSaleItemCommandValidator _validator = new();

    [Fact(DisplayName = "Should validate valid update sale item command")]
    public void Given_ValidCommand_When_Validating_Then_ShouldBeValid()
    {
        var command = new UpdateSaleItemCommandTestBuilder().Build();

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory(DisplayName = "Should reject update sale item command when required data is invalid")]
    [InlineData(nameof(UpdateSaleItemCommand.ProductId))]
    [InlineData(nameof(UpdateSaleItemCommand.ProductDescription))]
    [InlineData(nameof(UpdateSaleItemCommand.Quantity))]
    [InlineData(nameof(UpdateSaleItemCommand.UnitPrice))]
    public void Given_InvalidRequiredData_When_Validating_Then_ShouldBeInvalid(string propertyName)
    {
        var command = CreateInvalidCommand(propertyName);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == propertyName);
    }

    private static UpdateSaleItemCommand CreateInvalidCommand(string propertyName)
    {
        var emptyText = string.Empty;
        var emptyId = Guid.Empty;
        var invalidQuantity = 0;
        var invalidUnitPrice = 0m;

        return propertyName switch
        {
            nameof(UpdateSaleItemCommand.ProductId) => new UpdateSaleItemCommandTestBuilder()
                .WithProductId(emptyId)
                .Build(),
            
            nameof(UpdateSaleItemCommand.ProductDescription) => new UpdateSaleItemCommandTestBuilder()
                .WithProductDescription(emptyText)
                .Build(),
            
            nameof(UpdateSaleItemCommand.Quantity) => new UpdateSaleItemCommandTestBuilder()
                .WithQuantity(invalidQuantity)
                .Build(),
            
            nameof(UpdateSaleItemCommand.UnitPrice) => new UpdateSaleItemCommandTestBuilder()
                .WithUnitPrice(invalidUnitPrice)
                .Build(),
            
            _ => throw new ArgumentOutOfRangeException(nameof(propertyName), propertyName, null)
        };
    }
}

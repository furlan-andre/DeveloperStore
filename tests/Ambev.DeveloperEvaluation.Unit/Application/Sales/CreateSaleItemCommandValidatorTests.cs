using Ambev.DeveloperEvaluation.Application.Sales.CreateSaleItem;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleItemCommandValidatorTests
{
    private readonly CreateSaleItemCommandValidator _validator = new();

    [Fact(DisplayName = "Should validate valid create sale item command")]
    public void Given_ValidCommand_When_Validating_Then_ShouldBeValid()
    {
        var command = new CreateSaleItemCommandTestBuilder().Build();

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory(DisplayName = "Should reject create sale item command when required data is invalid")]
    [InlineData(nameof(CreateSaleItemCommand.ProductId))]
    [InlineData(nameof(CreateSaleItemCommand.ProductDescription))]
    [InlineData(nameof(CreateSaleItemCommand.Quantity))]
    [InlineData(nameof(CreateSaleItemCommand.UnitPrice))]
    public void Given_InvalidRequiredData_When_Validating_Then_ShouldBeInvalid(string propertyName)
    {
        var command = CreateInvalidCommand(propertyName);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == propertyName);
    }

    private static CreateSaleItemCommand CreateInvalidCommand(string propertyName)
    {
        var emptyText = string.Empty;
        var emptyId = Guid.Empty;
        var invalidQuantity = 0;
        var invalidUnitPrice = 0m;

        return propertyName switch
        {
            nameof(CreateSaleItemCommand.ProductId) => new CreateSaleItemCommandTestBuilder()
                .WithProductId(emptyId)
                .Build(),
            
            nameof(CreateSaleItemCommand.ProductDescription) => new CreateSaleItemCommandTestBuilder()
                .WithProductDescription(emptyText)
                .Build(),
            
            nameof(CreateSaleItemCommand.Quantity) => new CreateSaleItemCommandTestBuilder()
                .WithQuantity(invalidQuantity)
                .Build(),
            
            nameof(CreateSaleItemCommand.UnitPrice) => new CreateSaleItemCommandTestBuilder()
                .WithUnitPrice(invalidUnitPrice)
                .Build(),
            
            _ => throw new ArgumentOutOfRangeException(nameof(propertyName), propertyName, null)
        };
    }
}

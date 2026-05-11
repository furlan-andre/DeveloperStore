using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleCommandValidatorTests
{
    private readonly CreateSaleCommandValidator _validator = new();

    [Fact(DisplayName = "Should validate valid create sale command")]
    public void Given_ValidCommand_When_Validating_Then_ShouldBeValid()
    {
        var command = new CreateSaleCommandTestBuilder().Build();

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory(DisplayName = "Should reject create sale command when required sale data is invalid")]
    [InlineData(nameof(CreateSaleCommand.SaleNumber))]
    [InlineData(nameof(CreateSaleCommand.SaleDate))]
    [InlineData(nameof(CreateSaleCommand.CustomerId))]
    [InlineData(nameof(CreateSaleCommand.CustomerName))]
    [InlineData(nameof(CreateSaleCommand.BranchId))]
    [InlineData(nameof(CreateSaleCommand.BranchName))]
    [InlineData(nameof(CreateSaleCommand.Items))]
    public void Given_InvalidRequiredSaleData_When_Validating_Then_ShouldBeInvalid(string propertyName)
    {
        var command = CreateInvalidCommand(propertyName);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == propertyName);
    }

    [Fact(DisplayName = "Should reject create sale command when item is invalid")]
    public void Given_InvalidItem_When_Validating_Then_ShouldBeInvalid()
    {
        var quantity = 0;
        var item = new CreateSaleItemCommandTestBuilder()
            .WithQuantity(quantity)
            .Build();
        var command = new CreateSaleCommandTestBuilder()
            .WithItems([item])
            .Build();

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == "Items[0].Quantity");
    }

    private static CreateSaleCommand CreateInvalidCommand(string propertyName)
    {
        var emptyText = string.Empty;
        var emptyId = Guid.Empty;
        var defaultDate = default(DateTime);

        return propertyName switch
        {
            nameof(CreateSaleCommand.SaleNumber) => new CreateSaleCommandTestBuilder()
                .WithSaleNumber(emptyText)
                .Build(),
            
            nameof(CreateSaleCommand.SaleDate) => new CreateSaleCommandTestBuilder()
                .WithSaleDate(defaultDate)
                .Build(),
            
            nameof(CreateSaleCommand.CustomerId) => new CreateSaleCommandTestBuilder()
                .WithCustomerId(emptyId)
                .Build(),
            
            nameof(CreateSaleCommand.CustomerName) => new CreateSaleCommandTestBuilder()
                .WithCustomerName(emptyText)
                .Build(),
            
            nameof(CreateSaleCommand.BranchId) => new CreateSaleCommandTestBuilder()
                .WithBranchId(emptyId)
                .Build(),
            
            nameof(CreateSaleCommand.BranchName) => new CreateSaleCommandTestBuilder()
                .WithBranchName(emptyText)
                .Build(),
            
            nameof(CreateSaleCommand.Items) => new CreateSaleCommandTestBuilder()
                .WithItems([])
                .Build(),
            
            _ => throw new ArgumentOutOfRangeException(nameof(propertyName), propertyName, null)
        };
    }
}

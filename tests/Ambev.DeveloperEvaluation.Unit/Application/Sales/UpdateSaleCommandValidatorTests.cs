using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class UpdateSaleCommandValidatorTests
{
    private readonly UpdateSaleCommandValidator _validator = new();

    [Fact(DisplayName = "Should validate valid update sale command")]
    public void Given_ValidCommand_When_Validating_Then_ShouldBeValid()
    {
        var command = new UpdateSaleCommandTestBuilder().Build();

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory(DisplayName = "Should reject update sale command when required sale data is invalid")]
    [InlineData(nameof(UpdateSaleCommand.Id))]
    [InlineData(nameof(UpdateSaleCommand.SaleNumber))]
    [InlineData(nameof(UpdateSaleCommand.SaleDate))]
    [InlineData(nameof(UpdateSaleCommand.CustomerId))]
    [InlineData(nameof(UpdateSaleCommand.CustomerName))]
    [InlineData(nameof(UpdateSaleCommand.BranchId))]
    [InlineData(nameof(UpdateSaleCommand.BranchName))]
    [InlineData(nameof(UpdateSaleCommand.Items))]
    public void Given_InvalidRequiredSaleData_When_Validating_Then_ShouldBeInvalid(string propertyName)
    {
        var command = CreateInvalidCommand(propertyName);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == propertyName);
    }

    [Fact(DisplayName = "Should reject update sale command when item is invalid")]
    public void Given_InvalidItem_When_Validating_Then_ShouldBeInvalid()
    {
        var unitPrice = 0m;
        var item = new UpdateSaleItemCommandTestBuilder()
            .WithUnitPrice(unitPrice)
            .Build();
        var command = new UpdateSaleCommandTestBuilder()
            .WithItems([item])
            .Build();

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == "Items[0].UnitPrice");
    }

    private static UpdateSaleCommand CreateInvalidCommand(string propertyName)
    {
        var emptyText = string.Empty;
        var emptyId = Guid.Empty;
        var defaultDate = default(DateTime);

        return propertyName switch
        {
            nameof(UpdateSaleCommand.Id) => new UpdateSaleCommandTestBuilder()
                .WithId(emptyId)
                .Build(),
            
            nameof(UpdateSaleCommand.SaleNumber) => new UpdateSaleCommandTestBuilder()
                .WithSaleNumber(emptyText)
                .Build(),
            
            nameof(UpdateSaleCommand.SaleDate) => new UpdateSaleCommandTestBuilder()
                .WithSaleDate(defaultDate)
                .Build(),
            
            nameof(UpdateSaleCommand.CustomerId) => new UpdateSaleCommandTestBuilder()
                .WithCustomerId(emptyId)
                .Build(),
            
            nameof(UpdateSaleCommand.CustomerName) => new UpdateSaleCommandTestBuilder()
                .WithCustomerName(emptyText)
                .Build(),
            
            nameof(UpdateSaleCommand.BranchId) => new UpdateSaleCommandTestBuilder()
                .WithBranchId(emptyId)
                .Build(),
            
            nameof(UpdateSaleCommand.BranchName) => new UpdateSaleCommandTestBuilder()
                .WithBranchName(emptyText)
                .Build(),
            
            nameof(UpdateSaleCommand.Items) => new UpdateSaleCommandTestBuilder()
                .WithItems([])
                .Build(),
            
            _ => throw new ArgumentOutOfRangeException(nameof(propertyName), propertyName, null)
        };
    }
}

using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class ListSalesCommandValidatorTests
{
    private readonly ListSalesCommandValidator _validator = new();

    [Fact(DisplayName = "Should validate valid list sales command")]
    public void Given_ValidCommand_When_Validating_Then_ShouldBeValid()
    {
        var order = "saleDate desc,totalSaleAmount asc";
        IReadOnlyDictionary<string, string?> filters = new Dictionary<string, string?>
        {
            ["active"] = "true",
            ["saleNumber"] = "SALE"
        };
        var command = new ListSalesCommandTestBuilder()
            .WithOrder(order)
            .WithFilters(filters)
            .Build();

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory(DisplayName = "Should reject list sales command when pagination is invalid")]
    [InlineData(0, 10, nameof(ListSalesCommand.Page))]
    [InlineData(1, 0, nameof(ListSalesCommand.Size))]
    [InlineData(1, 101, nameof(ListSalesCommand.Size))]
    public void Given_InvalidPagination_When_Validating_Then_ShouldBeInvalid(
        int page,
        int size,
        string propertyName)
    {
        var command = new ListSalesCommandTestBuilder()
            .WithPage(page)
            .WithSize(size)
            .Build();

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == propertyName);
    }

    [Fact(DisplayName = "Should reject list sales command when filter field is unsupported")]
    public void Given_UnsupportedFilter_When_Validating_Then_ShouldBeInvalid()
    {
        var unsupportedFilter = "unsupported";
        IReadOnlyDictionary<string, string?> filters = new Dictionary<string, string?>
        {
            [unsupportedFilter] = "value"
        };
        var command = new ListSalesCommandTestBuilder()
            .WithFilters(filters)
            .Build();

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName.StartsWith("Filters"));
    }

    [Fact(DisplayName = "Should reject list sales command when order field is unsupported")]
    public void Given_UnsupportedOrder_When_Validating_Then_ShouldBeInvalid()
    {
        var order = "unsupported desc";
        var command = new ListSalesCommandTestBuilder()
            .WithOrder(order)
            .Build();

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(ListSalesCommand.Order));
    }
}

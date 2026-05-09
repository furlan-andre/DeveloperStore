using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.TestUtils.Domain.Sales.Builders;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Sales;

public class CustomerTests
{
    [Fact(DisplayName = "Should create valid customer")]
    public void Given_ValidData_When_CreatingCustomer_Then_ShouldCreateCustomer()
    {
        var customer = ReferenceDataTestBuilder.CreateCustomer();

        Assert.NotNull(customer);
    }

    [Fact(DisplayName = "Should keep informed customer id")]
    public void Given_ValidId_When_CreatingCustomer_Then_ShouldKeepId()
    {
        var id = Guid.NewGuid();
        var name = "John Doe";

        var customer = new Customer(id, name);

        Assert.Equal(id, customer.Id);
    }

    [Fact(DisplayName = "Should keep informed customer name")]
    public void Given_ValidName_When_CreatingCustomer_Then_ShouldKeepName()
    {
        var name = "John Doe";
        var id = Guid.NewGuid();

        var customer = new Customer(id, name);

        Assert.Equal(name, customer.Name);
    }

    [Fact(DisplayName = "Should throw DomainException when customer id is empty")]
    public void Given_EmptyId_When_CreatingCustomer_Then_ShouldThrowDomainException()
    {
        var id = Guid.Empty;
        var name = "John Doe";

        var action = () => new Customer(id, name);

        Assert.Throws<DomainException>(action);
    }

    [Theory(DisplayName = "Should throw DomainException when customer name is invalid")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Given_InvalidName_When_CreatingCustomer_Then_ShouldThrowDomainException(string? name)
    {
        var id = Guid.NewGuid();

        var action = () => new Customer(id, name);

        Assert.Throws<DomainException>(action);
    }
}

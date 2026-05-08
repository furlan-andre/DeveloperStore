using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Sales;

public class CustomerTests
{
    [Fact(DisplayName = "Should create valid customer")]
    public void Given_ValidData_When_CreatingCustomer_Then_ShouldCreateCustomer()
    {
        var id = Guid.NewGuid();
        var name = "John Doe";

        var customer = new Customer(id, name);

        Assert.NotNull(customer);
    }

    [Fact(DisplayName = "Should keep informed customer id")]
    public void Given_ValidId_When_CreatingCustomer_Then_ShouldKeepId()
    {
        var id = Guid.NewGuid();

        var customer = new Customer(id, "John Doe");

        Assert.Equal(id, customer.Id);
    }

    [Fact(DisplayName = "Should keep informed customer name")]
    public void Given_ValidName_When_CreatingCustomer_Then_ShouldKeepName()
    {
        var name = "John Doe";

        var customer = new Customer(Guid.NewGuid(), name);

        Assert.Equal(name, customer.Name);
    }

    [Fact(DisplayName = "Should throw DomainException when customer id is empty")]
    public void Given_EmptyId_When_CreatingCustomer_Then_ShouldThrowDomainException()
    {
        var action = () => new Customer(Guid.Empty, "John Doe");

        Assert.Throws<DomainException>(action);
    }

    [Theory(DisplayName = "Should throw DomainException when customer name is invalid")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Given_InvalidName_When_CreatingCustomer_Then_ShouldThrowDomainException(string? name)
    {
        var action = () => new Customer(Guid.NewGuid(), name);

        Assert.Throws<DomainException>(action);
    }
}

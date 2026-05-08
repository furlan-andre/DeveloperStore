using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Sales;

public class ProductTests
{
    [Fact(DisplayName = "Should create valid product")]
    public void Given_ValidData_When_CreatingProduct_Then_ShouldCreateProduct()
    {
        var id = Guid.NewGuid();
        var description = "Product description";

        var product = new Product(id, description);

        Assert.NotNull(product);
    }

    [Fact(DisplayName = "Should keep informed product id")]
    public void Given_ValidId_When_CreatingProduct_Then_ShouldKeepId()
    {
        var id = Guid.NewGuid();

        var product = new Product(id, "Product description");

        Assert.Equal(id, product.Id);
    }

    [Fact(DisplayName = "Should keep informed product description")]
    public void Given_ValidDescription_When_CreatingProduct_Then_ShouldKeepDescription()
    {
        var description = "Product description";

        var product = new Product(Guid.NewGuid(), description);

        Assert.Equal(description, product.Description);
    }

    [Fact(DisplayName = "Should throw DomainException when product id is empty")]
    public void Given_EmptyId_When_CreatingProduct_Then_ShouldThrowDomainException()
    {
        var action = () => new Product(Guid.Empty, "Product description");

        Assert.Throws<DomainException>(action);
    }

    [Theory(DisplayName = "Should throw DomainException when product description is invalid")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Given_InvalidDescription_When_CreatingProduct_Then_ShouldThrowDomainException(string? description)
    {
        var action = () => new Product(Guid.NewGuid(), description);

        Assert.Throws<DomainException>(action);
    }
}

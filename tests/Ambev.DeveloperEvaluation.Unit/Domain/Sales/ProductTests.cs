using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.TestUtils.Domain.Sales.Builders;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Sales;

public class ProductTests
{
    [Fact(DisplayName = "Should create valid product")]
    public void Given_ValidData_When_CreatingProduct_Then_ShouldCreateProduct()
    {
        var product = ReferenceDataTestBuilder.CreateProduct();

        Assert.NotNull(product);
    }

    [Fact(DisplayName = "Should keep informed product id")]
    public void Given_ValidId_When_CreatingProduct_Then_ShouldKeepId()
    {
        var id = Guid.NewGuid();
        var description = "Product description";

        var product = new Product(id, description);

        Assert.Equal(id, product.Id);
    }

    [Fact(DisplayName = "Should keep informed product description")]
    public void Given_ValidDescription_When_CreatingProduct_Then_ShouldKeepDescription()
    {
        var description = "Product description";
        var id = Guid.NewGuid();

        var product = new Product(id, description);

        Assert.Equal(description, product.Description);
    }

    [Fact(DisplayName = "Should throw DomainException when product id is empty")]
    public void Given_EmptyId_When_CreatingProduct_Then_ShouldThrowDomainException()
    {
        var id = Guid.Empty;
        var description = "Product description";

        var action = () => new Product(id, description);

        Assert.Throws<DomainException>(action);
    }

    [Theory(DisplayName = "Should throw DomainException when product description is invalid")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Given_InvalidDescription_When_CreatingProduct_Then_ShouldThrowDomainException(string? description)
    {
        var id = Guid.NewGuid();

        var action = () => new Product(id, description);

        Assert.Throws<DomainException>(action);
    }
}

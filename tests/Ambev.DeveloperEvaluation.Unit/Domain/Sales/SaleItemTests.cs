using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Services.Sales;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Sales;

public class SaleItemTests
{
    private static readonly Product Product = new(Guid.NewGuid(), "Product description");

    [Fact(DisplayName = "Should create valid sale item")]
    public void Given_ValidData_When_CreatingSaleItem_Then_ShouldCreateSaleItem()
    {
        var item = new SaleItem(Product, 1, 100m, new SaleDiscountPolicy());

        Assert.NotNull(item);
    }

    [Fact(DisplayName = "Should generate sale item id")]
    public void Given_ValidData_When_CreatingSaleItem_Then_ShouldGenerateId()
    {
        var item = new SaleItem(Product, 1, 100m, new SaleDiscountPolicy());

        Assert.NotEqual(Guid.Empty, item.Id);
    }

    [Fact(DisplayName = "Should keep product data")]
    public void Given_Product_When_CreatingSaleItem_Then_ShouldKeepProductData()
    {
        var item = new SaleItem(Product, 1, 100m, new SaleDiscountPolicy());

        Assert.Equal(Product.Id, item.Product.Id);
        Assert.Equal(Product.Description, item.Product.Description);
    }

    [Fact(DisplayName = "Should keep quantity")]
    public void Given_Quantity_When_CreatingSaleItem_Then_ShouldKeepQuantity()
    {
        var item = new SaleItem(Product, 3, 100m, new SaleDiscountPolicy());

        Assert.Equal(3, item.Quantity);
    }

    [Fact(DisplayName = "Should keep unit price")]
    public void Given_UnitPrice_When_CreatingSaleItem_Then_ShouldKeepUnitPrice()
    {
        var item = new SaleItem(Product, 1, 123.45m, new SaleDiscountPolicy());

        Assert.Equal(123.45m, item.UnitPrice);
    }

    [Fact(DisplayName = "Should calculate zero discount for quantity from 1 to 3")]
    public void Given_QuantityFromOneToThree_When_CreatingSaleItem_Then_ShouldCalculateZeroDiscount()
    {
        var item = new SaleItem(Product, 3, 100m, new SaleDiscountPolicy());

        Assert.Equal(0m, item.Discount);
    }

    [Fact(DisplayName = "Should calculate ten percent discount for quantity from 4 to 9")]
    public void Given_QuantityFromFourToNine_When_CreatingSaleItem_Then_ShouldCalculateTenPercentDiscount()
    {
        var item = new SaleItem(Product, 4, 100m, new SaleDiscountPolicy());

        Assert.Equal(40m, item.Discount);
    }

    [Fact(DisplayName = "Should calculate twenty percent discount for quantity from 10 to 20")]
    public void Given_QuantityFromTenToTwenty_When_CreatingSaleItem_Then_ShouldCalculateTwentyPercentDiscount()
    {
        var item = new SaleItem(Product, 10, 100m, new SaleDiscountPolicy());

        Assert.Equal(200m, item.Discount);
    }

    [Fact(DisplayName = "Should calculate total amount")]
    public void Given_Discount_When_CreatingSaleItem_Then_ShouldCalculateTotalAmount()
    {
        var item = new SaleItem(Product, 4, 100m, new SaleDiscountPolicy());

        Assert.Equal(360m, item.TotalAmount);
    }

    [Fact(DisplayName = "Should calculate total amount using controlled discount policy")]
    public void Given_ControlledDiscountPolicy_When_CreatingSaleItem_Then_ShouldCalculateTotalAmount()
    {
        var discountPolicy = Substitute.For<ISaleDiscountPolicy>();
        discountPolicy.CalculateDiscount(2, 100m).Returns(15m);

        var item = new SaleItem(Product, 2, 100m, discountPolicy);

        Assert.Equal(15m, item.Discount);
        Assert.Equal(185m, item.TotalAmount);
    }

    [Fact(DisplayName = "Should allow quantity equal to 20")]
    public void Given_QuantityEqualToTwenty_When_CreatingSaleItem_Then_ShouldCreateSaleItem()
    {
        var item = new SaleItem(Product, 20, 100m, new SaleDiscountPolicy());

        Assert.Equal(20, item.Quantity);
    }

    [Fact(DisplayName = "Should throw DomainException when product is null")]
    public void Given_NullProduct_When_CreatingSaleItem_Then_ShouldThrowDomainException()
    {
        var action = () => new SaleItem(null, 1, 100m, new SaleDiscountPolicy());

        Assert.Throws<DomainException>(action);
    }

    [Fact(DisplayName = "Should throw DomainException when discount policy is null")]
    public void Given_NullDiscountPolicy_When_CreatingSaleItem_Then_ShouldThrowDomainException()
    {
        var action = () => new SaleItem(Product, 1, 100m, null);

        Assert.Throws<DomainException>(action);
    }

    [Theory(DisplayName = "Should throw DomainException when quantity is invalid")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(21)]
    public void Given_InvalidQuantity_When_CreatingSaleItem_Then_ShouldThrowDomainException(int quantity)
    {
        var action = () => new SaleItem(Product, quantity, 100m, new SaleDiscountPolicy());

        Assert.Throws<DomainException>(action);
    }

    [Theory(DisplayName = "Should throw DomainException when unit price is invalid")]
    [InlineData(0)]
    [InlineData(-1)]
    public void Given_InvalidUnitPrice_When_CreatingSaleItem_Then_ShouldThrowDomainException(decimal unitPrice)
    {
        var action = () => new SaleItem(Product, 1, unitPrice, new SaleDiscountPolicy());

        Assert.Throws<DomainException>(action);
    }

    [Fact(DisplayName = "Should throw DomainException when discount is negative")]
    public void Given_NegativeDiscount_When_CreatingSaleItem_Then_ShouldThrowDomainException()
    {
        var discountPolicy = Substitute.For<ISaleDiscountPolicy>();
        discountPolicy.CalculateDiscount(1, 100m).Returns(-1m);

        var action = () => new SaleItem(Product, 1, 100m, discountPolicy);

        Assert.Throws<DomainException>(action);
    }

    [Fact(DisplayName = "Should throw DomainException when discount is greater than subtotal")]
    public void Given_DiscountGreaterThanSubtotal_When_CreatingSaleItem_Then_ShouldThrowDomainException()
    {
        var discountPolicy = Substitute.For<ISaleDiscountPolicy>();
        discountPolicy.CalculateDiscount(1, 100m).Returns(101m);

        var action = () => new SaleItem(Product, 1, 100m, discountPolicy);

        Assert.Throws<DomainException>(action);
    }
}

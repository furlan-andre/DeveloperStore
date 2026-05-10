using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Services.Sales;
using Ambev.DeveloperEvaluation.TestUtils.Domain.Sales.Builders;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Sales;

public class SaleItemTests
{
    [Fact(DisplayName = "Should create valid sale item")]
    public void Given_ValidData_When_CreatingSaleItem_Then_ShouldCreateSaleItem()
    {
        var item = new SaleItemTestBuilder().Build();

        Assert.NotNull(item);
    }

    [Fact(DisplayName = "Should generate sale item id")]
    public void Given_ValidData_When_CreatingSaleItem_Then_ShouldGenerateId()
    {
        var item = new SaleItemTestBuilder().Build();

        Assert.NotEqual(Guid.Empty, item.Id);
    }

    [Fact(DisplayName = "Should start active")]
    public void Given_ValidData_When_CreatingSaleItem_Then_ShouldStartActive()
    {
        var item = new SaleItemTestBuilder().Build();

        Assert.True(item.Active);
    }

    [Fact(DisplayName = "Should update active flag")]
    public void Given_ActiveFlag_When_SettingSaleItemActive_Then_ShouldUpdateActiveFlag()
    {
        var item = new SaleItemTestBuilder().Build();

        item.SetActive(false);

        Assert.False(item.Active);
    }

    [Fact(DisplayName = "Should keep product data")]
    public void Given_Product_When_CreatingSaleItem_Then_ShouldKeepProductData()
    {
        var product = ReferenceDataTestBuilder.CreateProduct();

        var item = new SaleItemTestBuilder()
            .WithProduct(product)
            .Build();

        Assert.Equal(product.Id, item.Product.Id);
        Assert.Equal(product.Description, item.Product.Description);
    }

    [Fact(DisplayName = "Should keep quantity")]
    public void Given_Quantity_When_CreatingSaleItem_Then_ShouldKeepQuantity()
    {
        var quantity = 3;

        var item = new SaleItemTestBuilder()
            .WithQuantity(quantity)
            .Build();

        Assert.Equal(quantity, item.Quantity);
    }

    [Fact(DisplayName = "Should keep unit price")]
    public void Given_UnitPrice_When_CreatingSaleItem_Then_ShouldKeepUnitPrice()
    {
        var unitPrice = 123.45m;

        var item = new SaleItemTestBuilder()
            .WithUnitPrice(unitPrice)
            .Build();

        Assert.Equal(unitPrice, item.UnitPrice);
    }

    [Fact(DisplayName = "Should calculate zero discount for quantity from 1 to 3")]
    public void Given_QuantityFromOneToThree_When_CreatingSaleItem_Then_ShouldCalculateZeroDiscount()
    {
        var quantity = 3;
        var unitPrice = 100m;
        var expectedDiscount = 0m;

        var item = new SaleItemTestBuilder()
            .WithQuantity(quantity)
            .WithUnitPrice(unitPrice)
            .Build();

        Assert.Equal(expectedDiscount, item.Discount);
    }

    [Fact(DisplayName = "Should calculate ten percent discount for quantity from 4 to 9")]
    public void Given_QuantityFromFourToNine_When_CreatingSaleItem_Then_ShouldCalculateTenPercentDiscount()
    {
        var quantity = 4;
        var unitPrice = 100m;
        var expectedDiscount = 40m;

        var item = new SaleItemTestBuilder()
            .WithQuantity(quantity)
            .WithUnitPrice(unitPrice)
            .Build();

        Assert.Equal(expectedDiscount, item.Discount);
    }

    [Fact(DisplayName = "Should calculate twenty percent discount for quantity from 10 to 20")]
    public void Given_QuantityFromTenToTwenty_When_CreatingSaleItem_Then_ShouldCalculateTwentyPercentDiscount()
    {
        var quantity = 10;
        var unitPrice = 100m;
        var expectedDiscount = 200m;

        var item = new SaleItemTestBuilder()
            .WithQuantity(quantity)
            .WithUnitPrice(unitPrice)
            .Build();

        Assert.Equal(expectedDiscount, item.Discount);
    }

    [Fact(DisplayName = "Should calculate total amount")]
    public void Given_Discount_When_CreatingSaleItem_Then_ShouldCalculateTotalAmount()
    {
        var quantity = 4;
        var unitPrice = 100m;
        var expectedTotalAmount = 360m;

        var item = new SaleItemTestBuilder()
            .WithQuantity(quantity)
            .WithUnitPrice(unitPrice)
            .Build();

        Assert.Equal(expectedTotalAmount, item.TotalAmount);
    }

    [Fact(DisplayName = "Should calculate total amount using controlled discount policy")]
    public void Given_ControlledDiscountPolicy_When_CreatingSaleItem_Then_ShouldCalculateTotalAmount()
    {
        var discountPolicy = Substitute.For<ISaleDiscountPolicy>();
        var quantity = 2;
        var unitPrice = 100m;
        var discount = 15m;
        var expectedTotalAmount = 185m;
        discountPolicy.CalculateDiscount(quantity, unitPrice).Returns(discount);

        var item = new SaleItemTestBuilder()
            .WithQuantity(quantity)
            .WithUnitPrice(unitPrice)
            .WithDiscountPolicy(discountPolicy)
            .Build();

        Assert.Equal(discount, item.Discount);
        Assert.Equal(expectedTotalAmount, item.TotalAmount);
    }

    [Fact(DisplayName = "Should allow quantity equal to 20")]
    public void Given_QuantityEqualToTwenty_When_CreatingSaleItem_Then_ShouldCreateSaleItem()
    {
        var quantity = 20;
        var unitPrice = 100m;

        var item = new SaleItemTestBuilder()
            .WithQuantity(quantity)
            .WithUnitPrice(unitPrice)
            .Build();

        Assert.Equal(quantity, item.Quantity);
    }

    [Fact(DisplayName = "Should throw DomainException when product is null")]
    public void Given_NullProduct_When_CreatingSaleItem_Then_ShouldThrowDomainException()
    {
        Product? product = null;

        var action = () => new SaleItemTestBuilder()
            .WithProduct(product)
            .Build();

        Assert.Throws<DomainException>(action);
    }

    [Fact(DisplayName = "Should throw DomainException when discount policy is null")]
    public void Given_NullDiscountPolicy_When_CreatingSaleItem_Then_ShouldThrowDomainException()
    {
        ISaleDiscountPolicy? discountPolicy = null;

        var action = () => new SaleItemTestBuilder()
            .WithDiscountPolicy(discountPolicy)
            .Build();

        Assert.Throws<DomainException>(action);
    }

    [Theory(DisplayName = "Should throw DomainException when quantity is invalid")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(21)]
    public void Given_InvalidQuantity_When_CreatingSaleItem_Then_ShouldThrowDomainException(int quantity)
    {
        var unitPrice = 100m;

        var action = () => new SaleItemTestBuilder()
            .WithQuantity(quantity)
            .WithUnitPrice(unitPrice)
            .Build();

        Assert.Throws<DomainException>(action);
    }

    [Theory(DisplayName = "Should throw DomainException when unit price is invalid")]
    [InlineData(0)]
    [InlineData(-1)]
    public void Given_InvalidUnitPrice_When_CreatingSaleItem_Then_ShouldThrowDomainException(decimal unitPrice)
    {
        var quantity = 1;

        var action = () => new SaleItemTestBuilder()
            .WithQuantity(quantity)
            .WithUnitPrice(unitPrice)
            .Build();

        Assert.Throws<DomainException>(action);
    }

    [Fact(DisplayName = "Should throw DomainException when discount is negative")]
    public void Given_NegativeDiscount_When_CreatingSaleItem_Then_ShouldThrowDomainException()
    {
        var discountPolicy = Substitute.For<ISaleDiscountPolicy>();
        var quantity = 1;
        var unitPrice = 100m;
        var discount = -1m;
        discountPolicy.CalculateDiscount(quantity, unitPrice).Returns(discount);

        var action = () => new SaleItemTestBuilder()
            .WithQuantity(quantity)
            .WithUnitPrice(unitPrice)
            .WithDiscountPolicy(discountPolicy)
            .Build();

        Assert.Throws<DomainException>(action);
    }

    [Fact(DisplayName = "Should throw DomainException when discount is greater than subtotal")]
    public void Given_DiscountGreaterThanSubtotal_When_CreatingSaleItem_Then_ShouldThrowDomainException()
    {
        var discountPolicy = Substitute.For<ISaleDiscountPolicy>();
        var quantity = 1;
        var unitPrice = 100m;
        var discount = 101m;
        discountPolicy.CalculateDiscount(quantity, unitPrice).Returns(discount);

        var action = () => new SaleItemTestBuilder()
            .WithQuantity(quantity)
            .WithUnitPrice(unitPrice)
            .WithDiscountPolicy(discountPolicy)
            .Build();

        Assert.Throws<DomainException>(action);
    }
}

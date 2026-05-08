using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Services.Sales;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Sales;

public class SaleDiscountPolicyTests
{
    private readonly SaleDiscountPolicy _policy = new();

    [Theory(DisplayName = "Should return zero discount for quantities from 1 to 3")]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Given_QuantityFromOneToThree_When_CalculatingDiscount_Then_ShouldReturnZero(int quantity)
    {
        var discount = _policy.CalculateDiscount(quantity, 100m);

        Assert.Equal(0m, discount);
    }

    [Theory(DisplayName = "Should return ten percent discount for quantities from 4 to 9")]
    [InlineData(4, 100, 40)]
    [InlineData(9, 100, 90)]
    public void Given_QuantityFromFourToNine_When_CalculatingDiscount_Then_ShouldReturnTenPercent(
        int quantity,
        decimal unitPrice,
        decimal expectedDiscount)
    {
        var discount = _policy.CalculateDiscount(quantity, unitPrice);

        Assert.Equal(expectedDiscount, discount);
    }

    [Theory(DisplayName = "Should return twenty percent discount for quantities from 10 to 20")]
    [InlineData(10, 100, 200)]
    [InlineData(20, 100, 400)]
    public void Given_QuantityFromTenToTwenty_When_CalculatingDiscount_Then_ShouldReturnTwentyPercent(
        int quantity,
        decimal unitPrice,
        decimal expectedDiscount)
    {
        var discount = _policy.CalculateDiscount(quantity, unitPrice);

        Assert.Equal(expectedDiscount, discount);
    }

    [Theory(DisplayName = "Should throw DomainException when quantity is invalid")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(21)]
    public void Given_InvalidQuantity_When_CalculatingDiscount_Then_ShouldThrowDomainException(int quantity)
    {
        var action = () => { _policy.CalculateDiscount(quantity, 100m); };

        Assert.Throws<DomainException>(action);
    }

    [Theory(DisplayName = "Should throw DomainException when unit price is invalid")]
    [InlineData(0)]
    [InlineData(-1)]
    public void Given_InvalidUnitPrice_When_CalculatingDiscount_Then_ShouldThrowDomainException(decimal unitPrice)
    {
        var action = () => { _policy.CalculateDiscount(1, unitPrice); };

        Assert.Throws<DomainException>(action);
    }
}

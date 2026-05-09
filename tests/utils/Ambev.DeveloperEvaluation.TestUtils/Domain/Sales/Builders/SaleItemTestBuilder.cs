using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Services.Sales;
using Bogus;

namespace Ambev.DeveloperEvaluation.TestUtils.Domain.Sales.Builders;

public class SaleItemTestBuilder
{
    private static readonly Faker Faker = new();

    private Product? _product = ReferenceDataTestBuilder.CreateProduct();
    private int _quantity = Faker.Random.Int(1, 3);
    private decimal _unitPrice = Faker.Finance.Amount(1m, 1000m);
    private ISaleDiscountPolicy? _discountPolicy = new SaleDiscountPolicy();

    public SaleItemTestBuilder WithProduct(Product? product)
    {
        _product = product;
        return this;
    }

    public SaleItemTestBuilder WithQuantity(int quantity)
    {
        _quantity = quantity;
        return this;
    }

    public SaleItemTestBuilder WithUnitPrice(decimal unitPrice)
    {
        _unitPrice = unitPrice;
        return this;
    }

    public SaleItemTestBuilder WithDiscountPolicy(ISaleDiscountPolicy? discountPolicy)
    {
        _discountPolicy = discountPolicy;
        return this;
    }

    public SaleItem Build()
    {
        return new SaleItem(_product, _quantity, _unitPrice, _discountPolicy);
    }
}

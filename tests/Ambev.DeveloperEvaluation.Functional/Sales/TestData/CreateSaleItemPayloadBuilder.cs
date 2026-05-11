using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSaleItem;
using Bogus;

namespace Ambev.DeveloperEvaluation.Functional.Sales.TestData;

public sealed class CreateSaleItemPayloadBuilder
{
    private static readonly Faker Faker = new();

    private Guid _productId = Guid.NewGuid();
    private string _productDescription = Faker.Commerce.ProductName();
    private int _quantity = 4;
    private decimal _unitPrice = 100m;

    public CreateSaleItemPayloadBuilder WithProductId(Guid productId)
    {
        _productId = productId;
        return this;
    }

    public CreateSaleItemPayloadBuilder WithProductDescription(string productDescription)
    {
        _productDescription = productDescription;
        return this;
    }

    public CreateSaleItemPayloadBuilder WithQuantity(int quantity)
    {
        _quantity = quantity;
        return this;
    }

    public CreateSaleItemPayloadBuilder WithUnitPrice(decimal unitPrice)
    {
        _unitPrice = unitPrice;
        return this;
    }

    public CreateSaleItemInput Build()
    {
        return new CreateSaleItemInput
        {
            ProductId = _productId,
            ProductDescription = _productDescription,
            Quantity = _quantity,
            UnitPrice = _unitPrice
        };
    }
}

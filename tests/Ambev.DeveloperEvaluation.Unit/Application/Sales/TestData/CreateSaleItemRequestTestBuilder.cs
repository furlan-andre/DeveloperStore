using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

public class CreateSaleItemRequestTestBuilder
{
    private static readonly Faker Faker = new();

    private Guid _productId = Guid.NewGuid();
    private string _productDescription = Faker.Commerce.ProductName();
    private int _quantity = Faker.Random.Int(1, 3);
    private decimal _unitPrice = Faker.Finance.Amount(1m, 1000m);

    public CreateSaleItemRequestTestBuilder WithProductId(Guid productId)
    {
        _productId = productId;
        return this;
    }

    public CreateSaleItemRequestTestBuilder WithProductDescription(string productDescription)
    {
        _productDescription = productDescription;
        return this;
    }

    public CreateSaleItemRequestTestBuilder WithQuantity(int quantity)
    {
        _quantity = quantity;
        return this;
    }

    public CreateSaleItemRequestTestBuilder WithUnitPrice(decimal unitPrice)
    {
        _unitPrice = unitPrice;
        return this;
    }

    public CreateSaleItemRequest Build()
    {
        return new CreateSaleItemRequest
        {
            ProductId = _productId,
            ProductDescription = _productDescription,
            Quantity = _quantity,
            UnitPrice = _unitPrice
        };
    }
}

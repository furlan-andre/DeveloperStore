using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSaleItem;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Sales.CreateSales;

public class CreateSaleItemInputTestBuilder
{
    private static readonly Faker Faker = new();

    private Guid _productId = Guid.NewGuid();
    private string _productDescription = Faker.Commerce.ProductName();
    private int _quantity = Faker.Random.Int(1, 3);
    private decimal _unitPrice = Faker.Finance.Amount(1m, 1000m);

    public CreateSaleItemInputTestBuilder WithProductId(Guid productId)
    {
        _productId = productId;
        return this;
    }

    public CreateSaleItemInputTestBuilder WithProductDescription(string productDescription)
    {
        _productDescription = productDescription;
        return this;
    }

    public CreateSaleItemInputTestBuilder WithQuantity(int quantity)
    {
        _quantity = quantity;
        return this;
    }

    public CreateSaleItemInputTestBuilder WithUnitPrice(decimal unitPrice)
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

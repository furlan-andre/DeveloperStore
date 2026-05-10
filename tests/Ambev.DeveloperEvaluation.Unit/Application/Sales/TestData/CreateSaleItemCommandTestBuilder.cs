using Ambev.DeveloperEvaluation.Application.Sales.CreateSaleItem;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

public class CreateSaleItemCommandTestBuilder
{
    private static readonly Faker Faker = new();

    private Guid _productId = Guid.NewGuid();
    private string _productDescription = Faker.Commerce.ProductName();
    private int _quantity = Faker.Random.Int(1, 3);
    private decimal _unitPrice = Faker.Finance.Amount(1m, 1000m);

    public CreateSaleItemCommandTestBuilder WithProductId(Guid productId)
    {
        _productId = productId;
        return this;
    }

    public CreateSaleItemCommandTestBuilder WithProductDescription(string productDescription)
    {
        _productDescription = productDescription;
        return this;
    }

    public CreateSaleItemCommandTestBuilder WithQuantity(int quantity)
    {
        _quantity = quantity;
        return this;
    }

    public CreateSaleItemCommandTestBuilder WithUnitPrice(decimal unitPrice)
    {
        _unitPrice = unitPrice;
        return this;
    }

    public CreateSaleItemCommand Build()
    {
        return new CreateSaleItemCommand
        {
            ProductId = _productId,
            ProductDescription = _productDescription,
            Quantity = _quantity,
            UnitPrice = _unitPrice
        };
    }
}

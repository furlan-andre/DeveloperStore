using Ambev.DeveloperEvaluation.Application.Sales.UpdateSaleItem;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

public class UpdateSaleItemRequestTestBuilder
{
    private static readonly Faker Faker = new();

    private Guid? _id;
    private Guid _productId = Guid.NewGuid();
    private string _productDescription = Faker.Commerce.ProductName();
    private int _quantity = Faker.Random.Int(1, 3);
    private decimal _unitPrice = Faker.Finance.Amount(1m, 1000m);
    private bool _active = true;

    public UpdateSaleItemRequestTestBuilder WithId(Guid? id)
    {
        _id = id;
        return this;
    }

    public UpdateSaleItemRequestTestBuilder WithProductId(Guid productId)
    {
        _productId = productId;
        return this;
    }

    public UpdateSaleItemRequestTestBuilder WithProductDescription(string productDescription)
    {
        _productDescription = productDescription;
        return this;
    }

    public UpdateSaleItemRequestTestBuilder WithQuantity(int quantity)
    {
        _quantity = quantity;
        return this;
    }

    public UpdateSaleItemRequestTestBuilder WithUnitPrice(decimal unitPrice)
    {
        _unitPrice = unitPrice;
        return this;
    }

    public UpdateSaleItemRequestTestBuilder WithActive(bool active)
    {
        _active = active;
        return this;
    }

    public UpdateSaleItemRequest Build()
    {
        return new UpdateSaleItemRequest
        {
            Id = _id,
            ProductId = _productId,
            ProductDescription = _productDescription,
            Quantity = _quantity,
            UnitPrice = _unitPrice,
            Active = _active
        };
    }
}

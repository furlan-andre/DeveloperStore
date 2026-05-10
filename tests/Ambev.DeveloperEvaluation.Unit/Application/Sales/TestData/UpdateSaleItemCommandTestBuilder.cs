using Ambev.DeveloperEvaluation.Application.Sales.UpdateSaleItem;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

public class UpdateSaleItemCommandTestBuilder
{
    private static readonly Faker Faker = new();

    private Guid? _id;
    private Guid _productId = Guid.NewGuid();
    private string _productDescription = Faker.Commerce.ProductName();
    private int _quantity = Faker.Random.Int(1, 3);
    private decimal _unitPrice = Faker.Finance.Amount(1m, 1000m);
    private bool _active = true;

    public UpdateSaleItemCommandTestBuilder WithId(Guid? id)
    {
        _id = id;
        return this;
    }

    public UpdateSaleItemCommandTestBuilder WithProductId(Guid productId)
    {
        _productId = productId;
        return this;
    }

    public UpdateSaleItemCommandTestBuilder WithProductDescription(string productDescription)
    {
        _productDescription = productDescription;
        return this;
    }

    public UpdateSaleItemCommandTestBuilder WithQuantity(int quantity)
    {
        _quantity = quantity;
        return this;
    }

    public UpdateSaleItemCommandTestBuilder WithUnitPrice(decimal unitPrice)
    {
        _unitPrice = unitPrice;
        return this;
    }

    public UpdateSaleItemCommandTestBuilder WithActive(bool active)
    {
        _active = active;
        return this;
    }

    public UpdateSaleItemCommand Build()
    {
        return new UpdateSaleItemCommand
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

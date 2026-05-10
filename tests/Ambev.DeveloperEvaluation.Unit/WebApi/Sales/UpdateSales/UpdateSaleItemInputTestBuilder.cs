using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSaleItem;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Sales.UpdateSales;

public class UpdateSaleItemInputTestBuilder
{
    private static readonly Faker Faker = new();

    private Guid? _id;
    private Guid _productId = Guid.NewGuid();
    private string _productDescription = Faker.Commerce.ProductName();
    private int _quantity = Faker.Random.Int(1, 3);
    private decimal _unitPrice = Faker.Finance.Amount(1m, 1000m);
    private bool _active = true;

    public UpdateSaleItemInputTestBuilder WithId(Guid? id)
    {
        _id = id;
        return this;
    }

    public UpdateSaleItemInputTestBuilder WithProductId(Guid productId)
    {
        _productId = productId;
        return this;
    }

    public UpdateSaleItemInputTestBuilder WithProductDescription(string productDescription)
    {
        _productDescription = productDescription;
        return this;
    }

    public UpdateSaleItemInputTestBuilder WithQuantity(int quantity)
    {
        _quantity = quantity;
        return this;
    }

    public UpdateSaleItemInputTestBuilder WithUnitPrice(decimal unitPrice)
    {
        _unitPrice = unitPrice;
        return this;
    }

    public UpdateSaleItemInputTestBuilder WithActive(bool active)
    {
        _active = active;
        return this;
    }

    public UpdateSaleItemInput Build()
    {
        return new UpdateSaleItemInput
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

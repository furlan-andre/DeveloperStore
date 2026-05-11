using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSaleItem;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSaleItem;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSaleItem;
using Bogus;

namespace Ambev.DeveloperEvaluation.Functional.Sales.TestData;

public sealed class UpdateSaleItemPayloadBuilder
{
    private static readonly Faker Faker = new();

    private Guid? _id;
    private Guid _productId = Guid.NewGuid();
    private string _productDescription = Faker.Commerce.ProductName();
    private int _quantity = 4;
    private decimal _unitPrice = 100m;
    private bool _active = true;

    public UpdateSaleItemPayloadBuilder FromCreateItem(CreateSaleItemInput item)
    {
        _productId = item.ProductId;
        _productDescription = item.ProductDescription;
        _quantity = item.Quantity;
        _unitPrice = item.UnitPrice;
        return this;
    }

    public UpdateSaleItemPayloadBuilder FromGetItem(GetSaleItemResult item)
    {
        _id = item.Id;
        _productId = item.ProductId;
        _productDescription = item.ProductDescription;
        _quantity = item.Quantity;
        _unitPrice = item.UnitPrice;
        _active = item.Active;
        return this;
    }

    public UpdateSaleItemPayloadBuilder WithId(Guid? id)
    {
        _id = id;
        return this;
    }

    public UpdateSaleItemPayloadBuilder WithQuantity(int quantity)
    {
        _quantity = quantity;
        return this;
    }

    public UpdateSaleItemPayloadBuilder WithUnitPrice(decimal unitPrice)
    {
        _unitPrice = unitPrice;
        return this;
    }

    public UpdateSaleItemPayloadBuilder WithActive(bool active)
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

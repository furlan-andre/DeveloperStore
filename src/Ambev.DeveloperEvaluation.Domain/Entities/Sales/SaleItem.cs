using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Services.Sales;

namespace Ambev.DeveloperEvaluation.Domain.Entities.Sales;

public class SaleItem
{
    public Guid Id { get; private set; }
    public Product Product { get; private set; } = null!;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Discount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public bool Active { get; private set; }

    private SaleItem()
    {
    }

    public SaleItem(Product? product, int quantity, decimal unitPrice, ISaleDiscountPolicy? discountPolicy)
    {
        if (product is null)
            throw new DomainException("Product is required.");

        if (discountPolicy is null)
            throw new DomainException("Discount policy is required.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        if (quantity > 20)
            throw new DomainException("Quantity cannot be greater than 20.");

        if (unitPrice <= 0)
            throw new DomainException("Unit price must be greater than zero.");

        var subtotal = quantity * unitPrice;
        var discount = discountPolicy.CalculateDiscount(quantity, unitPrice);
        var totalAmount = subtotal - discount;

        if (discount < 0)
            throw new DomainException("Discount cannot be negative.");

        if (discount > subtotal)
            throw new DomainException("Discount cannot be greater than subtotal.");

        if (totalAmount < 0)
            throw new DomainException("Total amount cannot be negative.");

        Id = Guid.NewGuid();
        Product = product;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = discount;
        TotalAmount = totalAmount;
        Active = true;
    }

    public void UpdateFrom(SaleItem? saleItem, bool active)
    {
        if (saleItem is null)
            throw new DomainException("Sale item is required.");

        Product = saleItem.Product;
        Quantity = saleItem.Quantity;
        UnitPrice = saleItem.UnitPrice;
        Discount = saleItem.Discount;
        TotalAmount = saleItem.TotalAmount;
        Active = active;
    }

    public void SetActive(bool active)
    {
        Active = active;
    }
}

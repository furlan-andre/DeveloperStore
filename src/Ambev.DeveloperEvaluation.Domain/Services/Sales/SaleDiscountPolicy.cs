using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Services.Sales;

public sealed class SaleDiscountPolicy : ISaleDiscountPolicy
{
    public decimal CalculateDiscount(int quantity, decimal unitPrice)
    {
        if (quantity < 1)
            throw new DomainException("Quantity must be greater than zero.");

        if (quantity > 20)
            throw new DomainException("Quantity cannot be greater than 20.");

        if (unitPrice <= 0)
            throw new DomainException("Unit price must be greater than zero.");

        var subtotal = quantity * unitPrice;

        if (quantity >= 10)
            return subtotal * 0.20m;

        if (quantity >= 4)
            return subtotal * 0.10m;

        return 0m;
    }
}

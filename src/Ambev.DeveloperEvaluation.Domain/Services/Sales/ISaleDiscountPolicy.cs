namespace Ambev.DeveloperEvaluation.Domain.Services.Sales;

public interface ISaleDiscountPolicy
{
    decimal CalculateDiscount(int quantity, decimal unitPrice);
}

using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public class ListSalesCommandValidator : AbstractValidator<ListSalesCommand>
{
    private static readonly HashSet<string> AllowedFilterFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "id",
        "saleNumber",
        "saleDate",
        "_minSaleDate",
        "_maxSaleDate",
        "customerId",
        "customerName",
        "branchId",
        "branchName",
        "_minTotalSaleAmount",
        "_maxTotalSaleAmount",
        "active"
    };

    private static readonly HashSet<string> AllowedOrderFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "id",
        "saleNumber",
        "saleDate",
        "customerId",
        "customerName",
        "branchId",
        "branchName",
        "totalSaleAmount",
        "active"
    };

    public ListSalesCommandValidator()
    {
        RuleFor(sale => sale.Page).GreaterThanOrEqualTo(1);
        RuleFor(sale => sale.Size).InclusiveBetween(1, 100);
        RuleForEach(sale => sale.Filters.Keys)
            .Must(field => AllowedFilterFields.Contains(field))
            .WithMessage("Unsupported sale filter field.");
        RuleFor(sale => sale.Order)
            .Must(HaveValidOrderFields)
            .WithMessage("Unsupported sale order field.");
    }

    private static bool HaveValidOrderFields(string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
            return true;

        var orderParts = order
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return orderParts.All(orderPart =>
        {
            var field = orderPart.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[0];
            return AllowedOrderFields.Contains(field);
        });
    }
}

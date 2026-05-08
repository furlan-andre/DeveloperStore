using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities.Sales;

public class Sale
{
    private readonly List<SaleItem> _items = [];

    public Guid Id { get; private set; }
    public string SaleNumber { get; private set; } = string.Empty;
    public DateTime SaleDate { get; private set; }
    public Customer Customer { get; private set; } = null!;
    public Branch Branch { get; private set; } = null!;
    public decimal TotalSaleAmount { get; private set; }
    public bool Active { get; private set; }
    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

    private Sale()
    {
    }

    public static Sale Create(
        string? saleNumber,
        DateTime saleDate,
        Customer? customer,
        Branch? branch,
        IEnumerable<SaleItem?>? items)
    {
        if (string.IsNullOrWhiteSpace(saleNumber))
            throw new DomainException("Sale number is required.");

        if (saleDate == default)
            throw new DomainException("Sale date is required.");

        if (customer is null)
            throw new DomainException("Customer is required.");

        if (branch is null)
            throw new DomainException("Branch is required.");

        if (items is null)
            throw new DomainException("Sale items are required.");

        var saleItems = items.ToList();

        if (saleItems.Count <= 0)
            throw new DomainException("Sale must have at least one item.");

        if (saleItems.Any(item => item is null))
            throw new DomainException("Sale items cannot contain null values.");

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = saleNumber,
            SaleDate = saleDate,
            Customer = customer,
            Branch = branch,
            Active = true
        };

        sale._items.AddRange(saleItems!);
        sale.TotalSaleAmount = sale._items.Sum(item => item.TotalAmount);

        return sale;
    }
}

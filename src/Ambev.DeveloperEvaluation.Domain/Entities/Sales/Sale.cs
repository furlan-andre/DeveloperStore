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
        ValidateSaleData(saleNumber, saleDate, customer, branch);
        var saleItems = ValidateSaleItems(items, item => item is null);

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = saleNumber!,
            SaleDate = saleDate,
            Customer = customer!,
            Branch = branch!,
            Active = true
        };

        sale._items.AddRange(saleItems);
        sale.RecalculateTotalSaleAmount();

        return sale;
    }

    public void Update(
        string? saleNumber,
        DateTime saleDate,
        Customer? customer,
        Branch? branch,
        bool active,
        IEnumerable<SaleItemUpdateData?>? items)
    {
        ValidateSaleData(saleNumber, saleDate, customer, branch);
        var updateItems = ValidateUpdateItems(items);
        var reconciledItems = ReconcileItems(updateItems);

        SaleNumber = saleNumber!;
        SaleDate = saleDate;
        Customer = customer!;
        Branch = branch!;
        Active = active;
        
        _items.Clear();
        _items.AddRange(reconciledItems);
        
        RecalculateTotalSaleAmount();
    }

    private List<SaleItem> ReconcileItems(List<SaleItemUpdateData> updateItems)
    {
        var reconciledItems = new List<SaleItem>();

        foreach (var updateItem in updateItems)
        {
            if (IsNewItem(updateItem.Id))
            {
                updateItem.SaleItem!.SetActive(updateItem.Active);
                reconciledItems.Add(updateItem.SaleItem!);
                continue;
            }

            var existingItem = GetExistingItem(updateItem.Id!.Value);
            existingItem.UpdateFrom(updateItem.SaleItem, updateItem.Active);
            reconciledItems.Add(existingItem);
        }

        return reconciledItems;
    }

    private void RecalculateTotalSaleAmount()
    {
        TotalSaleAmount = _items
            .Where(item => item.Active)
            .Sum(item => item.TotalAmount);
    }

    private SaleItem GetExistingItem(Guid itemId)
    {
        var existingItem = _items.FirstOrDefault(item => item.Id == itemId);

        if (existingItem is null)
            throw new DomainException("Sale item does not belong to this sale.");

        return existingItem;
    }

    private static bool IsNewItem(Guid? itemId)
    {
        return !itemId.HasValue || itemId.Value == Guid.Empty;
    }

    private static void ValidateSaleData(
        string? saleNumber,
        DateTime saleDate,
        Customer? customer,
        Branch? branch)
    {
        if (string.IsNullOrWhiteSpace(saleNumber))
            throw new DomainException("Sale number is required.");

        if (saleDate == default)
            throw new DomainException("Sale date is required.");

        if (customer is null)
            throw new DomainException("Customer is required.");

        if (branch is null)
            throw new DomainException("Branch is required.");
    }

    private static List<T> ValidateSaleItems<T>(
        IEnumerable<T?>? items,
        Func<T?, bool> hasInvalidItem)
        where T : class
    {
        if (items is null)
            throw new DomainException("Sale items are required.");

        var saleItems = items.ToList();

        if (saleItems.Count <= 0)
            throw new DomainException("Sale must have at least one item.");

        if (saleItems.Any(hasInvalidItem))
            throw new DomainException("Sale items cannot contain null values.");

        return saleItems.Select(item => item!).ToList();
    }

    private static List<SaleItemUpdateData> ValidateUpdateItems(IEnumerable<SaleItemUpdateData?>? items)
    {
        var updateItems = ValidateSaleItems(items, item => item is null || item.SaleItem is null);
        
        var existingItemIds = updateItems
            .Where(item => !IsNewItem(item.Id))
            .Select(item => item.Id!.Value)
            .ToList();

        if (existingItemIds.Count != existingItemIds.Distinct().Count())
            throw new DomainException("Sale items cannot contain duplicated ids.");

        return updateItems;
    }
}

using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public sealed class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sale);

        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(sale => sale.Items)
            .FirstOrDefaultAsync(sale => sale.Id == id, cancellationToken);
    }

    public async Task<Sale?> GetByIdAsNoTrackingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .AsNoTracking()
            .Include(sale => sale.Items)
            .FirstOrDefaultAsync(sale => sale.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Sale>> ListAsync(
        SaleListQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var sales = _context.Sales.AsNoTracking().AsQueryable();

        sales = ApplyFilters(sales, query.Filters);
        sales = ApplyOrdering(sales, query.Order);

        var totalCount = await sales.CountAsync(cancellationToken);
        var items = await sales
            .Skip((query.Page - 1) * query.Size)
            .Take(query.Size)
            .ToListAsync(cancellationToken);

        return new PagedResult<Sale>
        {
            Items = items,
            CurrentPage = query.Page,
            PageSize = query.Size,
            TotalItems = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.Size)
        };
    }

    public async Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sale);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sale);

        await _context.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Sale> ApplyFilters(
        IQueryable<Sale> sales,
        IReadOnlyDictionary<string, string?> filters)
    {
        foreach (var filter in filters)
        {
            if (string.IsNullOrWhiteSpace(filter.Value))
                continue;

            sales = filter.Key switch
            {
                "id" => Guid.TryParse(filter.Value, out var value) 
                    ? sales.Where(sale => sale.Id == value): sales,
                
                "saleNumber" => ApplyStringFilter(sales, sale => sale.SaleNumber, filter.Value),
                
                "saleDate" => DateTime.TryParse(filter.Value, out var value) 
                    ? sales.Where(sale => sale.SaleDate == value) : sales,
                
                "_minSaleDate" => DateTime.TryParse(filter.Value, out var value) 
                    ? sales.Where(sale => sale.SaleDate >= value) : sales,
                
                "_maxSaleDate" => DateTime.TryParse(filter.Value, out var value) 
                    ? sales.Where(sale => sale.SaleDate <= value) : sales,
                
                "customerId" => Guid.TryParse(filter.Value, out var value) 
                    ? sales.Where(sale => sale.Customer.Id == value) : sales,
                
                "customerName" => ApplyStringFilter(sales, sale => sale.Customer.Name, filter.Value),
                
                "branchId" => Guid.TryParse(filter.Value, out var value)
                    ? sales.Where(sale => sale.Branch.Id == value) : sales,
                "branchName" => ApplyStringFilter(sales, sale => sale.Branch.Name, filter.Value),
                
                "_minTotalSaleAmount" => decimal.TryParse(filter.Value, out var value)
                    ? sales.Where(sale => sale.TotalSaleAmount >= value) : sales,
                
                "_maxTotalSaleAmount" => decimal.TryParse(filter.Value, out var value)
                    ? sales.Where(sale => sale.TotalSaleAmount <= value) : sales,
                
                "active" => bool.TryParse(filter.Value, out var value) 
                    ? sales.Where(sale => sale.Active == value) : sales,
                _ => sales
            };
        }

        return sales;
    }

    private static IQueryable<Sale> ApplyStringFilter(
        IQueryable<Sale> sales,
        Expression<Func<Sale, string>> property,
        string value)
    {
        var startsWithWildcard = value.StartsWith('*');
        var endsWithWildcard = value.EndsWith('*');
        var normalizedValue = value.Trim('*');

        if (startsWithWildcard && endsWithWildcard)
            return sales.Where(BuildStringExpression(property, normalizedValue, StringFilterMode.Contains));

        if (startsWithWildcard)
            return sales.Where(BuildStringExpression(property, normalizedValue, StringFilterMode.EndsWith));

        if (endsWithWildcard)
            return sales.Where(BuildStringExpression(property, normalizedValue, StringFilterMode.StartsWith));

        return sales.Where(BuildStringExpression(property, normalizedValue, StringFilterMode.Equals));
    }

    private static Expression<Func<Sale, bool>> BuildStringExpression(
        Expression<Func<Sale, string>> property,
        string value,
        StringFilterMode mode)
    {
        var methodName = mode switch
        {
            StringFilterMode.Contains => nameof(string.Contains),
            StringFilterMode.StartsWith => nameof(string.StartsWith),
            StringFilterMode.EndsWith => nameof(string.EndsWith),
            _ => string.Empty
        };

        if (mode == StringFilterMode.Equals)
        {
            return Expression.Lambda<Func<Sale, bool>>(
                Expression.Equal(property.Body, Expression.Constant(value)),
                property.Parameters);
        }

        return Expression.Lambda<Func<Sale, bool>>(
            Expression.Call(property.Body, methodName, Type.EmptyTypes, Expression.Constant(value)),
            property.Parameters);
    }

    private static IQueryable<Sale> ApplyOrdering(IQueryable<Sale> sales, string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
            return sales.OrderBy(sale => sale.SaleDate);

        IOrderedQueryable<Sale>? orderedSales = null;
        var orderParts = order.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var orderPart in orderParts)
        {
            var segments = orderPart.Split(' ', 
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            
            var field = segments[0];
            
            var descending = segments.Length > 1 
                             && segments[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

            orderedSales = ApplyOrderingField(orderedSales ?? sales, field, descending, orderedSales is not null);
        }

        return orderedSales ?? sales.OrderBy(sale => sale.SaleDate);
    }

    private static IOrderedQueryable<Sale> ApplyOrderingField(
        IQueryable<Sale> sales,
        string field,
        bool descending,
        bool thenBy)
    {
        return field switch
        {
            "id" => OrderBy(sales, sale => sale.Id, descending, thenBy),
            "saleNumber" => OrderBy(sales, sale => sale.SaleNumber, descending, thenBy),
            "saleDate" => OrderBy(sales, sale => sale.SaleDate, descending, thenBy),
            "customerId" => OrderBy(sales, sale => sale.Customer.Id, descending, thenBy),
            "customerName" => OrderBy(sales, sale => sale.Customer.Name, descending, thenBy),
            "branchId" => OrderBy(sales, sale => sale.Branch.Id, descending, thenBy),
            "branchName" => OrderBy(sales, sale => sale.Branch.Name, descending, thenBy),
            "totalSaleAmount" => OrderBy(sales, sale => sale.TotalSaleAmount, descending, thenBy),
            "active" => OrderBy(sales, sale => sale.Active, descending, thenBy),
            _ => OrderBy(sales, sale => sale.SaleDate, descending, thenBy)
        };
    }

    private static IOrderedQueryable<Sale> OrderBy<TKey>(
        IQueryable<Sale> sales,
        Expression<Func<Sale, TKey>> keySelector,
        bool descending,
        bool thenBy)
    {
        if (thenBy && sales is IOrderedQueryable<Sale> orderedSales)
            return descending ? orderedSales.ThenByDescending(keySelector) : orderedSales.ThenBy(keySelector);

        return descending ? sales.OrderByDescending(keySelector) : sales.OrderBy(keySelector);
    }

    private enum StringFilterMode
    {
        Equals,
        Contains,
        StartsWith,
        EndsWith
    }
}

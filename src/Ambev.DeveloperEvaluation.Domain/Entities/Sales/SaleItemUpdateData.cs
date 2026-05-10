namespace Ambev.DeveloperEvaluation.Domain.Entities.Sales;

public sealed record SaleItemUpdateData(Guid? Id, SaleItem? SaleItem, bool Active = true);

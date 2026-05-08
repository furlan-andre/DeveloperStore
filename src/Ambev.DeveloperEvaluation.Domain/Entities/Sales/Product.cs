using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities.Sales;

public class Product
{
    public Guid Id { get; private set; }
    public string Description { get; private set; } = null!;

    private Product()
    {
    }

    public Product(Guid id, string? description)
    {
        if (id == Guid.Empty)
            throw new DomainException("Product id cannot be empty.");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Product description is required.");

        Id = id;
        Description = description;
    }
}

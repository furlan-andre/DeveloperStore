using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities.Sales;

public class Customer
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    public Customer(Guid id, string? name)
    {
        if (id == Guid.Empty)
            throw new DomainException("Customer id cannot be empty.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Customer name is required.");

        Id = id;
        Name = name;
    }
}

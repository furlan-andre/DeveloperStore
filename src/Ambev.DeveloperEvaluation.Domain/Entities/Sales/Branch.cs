using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities.Sales;

public class Branch
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    public Branch(Guid id, string? name)
    {
        if (id == Guid.Empty)
            throw new DomainException("Branch id cannot be empty.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Branch name is required.");

        Id = id;
        Name = name;
    }
}

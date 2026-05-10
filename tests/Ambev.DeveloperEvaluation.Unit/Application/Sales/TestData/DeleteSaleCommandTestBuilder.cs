using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

public class DeleteSaleCommandTestBuilder
{
    private Guid _id = Guid.NewGuid();

    public DeleteSaleCommandTestBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public DeleteSaleCommand Build()
    {
        return new DeleteSaleCommand
        {
            Id = _id
        };
    }
}

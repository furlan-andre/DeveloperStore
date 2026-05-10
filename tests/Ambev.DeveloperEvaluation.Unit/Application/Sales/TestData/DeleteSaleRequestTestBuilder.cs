using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

public class DeleteSaleRequestTestBuilder
{
    private Guid _id = Guid.NewGuid();

    public DeleteSaleRequestTestBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public DeleteSaleRequest Build()
    {
        return new DeleteSaleRequest
        {
            Id = _id
        };
    }
}

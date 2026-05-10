using Ambev.DeveloperEvaluation.Application.Sales.GetSale;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

public class GetSaleCommandTestBuilder
{
    private Guid _id = Guid.NewGuid();

    public GetSaleCommandTestBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public GetSaleCommand Build()
    {
        return new GetSaleCommand
        {
            Id = _id
        };
    }
}

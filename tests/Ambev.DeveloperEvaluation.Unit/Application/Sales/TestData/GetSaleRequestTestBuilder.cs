using Ambev.DeveloperEvaluation.Application.Sales.GetSale;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

public class GetSaleRequestTestBuilder
{
    private Guid _id = Guid.NewGuid();

    public GetSaleRequestTestBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public GetSaleRequest Build()
    {
        return new GetSaleRequest
        {
            Id = _id
        };
    }
}

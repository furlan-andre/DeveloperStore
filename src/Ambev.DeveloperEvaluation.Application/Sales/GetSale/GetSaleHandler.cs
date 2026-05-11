using Ambev.DeveloperEvaluation.Application.Sales.Service;
using Ambev.DeveloperEvaluation.Common.Results;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleHandler : IRequestHandler<GetSaleCommand, Result<GetSaleResponse>>
{
    private readonly IGetSaleService _getSaleService;
    private readonly IMapper _mapper;

    public GetSaleHandler(IGetSaleService getSaleService, IMapper mapper)
    {
        _getSaleService = getSaleService;
        _mapper = mapper;
    }

    public async Task<Result<GetSaleResponse>> Handle(GetSaleCommand command, CancellationToken cancellationToken)
    {
        var request = _mapper.Map<GetSaleRequest>(command);
        return await _getSaleService.GetByIdAsync(request, cancellationToken);
    }
}

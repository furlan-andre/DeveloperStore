using Ambev.DeveloperEvaluation.Application.Sales.Service;
using Ambev.DeveloperEvaluation.Common.Results;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, Result<DeleteSaleResponse>>
{
    private readonly IDeleteSaleService _deleteSaleService;
    private readonly IMapper _mapper;

    public DeleteSaleHandler(IDeleteSaleService deleteSaleService, IMapper mapper)
    {
        _deleteSaleService = deleteSaleService;
        _mapper = mapper;
    }

    public async Task<Result<DeleteSaleResponse>> Handle(DeleteSaleCommand command, CancellationToken cancellationToken)
    {
        var request = _mapper.Map<DeleteSaleRequest>(command);
        return await _deleteSaleService.DeleteAsync(request, cancellationToken);
    }
}

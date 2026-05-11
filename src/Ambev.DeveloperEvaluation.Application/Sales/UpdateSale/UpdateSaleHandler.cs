using Ambev.DeveloperEvaluation.Application.Sales.Service;
using Ambev.DeveloperEvaluation.Common.Results;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, Result<UpdateSaleResponse>>
{
    private readonly IUpdateSaleService _updateSaleService;
    private readonly IMapper _mapper;

    public UpdateSaleHandler(IUpdateSaleService updateSaleService, IMapper mapper)
    {
        _updateSaleService = updateSaleService;
        _mapper = mapper;
    }

    public async Task<Result<UpdateSaleResponse>> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var request = _mapper.Map<UpdateSaleRequest>(command);
        return await _updateSaleService.UpdateAsync(request, cancellationToken);
    }
}

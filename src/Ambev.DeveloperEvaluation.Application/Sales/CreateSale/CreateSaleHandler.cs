using Ambev.DeveloperEvaluation.Application.Sales.Service;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResponse>
{
    private readonly ICreateSaleService _createSaleService;
    private readonly IMapper _mapper;

    public CreateSaleHandler(ICreateSaleService createSaleService, IMapper mapper)
    {
        _createSaleService = createSaleService;
        _mapper = mapper;
    }

    public async Task<CreateSaleResponse> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var request = _mapper.Map<CreateSaleRequest>(command);
        return await _createSaleService.CreateAsync(request, cancellationToken);
    }
}

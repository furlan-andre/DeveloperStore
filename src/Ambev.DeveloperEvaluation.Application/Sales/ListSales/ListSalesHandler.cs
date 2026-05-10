using Ambev.DeveloperEvaluation.Application.Common.Pagination;
using Ambev.DeveloperEvaluation.Application.Sales.Service;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public class ListSalesHandler : IRequestHandler<ListSalesCommand, PagedResponse<ListSaleResponse>>
{
    private readonly IListSalesService _listSalesService;
    private readonly IMapper _mapper;

    public ListSalesHandler(IListSalesService listSalesService, IMapper mapper)
    {
        _listSalesService = listSalesService;
        _mapper = mapper;
    }

    public async Task<PagedResponse<ListSaleResponse>> Handle(
        ListSalesCommand command,
        CancellationToken cancellationToken)
    {
        var request = _mapper.Map<ListSalesRequest>(command);
        return await _listSalesService.ListAsync(request, cancellationToken);
    }
}

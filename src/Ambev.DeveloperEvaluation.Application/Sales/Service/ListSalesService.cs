using Ambev.DeveloperEvaluation.Application.Common.Pagination;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Common.Results;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.Service;

public class ListSalesService : IListSalesService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public ListSalesService(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<Result<PagedResponse<ListSaleResponse>>> ListAsync(
        ListSalesRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = await _saleRepository.ListAsync(
            new SaleListQuery
            {
                Page = request.Page,
                Size = request.Size,
                Order = request.Order,
                Filters = request.Filters
            },
            cancellationToken);

        var response = new PagedResponse<ListSaleResponse>
        {
            Items = _mapper.Map<IReadOnlyCollection<ListSaleResponse>>(result.Items),
            CurrentPage = result.CurrentPage,
            TotalPages = result.TotalPages,
            TotalItems = result.TotalItems,
            PageSize = result.PageSize
        };

        return Result<PagedResponse<ListSaleResponse>>.Success(response);
    }
}

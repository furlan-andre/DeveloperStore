using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.Service;

public class DeleteSaleService : IDeleteSaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public DeleteSaleService(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<DeleteSaleResponse> DeleteAsync(
        DeleteSaleRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (sale is null)
            throw new KeyNotFoundException($"Sale with id {request.Id} was not found.");

        sale.Delete();

        await _saleRepository.DeleteAsync(sale, cancellationToken);

        return _mapper.Map<DeleteSaleResponse>(sale);
    }
}

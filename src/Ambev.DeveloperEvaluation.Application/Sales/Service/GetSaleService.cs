using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Common.Results;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.Service;

public class GetSaleService : IGetSaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public GetSaleService(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetSaleResponse>> GetByIdAsync(
        GetSaleRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var sale = await _saleRepository.GetByIdAsNoTrackingAsync(request.Id, cancellationToken);

        if (sale is null)
        {
            return Result<GetSaleResponse>.Failure(CreateSaleNotFoundError(request.Id));
        }

        return Result<GetSaleResponse>.Success(_mapper.Map<GetSaleResponse>(sale));
    }

    private static Error CreateSaleNotFoundError(Guid saleId)
    {
        return Error.ResourceNotFound(
            "Sale not found",
            $"The sale with ID {saleId} does not exist.");
    }
}

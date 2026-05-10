using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Mappings;

public class DeleteSaleProfile : Profile
{
    public DeleteSaleProfile()
    {
        CreateMap<DeleteSaleResponse, DeleteSaleResult>();
    }
}

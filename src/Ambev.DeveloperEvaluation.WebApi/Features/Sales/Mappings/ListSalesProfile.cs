using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Mappings;

public class ListSalesProfile : Profile
{
    public ListSalesProfile()
    {
        CreateMap<ListSaleResponse, ListSaleResult>();
    }
}

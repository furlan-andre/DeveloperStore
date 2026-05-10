using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSaleItem;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSaleItem;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Mappings;

public class UpdateSaleProfile : Profile
{
    public UpdateSaleProfile()
    {
        CreateMap<UpdateSaleInput, UpdateSaleCommand>();
        CreateMap<UpdateSaleItemInput, UpdateSaleItemCommand>();
        CreateMap<UpdateSaleResponse, UpdateSaleResult>();
        CreateMap<UpdateSaleItemResponse, UpdateSaleItemResult>();
    }
}

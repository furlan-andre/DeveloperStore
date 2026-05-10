using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSaleItem;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSaleItem;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Mappings;

public class CreateSaleProfile : Profile
{
    public CreateSaleProfile()
    {
        CreateMap<CreateSaleInput, CreateSaleCommand>();
        CreateMap<CreateSaleItemInput, CreateSaleItemCommand>();
        CreateMap<CreateSaleResponse, CreateSaleResult>();
        CreateMap<CreateSaleItemResponse, CreateSaleItemResult>();
    }
}

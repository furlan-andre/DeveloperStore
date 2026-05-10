using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSaleItem;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.Mappings;

public class GetSaleProfile : Profile
{
    public GetSaleProfile()
    {
        CreateMap<GetSaleCommand, GetSaleRequest>();

        CreateMap<Sale, GetSaleResponse>()
            .ForMember(destination => destination.CustomerId, options => options.MapFrom(source => source.Customer.Id))
            .ForMember(destination => destination.CustomerName, options => options.MapFrom(source => source.Customer.Name))
            .ForMember(destination => destination.BranchId, options => options.MapFrom(source => source.Branch.Id))
            .ForMember(destination => destination.BranchName, options => options.MapFrom(source => source.Branch.Name));

        CreateMap<SaleItem, GetSaleItemResponse>()
            .ForMember(destination => destination.ProductId, options => options.MapFrom(source => source.Product.Id))
            .ForMember(destination => destination.ProductDescription, options => options.MapFrom(source => source.Product.Description));
    }
}

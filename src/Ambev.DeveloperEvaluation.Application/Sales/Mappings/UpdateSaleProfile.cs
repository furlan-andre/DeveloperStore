using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSaleItem;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.Mappings;

public class UpdateSaleProfile : Profile
{
    public UpdateSaleProfile()
    {
        CreateMap<UpdateSaleCommand, UpdateSaleRequest>();
        CreateMap<UpdateSaleItemCommand, UpdateSaleItemRequest>();

        CreateMap<Sale, UpdateSaleResponse>()
            .ForMember(destination => destination.CustomerId, options => options.MapFrom(source => source.Customer.Id))
            .ForMember(destination => destination.CustomerName, options => options.MapFrom(source => source.Customer.Name))
            .ForMember(destination => destination.BranchId, options => options.MapFrom(source => source.Branch.Id))
            .ForMember(destination => destination.BranchName, options => options.MapFrom(source => source.Branch.Name));

        CreateMap<SaleItem, UpdateSaleItemResponse>()
            .ForMember(destination => destination.ProductId, options => options.MapFrom(source => source.Product.Id))
            .ForMember(destination => destination.ProductDescription, options => options.MapFrom(source => source.Product.Description));
    }
}

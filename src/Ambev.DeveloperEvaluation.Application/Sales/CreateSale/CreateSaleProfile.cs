using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleProfile : Profile
{
    public CreateSaleProfile()
    {
        CreateMap<Sale, CreateSaleResult>()
            .ForMember(destination => destination.CustomerId, options => options.MapFrom(source => source.Customer.Id))
            .ForMember(destination => destination.CustomerName, options => options.MapFrom(source => source.Customer.Name))
            .ForMember(destination => destination.BranchId, options => options.MapFrom(source => source.Branch.Id))
            .ForMember(destination => destination.BranchName, options => options.MapFrom(source => source.Branch.Name));

        CreateMap<SaleItem, CreateSaleItemResult>()
            .ForMember(destination => destination.ProductId, options => options.MapFrom(source => source.Product.Id))
            .ForMember(destination => destination.ProductDescription, options => options.MapFrom(source => source.Product.Description));
    }
}

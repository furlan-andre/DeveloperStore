using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.Mappings;

public class ListSalesProfile : Profile
{
    public ListSalesProfile()
    {
        CreateMap<ListSalesCommand, ListSalesRequest>();

        CreateMap<Sale, ListSaleResponse>()
            .ForMember(destination => destination.CustomerId, options => options.MapFrom(source => source.Customer.Id))
            .ForMember(destination => destination.CustomerName, options => options.MapFrom(source => source.Customer.Name))
            .ForMember(destination => destination.BranchId, options => options.MapFrom(source => source.Branch.Id))
            .ForMember(destination => destination.BranchName, options => options.MapFrom(source => source.Branch.Name));
    }
}

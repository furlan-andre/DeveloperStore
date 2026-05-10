using Ambev.DeveloperEvaluation.Application.Common.Pagination;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public sealed class ListSalesCommand : PagedRequest, IRequest<PagedResponse<ListSaleResponse>>;

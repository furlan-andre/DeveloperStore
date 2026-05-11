using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Application.Sales.Mappings;
using Ambev.DeveloperEvaluation.Application.Sales.Service;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Ambev.DeveloperEvaluation.TestUtils.Domain.Sales.Builders;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using AutoMapper;
using Ambev.DeveloperEvaluation.Unit.Common;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class ListSalesServiceTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ListSalesService _service;

    public ListSalesServiceTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();

        var mapperConfiguration = AutoMapperTestHelper.CreateConfiguration(configuration =>
        {
            configuration.AddProfile<ListSalesProfile>();
        });

        _mapper = mapperConfiguration.CreateMapper();
        _service = new ListSalesService(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Should have valid AutoMapper configuration")]
    public void Given_ListSalesProfile_When_ValidatingAutoMapper_Then_ShouldBeValid()
    {
        var mapperConfiguration = AutoMapperTestHelper.CreateConfiguration(configuration =>
        {
            configuration.AddProfile<ListSalesProfile>();
        });

        mapperConfiguration.AssertConfigurationIsValid();
    }

    [Fact(DisplayName = "Should throw ArgumentNullException when request is null")]
    public async Task Given_NullRequest_When_ListingSales_Then_ShouldThrowArgumentNullException()
    {
        ListSalesRequest? request = null;

        var action = async () => await _service.ListAsync(request!);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "Should send list query to repository")]
    public async Task Given_ValidRequest_When_ListingSales_Then_ShouldSendQueryToRepository()
    {
        var page = 2;
        var size = 20;
        var order = "saleDate desc";
        IReadOnlyDictionary<string, string?> filters = new Dictionary<string, string?>
        {
            ["active"] = "true"
        };
        
        var request = new ListSalesRequestTestBuilder()
            .WithPage(page)
            .WithSize(size)
            .WithOrder(order)
            .WithFilters(filters)
            .Build();

        _saleRepository
            .ListAsync(Arg.Any<SaleListQuery>(), Arg.Any<CancellationToken>())
            .Returns(new PagedResult<Sale>());

        await _service.ListAsync(request);

        await _saleRepository.Received(1).ListAsync(
            Arg.Is<SaleListQuery>(query =>
                query.Page == page &&
                query.Size == size &&
                query.Order == order &&
                query.Filters == filters),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should return paged sales response")]
    public async Task Given_RepositoryResult_When_ListingSales_Then_ShouldReturnPagedResponse()
    {
        var page = 2;
        var size = 10;
        var totalItems = 21;
        var totalPages = 3;
        var sale = new SaleTestBuilder().Build();
        
        var request = new ListSalesRequestTestBuilder()
            .WithPage(page)
            .WithSize(size)
            .Build();
        
        var repositoryResult = new PagedResult<Sale>
        {
            Items = [sale],
            CurrentPage = page,
            PageSize = size,
            TotalItems = totalItems,
            TotalPages = totalPages
        };

        _saleRepository
            .ListAsync(Arg.Any<SaleListQuery>(), Arg.Any<CancellationToken>())
            .Returns(repositoryResult);

        var result = await _service.ListAsync(request);
        var response = result.Value;

        result.IsSuccess.Should().BeTrue();
        response.CurrentPage.Should().Be(page);
        response.PageSize.Should().Be(size);
        response.TotalItems.Should().Be(totalItems);
        response.TotalPages.Should().Be(totalPages);
        response.Items.Should().ContainSingle();

        var responseSale = response.Items.Single();
        responseSale.Id.Should().Be(sale.Id);
        responseSale.SaleNumber.Should().Be(sale.SaleNumber);
        responseSale.CustomerId.Should().Be(sale.Customer.Id);
        responseSale.CustomerName.Should().Be(sale.Customer.Name);
        responseSale.BranchId.Should().Be(sale.Branch.Id);
        responseSale.BranchName.Should().Be(sale.Branch.Name);
        responseSale.TotalSaleAmount.Should().Be(sale.TotalSaleAmount);
        responseSale.Active.Should().Be(sale.Active);
    }
}

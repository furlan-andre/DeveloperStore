using System.Net;
using Ambev.DeveloperEvaluation.Functional.Common;
using Ambev.DeveloperEvaluation.Functional.Sales.TestData;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Sales;

public sealed class ListSalesFunctionalTests : FunctionalTestBase
{
    public ListSalesFunctionalTests(PostgreSqlFunctionalFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "GET /api/sales should return paginated response")]
    public async Task Given_ExistingSales_When_Listing_Then_ShouldReturnPaginatedResponse()
    {
        var firstPayload = new CreateSalePayloadBuilder()
            .WithSaleNumber("SALE-LIST-001")
            .Build();
        
        var secondPayload = new CreateSalePayloadBuilder()
            .WithSaleNumber("SALE-LIST-002")
            .Build();
        
        await CreateSaleAsync(firstPayload);
        await CreateSaleAsync(secondPayload);

        var response = await Client.GetAsync("/api/sales?_page=1&_size=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var apiResponse = await ReadSuccessResponseAsync<PaginatedResponse<ListSaleResult>>(response);
        
        apiResponse.Success.Should().BeTrue();
        apiResponse.Message.Should().Be("Sales retrieved successfully");
        apiResponse.CurrentPage.Should().Be(1);
        apiResponse.TotalItems.Should().Be(2);
        apiResponse.TotalPages.Should().Be(1);
        apiResponse.Data.Should().NotBeNull();
        
        apiResponse.Data!.Select(sale => sale.SaleNumber)
            .Should()
            .BeEquivalentTo([firstPayload.SaleNumber, secondPayload.SaleNumber]);
    }

    [Fact(DisplayName = "GET /api/sales should filter by sale number")]
    public async Task Given_FilterBySaleNumber_When_Listing_Then_ShouldReturnMatchingSales()
    {
        var expectedSaleNumber = "SALE-FILTER-001";
        var otherSaleNumber = "SALE-FILTER-002";
        await CreateSaleAsync(new CreateSalePayloadBuilder().WithSaleNumber(expectedSaleNumber).Build());
        await CreateSaleAsync(new CreateSalePayloadBuilder().WithSaleNumber(otherSaleNumber).Build());

        var response = await Client.GetAsync($"/api/sales?saleNumber={expectedSaleNumber}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var apiResponse = await ReadSuccessResponseAsync<PaginatedResponse<ListSaleResult>>(response);
        
        apiResponse.TotalItems.Should().Be(1);
        apiResponse.Data.Should().ContainSingle();
        apiResponse.Data!.Single().SaleNumber.Should().Be(expectedSaleNumber);
    }

    [Fact(DisplayName = "GET /api/sales should return validation error when order is invalid")]
    public async Task Given_InvalidOrder_When_Listing_Then_ShouldReturnValidationError()
    {
        var response = await Client.GetAsync("/api/sales?_order=invalidField");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errorResponse = await ReadErrorResponseAsync(response);
        
        errorResponse.Type.Should().Be("ValidationError");
        errorResponse.Error.Should().Be("Invalid input data");
        errorResponse.Detail.Should().Contain("Order");
    }
}

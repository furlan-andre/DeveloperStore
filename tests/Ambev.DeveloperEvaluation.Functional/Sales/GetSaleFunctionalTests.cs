using System.Net;
using Ambev.DeveloperEvaluation.Functional.Common;
using Ambev.DeveloperEvaluation.Functional.Sales.TestData;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Sales;

public sealed class GetSaleFunctionalTests : FunctionalTestBase
{
    public GetSaleFunctionalTests(PostgreSqlFunctionalFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "GET /api/sales/{id} should return existing sale")]
    public async Task Given_ExistingSale_When_GettingById_Then_ShouldReturnSale()
    {
        var payload = new CreateSalePayloadBuilder().Build();
        var createdSale = await CreateSaleAsync(payload);

        var response = await Client.GetAsync($"/api/sales/{createdSale.Data!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var apiResponse = await ReadSuccessResponseAsync<ApiResponseWithData<GetSaleResult>>(response);
        
        apiResponse.Success.Should().BeTrue();
        apiResponse.Message.Should().Be("Sale retrieved successfully");
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Id.Should().Be(createdSale.Data.Id);
        apiResponse.Data.SaleNumber.Should().Be(payload.SaleNumber);
    }

    [Fact(DisplayName = "GET /api/sales/{id} should return resource not found when sale is missing")]
    public async Task Given_MissingSale_When_GettingById_Then_ShouldReturnResourceNotFound()
    {
        var saleId = Guid.NewGuid();

        var response = await Client.GetAsync($"/api/sales/{saleId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var errorResponse = await ReadErrorResponseAsync(response);
        
        errorResponse.Type.Should().Be("ResourceNotFound");
        errorResponse.Error.Should().Be("Sale not found");
        errorResponse.Detail.Should().Contain(saleId.ToString());
    }
}

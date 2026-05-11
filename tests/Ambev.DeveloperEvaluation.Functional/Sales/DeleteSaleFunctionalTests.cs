using System.Net;
using Ambev.DeveloperEvaluation.Functional.Common;
using Ambev.DeveloperEvaluation.Functional.Sales.TestData;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Sales;

public sealed class DeleteSaleFunctionalTests : FunctionalTestBase
{
    public DeleteSaleFunctionalTests(PostgreSqlFunctionalFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "DELETE /api/sales/{id} should return deleted sale")]
    public async Task Given_ExistingSale_When_Deleting_Then_ShouldReturnDeletedSale()
    {
        var createdSale = await CreateSaleAsync(new CreateSalePayloadBuilder().Build());

        var response = await Client.DeleteAsync($"/api/sales/{createdSale.Data!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var apiResponse = await ReadSuccessResponseAsync<ApiResponseWithData<DeleteSaleResult>>(response);
        
        apiResponse.Success.Should().BeTrue();
        apiResponse.Message.Should().Be("Sale deleted successfully");
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Id.Should().Be(createdSale.Data.Id);
        apiResponse.Data.Active.Should().BeFalse();
    }

    [Fact(DisplayName = "DELETE /api/sales/{id} should persist sale as inactive")]
    public async Task Given_ExistingSale_When_Deleting_Then_ShouldPersistSaleAsInactive()
    {
        var createdSale = await CreateSaleAsync(new CreateSalePayloadBuilder().Build());

        await Client.DeleteAsync($"/api/sales/{createdSale.Data!.Id}");

        var getResponse = await Client.GetAsync($"/api/sales/{createdSale.Data.Id}");
        var apiResponse = await ReadSuccessResponseAsync<ApiResponseWithData<GetSaleResult>>(getResponse);

        apiResponse.Data!.Active.Should().BeFalse();
        apiResponse.Data.Items.Should().NotBeEmpty();
    }

    [Fact(DisplayName = "DELETE /api/sales/{id} should return resource not found when sale is missing")]
    public async Task Given_MissingSale_When_Deleting_Then_ShouldReturnResourceNotFound()
    {
        var saleId = Guid.NewGuid();

        var response = await Client.DeleteAsync($"/api/sales/{saleId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var errorResponse = await ReadErrorResponseAsync(response);
        
        errorResponse.Type.Should().Be("ResourceNotFound");
        errorResponse.Error.Should().Be("Sale not found");
        errorResponse.Detail.Should().Contain(saleId.ToString());
    }
}

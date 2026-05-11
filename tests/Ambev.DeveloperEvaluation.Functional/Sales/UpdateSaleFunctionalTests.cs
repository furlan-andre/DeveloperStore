using System.Net;
using System.Net.Http.Json;
using Ambev.DeveloperEvaluation.Functional.Common;
using Ambev.DeveloperEvaluation.Functional.Sales.TestData;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSales;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Sales;

public sealed class UpdateSaleFunctionalTests : FunctionalTestBase
{
    public UpdateSaleFunctionalTests(PostgreSqlFunctionalFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "PUT /api/sales/{id} should return updated sale")]
    public async Task Given_ExistingSale_When_Updating_Then_ShouldReturnUpdatedSale()
    {
        var createPayload = new CreateSalePayloadBuilder().Build();
        var createdSale = await CreateSaleAsync(createPayload);
        var updatedSaleNumber = "SALE-UPDATED-001";
        var updatedCustomerName = "Updated Customer";
        var updatePayload = new UpdateSalePayloadBuilder()
            .FromCreatePayload(createPayload, createdSale.Data!.Items.Single().Id)
            .WithSaleNumber(updatedSaleNumber)
            .WithCustomerName(updatedCustomerName)
            .Build();

        var response = await Client.PutAsJsonAsync($"/api/sales/{createdSale.Data.Id}", updatePayload);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var apiResponse = await ReadSuccessResponseAsync<ApiResponseWithData<UpdateSaleResult>>(response);
        
        apiResponse.Success.Should().BeTrue();
        apiResponse.Message.Should().Be("Sale updated successfully");
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Id.Should().Be(createdSale.Data.Id);
        apiResponse.Data.SaleNumber.Should().Be(updatedSaleNumber);
        apiResponse.Data.CustomerName.Should().Be(updatedCustomerName);
    }

    [Fact(DisplayName = "PUT /api/sales/{id} should persist changes")]
    public async Task Given_ExistingSale_When_Updating_Then_ShouldPersistChanges()
    {
        var createPayload = new CreateSalePayloadBuilder().Build();
        var createdSale = await CreateSaleAsync(createPayload);
        var updatedBranchName = "Updated Branch";
        var updatePayload = new UpdateSalePayloadBuilder()
            .FromCreatePayload(createPayload, createdSale.Data!.Items.Single().Id)
            .WithBranchName(updatedBranchName)
            .Build();

        await Client.PutAsJsonAsync($"/api/sales/{createdSale.Data.Id}", updatePayload);

        var getResponse = await Client.GetAsync($"/api/sales/{createdSale.Data.Id}");
        var apiResponse = await ReadSuccessResponseAsync<ApiResponseWithData<GetSaleResult>>(getResponse);

        apiResponse.Data!.BranchName.Should().Be(updatedBranchName);
    }

    [Fact(DisplayName = "PUT /api/sales/{id} should return resource not found when sale is missing")]
    public async Task Given_MissingSale_When_Updating_Then_ShouldReturnResourceNotFound()
    {
        var saleId = Guid.NewGuid();
        var updatePayload = new UpdateSalePayloadBuilder().Build();

        var response = await Client.PutAsJsonAsync($"/api/sales/{saleId}", updatePayload);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var errorResponse = await ReadErrorResponseAsync(response);
        errorResponse.Type.Should().Be("ResourceNotFound");
        errorResponse.Error.Should().Be("Sale not found");
        errorResponse.Detail.Should().Contain(saleId.ToString());
    }

    [Fact(DisplayName = "PUT /api/sales/{id} should return validation error when payload is invalid")]
    public async Task Given_InvalidPayload_When_Updating_Then_ShouldReturnValidationError()
    {
        var saleId = Guid.NewGuid();
        var emptySaleNumber = string.Empty;
        var updatePayload = new UpdateSalePayloadBuilder()
            .WithSaleNumber(emptySaleNumber)
            .Build();

        var response = await Client.PutAsJsonAsync($"/api/sales/{saleId}", updatePayload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errorResponse = await ReadErrorResponseAsync(response);
        
        errorResponse.Type.Should().Be("ValidationError");
        errorResponse.Error.Should().Be("Invalid input data");
        errorResponse.Detail.Should().Contain("SaleNumber");
    }

    [Fact(DisplayName = "PUT /api/sales/{id} should persist inactive item")]
    public async Task Given_ExistingItem_When_UpdatingAsInactive_Then_ShouldPersistInactiveItem()
    {
        var firstItem = new CreateSaleItemPayloadBuilder()
            .WithQuantity(4)
            .WithUnitPrice(100m)
            .Build();
        
        var secondItem = new CreateSaleItemPayloadBuilder()
            .WithQuantity(4)
            .WithUnitPrice(100m)
            .Build();
        
        var createPayload = new CreateSalePayloadBuilder()
            .WithItems([firstItem, secondItem])
            .Build();
        
        var createdSale = await CreateSaleAsync(createPayload);

        var getResponse = await Client.GetAsync($"/api/sales/{createdSale.Data!.Id}");
        var createdState = (await ReadSuccessResponseAsync<ApiResponseWithData<GetSaleResult>>(getResponse)).Data!;
        var itemToInactivate = createdState.Items.First();
        var itemToKeep = createdState.Items.Last();

        var updateItems = new List<Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSaleItem.UpdateSaleItemInput>
        {
            new UpdateSaleItemPayloadBuilder()
                .FromGetItem(itemToInactivate)
                .WithActive(false)
                .Build(),
            
            new UpdateSaleItemPayloadBuilder()
                .FromGetItem(itemToKeep)
                .Build()
        };

        var updatePayload = new UpdateSalePayloadBuilder()
            .FromGetResult(createdState)
            .WithItems(updateItems)
            .Build();

        var response = await Client.PutAsJsonAsync($"/api/sales/{createdSale.Data.Id}", updatePayload);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var apiResponse = await ReadSuccessResponseAsync<ApiResponseWithData<UpdateSaleResult>>(response);
        
        apiResponse.Data!.Items.Single(item => item.Id == itemToInactivate.Id).Active.Should().BeFalse();
        apiResponse.Data.Items.Single(item => item.Id == itemToKeep.Id).Active.Should().BeTrue();
    }
}

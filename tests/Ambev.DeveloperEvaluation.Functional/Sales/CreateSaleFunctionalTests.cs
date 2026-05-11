using System.Net;
using System.Net.Http.Json;
using Ambev.DeveloperEvaluation.Functional.Common;
using Ambev.DeveloperEvaluation.Functional.Sales.TestData;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Sales;

public sealed class CreateSaleFunctionalTests : FunctionalTestBase
{
    public CreateSaleFunctionalTests(PostgreSqlFunctionalFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "POST /api/sales should return created when payload is valid")]
    public async Task Given_ValidPayload_When_CreatingSale_Then_ShouldReturnCreated()
    {
        var quantity = 4;
        var unitPrice = 100m;
        var expectedDiscount = 40m;
        var expectedItemTotal = 360m;
        var expectedSaleTotal = 360m;
        
        var item = new CreateSaleItemPayloadBuilder()
            .WithQuantity(quantity)
            .WithUnitPrice(unitPrice)
            .Build();
        
        var payload = new CreateSalePayloadBuilder()
            .WithItems([item])
            .Build();

        var response = await Client.PostAsJsonAsync("/api/sales", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var apiResponse = await ReadSuccessResponseAsync<ApiResponseWithData<CreateSaleResult>>(response);
        
        apiResponse.Success.Should().BeTrue();
        apiResponse.Message.Should().Be("Sale created successfully");
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Id.Should().NotBeEmpty();
        apiResponse.Data.SaleNumber.Should().Be(payload.SaleNumber);
        apiResponse.Data.TotalSaleAmount.Should().Be(expectedSaleTotal);
        apiResponse.Data.Items.Should().ContainSingle();

        var responseItem = apiResponse.Data.Items.Single();
        
        responseItem.Quantity.Should().Be(quantity);
        responseItem.UnitPrice.Should().Be(unitPrice);
        responseItem.Discount.Should().Be(expectedDiscount);
        responseItem.TotalAmount.Should().Be(expectedItemTotal);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Be($"/api/sales/{apiResponse.Data.Id}");
    }

    [Fact(DisplayName = "POST /api/sales should allow getting created sale")]
    public async Task Given_ValidPayload_When_CreatingSale_Then_ShouldAllowGettingCreatedSale()
    {
        var payload = new CreateSalePayloadBuilder().Build();

        var createdSale = await CreateSaleAsync(payload);

        var response = await Client.GetAsync($"/api/sales/{createdSale.Data!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var apiResponse = await ReadSuccessResponseAsync<ApiResponseWithData<GetSaleResult>>(response);
        
        apiResponse.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Id.Should().Be(createdSale.Data.Id);
        apiResponse.Data.SaleNumber.Should().Be(payload.SaleNumber);
        apiResponse.Data.CustomerId.Should().Be(payload.CustomerId);
        apiResponse.Data.CustomerName.Should().Be(payload.CustomerName);
        apiResponse.Data.BranchId.Should().Be(payload.BranchId);
        apiResponse.Data.BranchName.Should().Be(payload.BranchName);
        apiResponse.Data.Active.Should().BeTrue();
        apiResponse.Data.Items.Should().ContainSingle();
    }

    [Fact(DisplayName = "POST /api/sales should return validation error when payload is invalid")]
    public async Task Given_InvalidPayload_When_CreatingSale_Then_ShouldReturnValidationError()
    {
        var emptySaleNumber = string.Empty;
        var payload = new CreateSalePayloadBuilder()
            .WithSaleNumber(emptySaleNumber)
            .Build();

        var response = await Client.PostAsJsonAsync("/api/sales", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errorResponse = await ReadErrorResponseAsync(response);
        errorResponse.Type.Should().Be("ValidationError");
        errorResponse.Error.Should().Be("Invalid input data");
        errorResponse.Detail.Should().Contain(nameof(CreateSaleResult.SaleNumber));
    }

    [Fact(DisplayName = "POST /api/sales should return domain rule violation when domain rejects payload")]
    public async Task Given_DomainInvalidPayload_When_CreatingSale_Then_ShouldReturnDomainRuleViolation()
    {
        var excessiveQuantity = 21;
        
        var item = new CreateSaleItemPayloadBuilder()
            .WithQuantity(excessiveQuantity)
            .Build();
        
        var payload = new CreateSalePayloadBuilder()
            .WithItems([item])
            .Build();

        var response = await Client.PostAsJsonAsync("/api/sales", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errorResponse = await ReadErrorResponseAsync(response);
        
        errorResponse.Type.Should().Be("DomainRuleViolation");
        errorResponse.Error.Should().Be("Sale domain rule violated");
        errorResponse.Detail.Should().NotBeNullOrWhiteSpace();
    }
}

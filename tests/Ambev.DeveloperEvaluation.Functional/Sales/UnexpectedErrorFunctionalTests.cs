using System.Net;
using Ambev.DeveloperEvaluation.Application.Common.Pagination;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Application.Sales.Service;
using Ambev.DeveloperEvaluation.Common.Results;
using Ambev.DeveloperEvaluation.Functional.Common;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Sales;

public sealed class UnexpectedErrorFunctionalTests : IAsyncLifetime
{
    private readonly PostgreSqlFunctionalFixture _fixture;
    private CustomWebApplicationFactory? _factory;
    private HttpClient? _client;

    public UnexpectedErrorFunctionalTests()
    {
        _fixture = new PostgreSqlFunctionalFixture();
    }

    public async Task InitializeAsync()
    {
        await _fixture.InitializeAsync();
        await _fixture.ResetDatabaseAsync();

        _factory = new CustomWebApplicationFactory(
            _fixture.ConnectionString,
            services =>
            {
                services.RemoveAll<IListSalesService>();
                services.AddScoped<IListSalesService, ThrowingListSalesService>();
            });

        _client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();

        if (_factory is not null)
            await _factory.DisposeAsync();

        await _fixture.DisposeAsync();
    }

    [Fact(DisplayName = "Should return unexpected error when unhandled exception occurs")]
    public async Task Given_UnhandledException_When_RequestingEndpoint_Then_ShouldReturnUnexpectedError()
    {
        var response = await _client!.GetAsync("/api/sales");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var errorResponse = await FunctionalTestBase.ReadErrorResponseAsync(response);
        
        errorResponse.Type.Should().Be("UnexpectedError");
        errorResponse.Error.Should().Be("Unexpected error");
        errorResponse.Detail.Should().Be("An unexpected error occurred while processing the request.");
    }

    private sealed class ThrowingListSalesService : IListSalesService
    {
        public Task<Result<PagedResponse<ListSaleResponse>>> ListAsync(
            ListSalesRequest request,
            CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Forced functional test exception.");
        }
    }
}

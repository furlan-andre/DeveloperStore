using System.Net.Http.Json;
using System.Text.Json;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Common.Errors;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSales;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Common;

[Collection(nameof(FunctionalTestCollection))]
public abstract class FunctionalTestBase : IAsyncLifetime
{
    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly PostgreSqlFunctionalFixture _fixture;
    private readonly CustomWebApplicationFactory _factory;

    protected FunctionalTestBase(PostgreSqlFunctionalFixture fixture)
    {
        _fixture = fixture;
        _factory = CreateFactory();
        Client = _factory.CreateClient();
    }

    protected HttpClient Client { get; }

    public virtual async Task InitializeAsync()
    {
        await _fixture.ResetDatabaseAsync();
    }

    public virtual async Task DisposeAsync()
    {
        Client.Dispose();
        await _factory.DisposeAsync();
    }

    protected virtual CustomWebApplicationFactory CreateFactory()
    {
        return new CustomWebApplicationFactory(_fixture.ConnectionString);
    }

    protected async Task<ApiResponseWithData<CreateSaleResult>> CreateSaleAsync(object payload)
    {
        var response = await Client.PostAsJsonAsync("/api/sales", payload);
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResult>>(JsonOptions))!;
    }

    protected static async Task<T> ReadSuccessResponseAsync<T>(HttpResponseMessage response)
    {
        return (await response.Content.ReadFromJsonAsync<T>(JsonOptions))!;
    }

    public static async Task<ErrorResponse> ReadErrorResponseAsync(HttpResponseMessage response)
    {
        return (await response.Content.ReadFromJsonAsync<ErrorResponse>(JsonOptions))!;
    }
}

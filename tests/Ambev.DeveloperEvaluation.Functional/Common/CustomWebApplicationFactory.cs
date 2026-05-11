using Ambev.DeveloperEvaluation.Application.Messaging;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ambev.DeveloperEvaluation.Functional.Common;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    private readonly Action<IServiceCollection>? _configureServices;

    public CustomWebApplicationFactory(
        string connectionString,
        Action<IServiceCollection>? configureServices = null)
    {
        _connectionString = connectionString;
        _configureServices = configureServices;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("FunctionalTesting");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<DefaultContext>>();
            services.AddDbContext<DefaultContext>(options =>
                options.UseNpgsql(
                    _connectionString,
                    database => database.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM")));

            services.RemoveAll<ISalesEventPublisher>();
            services.AddScoped<ISalesEventPublisher, NoOpSalesEventPublisher>();

            _configureServices?.Invoke(services);
        });
    }
}

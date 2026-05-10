using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.Service;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSaleItem;
using Ambev.DeveloperEvaluation.Domain.Services.Sales;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class ApplicationModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        builder.Services.AddScoped<ISaleDiscountPolicy, SaleDiscountPolicy>();
        builder.Services.AddScoped<ICreateSaleService, CreateSaleService>();
        builder.Services.AddScoped<IUpdateSaleService, UpdateSaleService>();
        builder.Services.AddScoped<IDeleteSaleService, DeleteSaleService>();
        builder.Services.AddScoped<IValidator<CreateSaleCommand>, CreateSaleCommandValidator>();
        builder.Services.AddScoped<IValidator<CreateSaleItemCommand>, CreateSaleItemCommandValidator>();
        builder.Services.AddScoped<IValidator<UpdateSaleCommand>, UpdateSaleCommandValidator>();
        builder.Services.AddScoped<IValidator<UpdateSaleItemCommand>, UpdateSaleItemCommandValidator>();
        builder.Services.AddScoped<IValidator<DeleteSaleCommand>, DeleteSaleCommandValidator>();
    }
}

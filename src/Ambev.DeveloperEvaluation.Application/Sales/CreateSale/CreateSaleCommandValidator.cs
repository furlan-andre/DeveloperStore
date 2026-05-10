using Ambev.DeveloperEvaluation.Application.Sales.CreateSaleItem;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(sale => sale.SaleNumber).NotEmpty();
        RuleFor(sale => sale.SaleDate).NotEqual(default(DateTime));
        RuleFor(sale => sale.CustomerId).NotEmpty();
        RuleFor(sale => sale.CustomerName).NotEmpty();
        RuleFor(sale => sale.BranchId).NotEmpty();
        RuleFor(sale => sale.BranchName).NotEmpty();
        RuleFor(sale => sale.Items).NotNull().NotEmpty();
        RuleForEach(sale => sale.Items).SetValidator(new CreateSaleItemCommandValidator());
    }
}

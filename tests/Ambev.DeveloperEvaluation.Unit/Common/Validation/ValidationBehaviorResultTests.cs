using Ambev.DeveloperEvaluation.Application.Common.Pagination;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Common.Results;
using Ambev.DeveloperEvaluation.Common.Validation;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Common.Validation;

public class ValidationBehaviorResultTests
{
    [Fact(DisplayName = "Should return validation failure result when response is Result")]
    public async Task Given_InvalidRequestWithResultResponse_When_Handling_Then_ShouldReturnValidationFailure()
    {
        var command = new ListSalesCommand { Page = 0, Size = 10 };
        var validators = new IValidator<ListSalesCommand>[] { new ListSalesCommandValidator() };
        var behavior = new ValidationBehavior<ListSalesCommand, Result<PagedResponse<ListSaleResponse>>>(validators);
        var nextWasCalled = false;

        RequestHandlerDelegate<Result<PagedResponse<ListSaleResponse>>> next = () =>
        {
            nextWasCalled = true;
            return Task.FromResult(Result<PagedResponse<ListSaleResponse>>.Success(new PagedResponse<ListSaleResponse>()));
        };

        var result = await behavior.Handle(command, next, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be("ValidationError");
        result.Error.ErrorMessage.Should().Be("Invalid input data");
        result.Error.Detail.Should().Contain(nameof(ListSalesCommand.Page));
        nextWasCalled.Should().BeFalse();
    }
}

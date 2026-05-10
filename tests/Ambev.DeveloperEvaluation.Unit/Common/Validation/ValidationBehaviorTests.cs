using Ambev.DeveloperEvaluation.Common.Validation;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Common.Validation;

public class ValidationBehaviorTests
{
    [Fact(DisplayName = "Should call next when no validators are registered")]
    public async Task Given_NoValidators_When_HandlingRequest_Then_ShouldCallNext()
    {
        var request = new TestRequest { Name = string.Empty };
        var expectedResponse = "success";
        var behavior = new ValidationBehavior<TestRequest, string>([]);

        var response = await behavior.Handle(
            request,
            () => Task.FromResult(expectedResponse),
            CancellationToken.None);

        response.Should().Be(expectedResponse);
    }

    [Fact(DisplayName = "Should call next when request is valid")]
    public async Task Given_ValidRequest_When_HandlingRequest_Then_ShouldCallNext()
    {
        var request = new TestRequest { Name = "valid" };
        var expectedResponse = "success";
        var validators = new IValidator<TestRequest>[] { new TestRequestValidator() };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);

        var response = await behavior.Handle(
            request,
            () => Task.FromResult(expectedResponse),
            CancellationToken.None);

        response.Should().Be(expectedResponse);
    }

    [Fact(DisplayName = "Should throw ValidationException when request is invalid")]
    public async Task Given_InvalidRequest_When_HandlingRequest_Then_ShouldThrowValidationException()
    {
        var request = new TestRequest { Name = string.Empty };
        var validators = new IValidator<TestRequest>[] { new TestRequestValidator() };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);

        var action = async () => await behavior.Handle(
            request,
            () => Task.FromResult("success"),
            CancellationToken.None);

        await action.Should().ThrowAsync<ValidationException>();
    }

    private sealed record TestRequest : IRequest<string>
    {
        public string Name { get; init; } = string.Empty;
    }

    private sealed class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(request => request.Name).NotEmpty();
        }
    }
}

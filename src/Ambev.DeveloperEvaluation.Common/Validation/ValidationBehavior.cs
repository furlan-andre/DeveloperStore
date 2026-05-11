using Ambev.DeveloperEvaluation.Common.Results;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Common.Validation;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(result => result.Errors)
                .Where(failure => failure != null)
                .ToList();

            if (failures.Count != 0)
            {
                if (IsResultResponse())
                    return CreateValidationFailure(failures);

                throw new ValidationException(failures);
            }
        }

        return await next();
    }

    private static bool IsResultResponse()
    {
        var responseType = typeof(TResponse);
        return responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>);
    }

    private static TResponse CreateValidationFailure(
        IReadOnlyCollection<FluentValidation.Results.ValidationFailure> failures)
    {
        var detail = string.Join(
            "; ",
            failures.Select(failure => $"{failure.PropertyName}: {failure.ErrorMessage}"));

        var error = Error.Validation("Invalid input data", detail);
        var responseType = typeof(TResponse);
        var valueType = responseType.GetGenericArguments()[0];
        var resultType = typeof(Result<>).MakeGenericType(valueType);
        var failureMethod = resultType.GetMethod(nameof(Result<object>.Failure), [typeof(Error)]);

        return (TResponse)failureMethod!.Invoke(null, [error])!;
    }
}

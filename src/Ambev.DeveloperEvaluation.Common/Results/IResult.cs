namespace Ambev.DeveloperEvaluation.Common.Results;

public interface IResult
{
    bool IsSuccess { get; }
    bool IsFailure { get; }
    Error Error { get; }
}

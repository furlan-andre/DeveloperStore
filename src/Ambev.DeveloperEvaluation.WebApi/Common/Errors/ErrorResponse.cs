namespace Ambev.DeveloperEvaluation.WebApi.Common.Errors;

public sealed class ErrorResponse
{
    public string Type { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
}

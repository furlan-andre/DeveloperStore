namespace Ambev.DeveloperEvaluation.WebApi.Common;

public sealed class QueryParameters
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Order { get; set; }
    public IReadOnlyDictionary<string, string?> Filters { get; set; } = new Dictionary<string, string?>();
}

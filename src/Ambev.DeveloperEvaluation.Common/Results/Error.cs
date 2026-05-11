namespace Ambev.DeveloperEvaluation.Common.Results;

public sealed record Error(string Type, string ErrorMessage, string Detail)
{
    public static Error Validation(string errorMessage, string detail)
    {
        return new Error("ValidationError", errorMessage, detail);
    }

    public static Error ResourceNotFound(string errorMessage, string detail)
    {
        return new Error("ResourceNotFound", errorMessage, detail);
    }

    public static Error DomainRuleViolation(string errorMessage, string detail)
    {
        return new Error("DomainRuleViolation", errorMessage, detail);
    }

    public static Error Conflict(string errorMessage, string detail)
    {
        return new Error("Conflict", errorMessage, detail);
    }

    public static Error Unexpected(string errorMessage, string detail)
    {
        return new Error("UnexpectedError", errorMessage, detail);
    }
}

namespace TodoApp.Domain.Common;

public enum ErrorType
{
    Validation,
    NotFound
}

/// <summary>
/// Describes an expected failure (as opposed to an exceptional one) in a way that
/// outer layers can translate, e.g. into an HTTP status code.
/// </summary>
public sealed record Error(string Code, string Message, ErrorType Type)
{
    public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);

    public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);
}

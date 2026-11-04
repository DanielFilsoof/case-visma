using Microsoft.AspNetCore.Http.HttpResults;
using TodoApp.Domain.Common;

namespace TodoApp.Api.Common;

/// <summary>
/// Translates application errors into ProblemDetails responses (ASP.NET Core's standard error format).
/// The only place that knows how an <see cref="ErrorType"/> maps to an HTTP status code.
/// </summary>
public static class ErrorResults
{
    public static ProblemHttpResult ToProblem(this Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        return TypedResults.Problem(
            statusCode: statusCode,
            title: error.Code,
            detail: error.Message);
    }
}

using Microsoft.AspNetCore.Http.HttpResults;
using TodoApp.Api.Common;
using TodoApp.Application.Todos;

namespace TodoApp.Api.Endpoints;

public static class TodoEndpoints
{
    private const string GetTodoByIdRouteName = "GetTodoById";

    public static IEndpointRouteBuilder MapTodoEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/todos").WithTags("Todos");

        group.MapGet("/", GetAll)
            .WithSummary("List all todos.");

        group.MapGet("/{id:guid}", GetById)
            .WithName(GetTodoByIdRouteName)
            .WithSummary("Get a single todo.");

        group.MapPost("/", Create)
            .WithSummary("Create a new todo.");

        group.MapPatch("/{id:guid}/complete", Complete)
            .WithSummary("Mark a todo as done.");

        group.MapPatch("/{id:guid}/reopen", Reopen)
            .WithSummary("Mark a todo as not done.");

        group.MapDelete("/{id:guid}", Delete)
            .WithSummary("Delete a todo.");

        return routes;
    }

    private static async Task<Ok<IReadOnlyList<TodoDto>>> GetAll(
        ITodoService service,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(await service.GetAllAsync(cancellationToken));

    private static async Task<Results<Ok<TodoDto>, ProblemHttpResult>> GetById(
        Guid id,
        ITodoService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.Value) : result.Error!.ToProblem();
    }

    private static async Task<Results<CreatedAtRoute<TodoDto>, ProblemHttpResult>> Create(
        CreateTodoRequest request,
        ITodoService service,
        CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);

        return result.IsSuccess
            ? TypedResults.CreatedAtRoute(result.Value, GetTodoByIdRouteName, new { id = result.Value.Id })
            : result.Error!.ToProblem();
    }

    private static async Task<Results<Ok<TodoDto>, ProblemHttpResult>> Complete(
        Guid id,
        ITodoService service,
        CancellationToken cancellationToken)
    {
        var result = await service.CompleteAsync(id, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.Value) : result.Error!.ToProblem();
    }

    private static async Task<Results<Ok<TodoDto>, ProblemHttpResult>> Reopen(
        Guid id,
        ITodoService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ReopenAsync(id, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.Value) : result.Error!.ToProblem();
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> Delete(
        Guid id,
        ITodoService service,
        CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);

        return result.IsSuccess ? TypedResults.NoContent() : result.Error!.ToProblem();
    }
}

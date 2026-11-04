using TodoApp.Domain.Todos;

namespace TodoApp.Application.Todos;

public sealed record TodoDto(
    Guid Id,
    string Title,
    bool IsCompleted,
    DateTimeOffset CreatedAt,
    DateTimeOffset? CompletedAt)
{
    public static TodoDto FromDomain(TodoItem todo) =>
        new(todo.Id, todo.Title, todo.IsCompleted, todo.CreatedAt, todo.CompletedAt);
}

public sealed record CreateTodoRequest(string? Title);

using TodoApp.Application.Todos;
using TodoApp.Domain.Todos;

namespace TodoApp.Application.Tests;

/// <summary>
/// Substitute for the real repository letting the service be tested without EF Core.
/// </summary>
internal sealed class InMemoryTodoRepository : ITodoRepository
{
    private readonly Dictionary<Guid, TodoItem> _todos = [];

    public IReadOnlyCollection<TodoItem> Items => _todos.Values;

    public Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<TodoItem>>(_todos.Values.ToList());

    public Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_todos.GetValueOrDefault(id));

    public Task AddAsync(TodoItem todo, CancellationToken cancellationToken = default)
    {
        _todos.Add(todo.Id, todo);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TodoItem todo, CancellationToken cancellationToken = default)
    {
        _todos[todo.Id] = todo;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TodoItem todo, CancellationToken cancellationToken = default)
    {
        _todos.Remove(todo.Id);
        return Task.CompletedTask;
    }
}

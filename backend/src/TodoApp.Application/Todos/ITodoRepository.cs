using TodoApp.Domain.Todos;

namespace TodoApp.Application.Todos;

public interface ITodoRepository
{
    Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(TodoItem todo, CancellationToken cancellationToken = default);

    Task UpdateAsync(TodoItem todo, CancellationToken cancellationToken = default);

    Task DeleteAsync(TodoItem todo, CancellationToken cancellationToken = default);
}

using TodoApp.Domain.Common;

namespace TodoApp.Application.Todos;

public interface ITodoService
{
    Task<IReadOnlyList<TodoDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Result<TodoDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<TodoDto>> CreateAsync(CreateTodoRequest request, CancellationToken cancellationToken = default);

    Task<Result<TodoDto>> CompleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<TodoDto>> ReopenAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

using TodoApp.Domain.Common;
using TodoApp.Domain.Todos;

namespace TodoApp.Application.Todos;

public sealed class TodoService(ITodoRepository repository, TimeProvider timeProvider) : ITodoService
{
    public async Task<IReadOnlyList<TodoDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var todos = await repository.GetAllAsync(cancellationToken);

        return todos
            .OrderBy(todo => todo.CreatedAt)
            .Select(TodoDto.FromDomain)
            .ToList();
    }

    public async Task<Result<TodoDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var todo = await repository.GetByIdAsync(id, cancellationToken);

        return todo is null ? TodoErrors.NotFound(id) : TodoDto.FromDomain(todo);
    }

    public async Task<Result<TodoDto>> CreateAsync(CreateTodoRequest request, CancellationToken cancellationToken = default)
    {
        var result = TodoItem.Create(request.Title, timeProvider.GetUtcNow());

        if (!result.IsSuccess)
        {
            return result.Error!;
        }

        await repository.AddAsync(result.Value, cancellationToken);

        return TodoDto.FromDomain(result.Value);
    }

    public Task<Result<TodoDto>> CompleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        UpdateAsync(id, todo => todo.Complete(timeProvider.GetUtcNow()), cancellationToken);

    public Task<Result<TodoDto>> ReopenAsync(Guid id, CancellationToken cancellationToken = default) =>
        UpdateAsync(id, todo => todo.Reopen(), cancellationToken);

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var todo = await repository.GetByIdAsync(id, cancellationToken);

        if (todo is null)
        {
            return TodoErrors.NotFound(id);
        }

        await repository.DeleteAsync(todo, cancellationToken);

        return Result.Success();
    }

    private async Task<Result<TodoDto>> UpdateAsync(
        Guid id,
        Action<TodoItem> change,
        CancellationToken cancellationToken)
    {
        var todo = await repository.GetByIdAsync(id, cancellationToken);

        if (todo is null)
        {
            return TodoErrors.NotFound(id);
        }

        change(todo);
        await repository.UpdateAsync(todo, cancellationToken);

        return TodoDto.FromDomain(todo);
    }
}

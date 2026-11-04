using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Todos;
using TodoApp.Domain.Todos;

namespace TodoApp.Infrastructure.Persistence;

internal sealed class EfTodoRepository(TodoDbContext dbContext) : ITodoRepository
{
    public async Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Todos.AsNoTracking().ToListAsync(cancellationToken);

    public Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Todos.FirstOrDefaultAsync(todo => todo.Id == id, cancellationToken);

    public async Task AddAsync(TodoItem todo, CancellationToken cancellationToken = default)
    {
        dbContext.Todos.Add(todo);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TodoItem todo, CancellationToken cancellationToken = default)
    {
        dbContext.Todos.Update(todo);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TodoItem todo, CancellationToken cancellationToken = default)
    {
        dbContext.Todos.Remove(todo);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

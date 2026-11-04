using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Todos;

namespace TodoApp.Infrastructure.Persistence;

public sealed class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> Todos => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TodoDbContext).Assembly);
}

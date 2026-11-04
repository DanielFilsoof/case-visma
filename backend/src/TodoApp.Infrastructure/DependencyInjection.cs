using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Application.Todos;
using TodoApp.Infrastructure.Persistence;

namespace TodoApp.Infrastructure;

public static class DependencyInjection
{
    private const string InMemoryDatabaseName = "TodoDb";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<TodoDbContext>(options => options.UseInMemoryDatabase(InMemoryDatabaseName));

        services.AddScoped<ITodoRepository, EfTodoRepository>();

        return services;
    }
}

using Microsoft.Extensions.DependencyInjection;
using TodoApp.Application.Todos;

namespace TodoApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<ITodoService, TodoService>();

        return services;
    }
}

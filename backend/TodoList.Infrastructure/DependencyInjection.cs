using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoList.Application.Services;
using TodoList.Domain.Interfaces;
using TodoList.Infrastructure.Data;
using TodoList.Infrastructure.Repositories;

namespace TodoList.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        bool useInMemoryDatabase = false,
        string inMemoryDatabaseName = "TodoListTestDb")
    {
        if (useInMemoryDatabase)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(inMemoryDatabaseName));
        }
        else
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "ConnectionStrings:DefaultConnection no está configurada. Defina la variable de entorno ConnectionStrings__DefaultConnection.");
            }

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        services.AddScoped<ITodoRepository, TodoRepository>();
        services.AddScoped<TodoService>();

        return services;
    }
}

using TodoList.Application.DTOs;
using TodoList.Application.Services;
using TodoList.Infrastructure;
using TodoList.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

var isTesting = builder.Environment.EnvironmentName == "Testing";
var inMemoryDbName = builder.Configuration["Testing:DatabaseName"] ?? "TodoListTestDb";

builder.Services.AddInfrastructure(builder.Configuration, isTesting, inMemoryDbName);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseCors();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

var todosGroup = app.MapGroup("/api/todos");

todosGroup.MapGet("/", async (TodoService service, CancellationToken ct) =>
    Results.Ok(await service.GetAllAsync(ct)));

todosGroup.MapGet("/{id:int}", async (int id, TodoService service, CancellationToken ct) =>
{
    var item = await service.GetByIdAsync(id, ct);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

todosGroup.MapPost("/", async (CreateTodoDto dto, TodoService service, CancellationToken ct) =>
{
    try
    {
        var created = await service.CreateAsync(dto, ct);
        return Results.Created($"/api/todos/{created.Id}", created);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

todosGroup.MapPut("/{id:int}", async (int id, UpdateTodoDto dto, TodoService service, CancellationToken ct) =>
{
    try
    {
        var updated = await service.UpdateAsync(id, dto, ct);
        return updated is null ? Results.NotFound() : Results.Ok(updated);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

todosGroup.MapDelete("/{id:int}", async (int id, TodoService service, CancellationToken ct) =>
{
    var deleted = await service.DeleteAsync(id, ct);
    return deleted ? Results.NoContent() : Results.NotFound();
});

app.Run();

public partial class Program;

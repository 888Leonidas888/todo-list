using Microsoft.EntityFrameworkCore;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;
using TodoList.Infrastructure.Data;

namespace TodoList.Infrastructure.Repositories;

public class TodoRepository(AppDbContext dbContext) : ITodoRepository
{
    public async Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Todos
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<TodoItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await dbContext.Todos.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<TodoItem> AddAsync(TodoItem item, CancellationToken cancellationToken = default)
    {
        dbContext.Todos.Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task<TodoItem> UpdateAsync(TodoItem item, CancellationToken cancellationToken = default)
    {
        dbContext.Todos.Update(item);
        await dbContext.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Todos.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        dbContext.Todos.Remove(existing);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}

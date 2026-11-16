using TodoList.Application.DTOs;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;

namespace TodoList.Application.Services;

public class TodoService(ITodoRepository repository)
{
    public async Task<IReadOnlyList<TodoDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await repository.GetAllAsync(cancellationToken);
        return items.Select(MapToDto).ToList();
    }

    public async Task<TodoDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await repository.GetByIdAsync(id, cancellationToken);
        return item is null ? null : MapToDto(item);
    }

    public async Task<TodoDto> CreateAsync(CreateTodoDto dto, CancellationToken cancellationToken = default)
    {
        ValidateTitle(dto.Title);

        var entity = new TodoItem
        {
            Title = dto.Title.Trim(),
            Description = NormalizeDescription(dto.Description),
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        var created = await repository.AddAsync(entity, cancellationToken);
        return MapToDto(created);
    }

    public async Task<TodoDto?> UpdateAsync(int id, UpdateTodoDto dto, CancellationToken cancellationToken = default)
    {
        ValidateTitle(dto.Title);

        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Title = dto.Title.Trim();
        existing.Description = NormalizeDescription(dto.Description);
        existing.IsCompleted = dto.IsCompleted;

        var updated = await repository.UpdateAsync(existing, cancellationToken);
        return MapToDto(updated);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        repository.DeleteAsync(id, cancellationToken);

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("El título de la tarea es obligatorio.", nameof(title));
        }
    }

    private static string? NormalizeDescription(string? description) =>
        string.IsNullOrWhiteSpace(description) ? null : description.Trim();

    private static TodoDto MapToDto(TodoItem item) =>
        new(item.Id, item.Title, item.Description, item.IsCompleted, item.CreatedAt);
}

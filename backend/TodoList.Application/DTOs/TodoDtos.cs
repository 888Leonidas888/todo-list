namespace TodoList.Application.DTOs;

public record TodoDto(
    int Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime CreatedAt);

public record CreateTodoDto(
    string Title,
    string? Description);

public record UpdateTodoDto(
    string Title,
    string? Description,
    bool IsCompleted);

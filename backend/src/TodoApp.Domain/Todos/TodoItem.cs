using TodoApp.Domain.Common;

namespace TodoApp.Domain.Todos;

public sealed class TodoItem
{
    public const int TitleMaxLength = 200;

    private TodoItem(Guid id, string title, DateTimeOffset createdAt)
    {
        Id = id;
        Title = title;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; }

    public bool IsCompleted => CompletedAt is not null;

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public static Result<TodoItem> Create(string? title, DateTimeOffset now)
    {
        var normalizedTitle = title?.Trim();

        if (string.IsNullOrEmpty(normalizedTitle))
        {
            return TodoErrors.TitleRequired;
        }

        if (normalizedTitle.Length > TitleMaxLength)
        {
            return TodoErrors.TitleTooLong;
        }

        return new TodoItem(Guid.NewGuid(), normalizedTitle, now);
    }

    /// <summary>Marks the item as done. Idempotent: completing twice keeps the original timestamp.</summary>
    public void Complete(DateTimeOffset now)
    {
        if (IsCompleted)
        {
            return;
        }

        CompletedAt = now;
    }

    /// <summary>Marks the item as not done.</summary>
    public void Reopen() => CompletedAt = null;
}

using TodoApp.Domain.Todos;

namespace TodoApp.Domain.Tests;

public class TodoItemTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 3, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithValidTitle_ReturnsActiveTodo()
    {
        var result = TodoItem.Create("Buy milk", Now);

        Assert.True(result.IsSuccess);
        Assert.Equal("Buy milk", result.Value.Title);
        Assert.False(result.Value.IsCompleted);
        Assert.Equal(Now, result.Value.CreatedAt);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
    }

    [Fact]
    public void Create_TrimsTitle()
    {
        var result = TodoItem.Create("  Buy milk  ", Now);

        Assert.Equal("Buy milk", result.Value.Title);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithMissingTitle_FailsWithTitleRequired(string? title)
    {
        var result = TodoItem.Create(title, Now);

        Assert.False(result.IsSuccess);
        Assert.Equal(TodoErrors.TitleRequired, result.Error);
    }

    [Fact]
    public void Create_WithTooLongTitle_FailsWithTitleTooLong()
    {
        var title = new string('a', TodoItem.TitleMaxLength + 1);

        var result = TodoItem.Create(title, Now);

        Assert.Equal(TodoErrors.TitleTooLong, result.Error);
    }

    [Fact]
    public void Create_WithTitleAtMaxLength_Succeeds()
    {
        var title = new string('a', TodoItem.TitleMaxLength);

        Assert.True(TodoItem.Create(title, Now).IsSuccess);
    }

    [Fact]
    public void Complete_MarksTodoAsCompleted()
    {
        var todo = TodoItem.Create("Buy milk", Now).Value;

        todo.Complete(Now.AddHours(1));

        Assert.True(todo.IsCompleted);
        Assert.Equal(Now.AddHours(1), todo.CompletedAt);
    }

    [Fact]
    public void Complete_WhenAlreadyCompleted_KeepsOriginalTimestamp()
    {
        var todo = TodoItem.Create("Buy milk", Now).Value;
        todo.Complete(Now.AddHours(1));

        todo.Complete(Now.AddHours(2));

        Assert.Equal(Now.AddHours(1), todo.CompletedAt);
    }

    [Fact]
    public void Reopen_MarksTodoAsActive()
    {
        var todo = TodoItem.Create("Buy milk", Now).Value;
        todo.Complete(Now);

        todo.Reopen();

        Assert.False(todo.IsCompleted);
        Assert.Null(todo.CompletedAt);
    }
}

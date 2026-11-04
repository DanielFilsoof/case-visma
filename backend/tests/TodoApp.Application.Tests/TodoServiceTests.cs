using Microsoft.Extensions.Time.Testing;
using TodoApp.Application.Todos;
using TodoApp.Domain.Common;
using TodoApp.Domain.Todos;

namespace TodoApp.Application.Tests;

public class TodoServiceTests
{
    private readonly InMemoryTodoRepository _repository = new();
    private readonly FakeTimeProvider _timeProvider = new(new DateTimeOffset(2026, 11, 3, 12, 0, 0, TimeSpan.Zero));
    private readonly TodoService _sut;

    public TodoServiceTests() => _sut = new TodoService(_repository, _timeProvider);

    [Fact]
    public async Task CreateAsync_WithValidTitle_PersistsAndReturnsTodo()
    {
        var result = await _sut.CreateAsync(new CreateTodoRequest("Buy milk"));

        Assert.True(result.IsSuccess);
        Assert.Equal("Buy milk", result.Value.Title);
        Assert.Equal(_timeProvider.GetUtcNow(), result.Value.CreatedAt);
        Assert.Single(_repository.Items);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidTitle_ReturnsValidationErrorAndPersistsNothing()
    {
        var result = await _sut.CreateAsync(new CreateTodoRequest(""));

        Assert.Equal(ErrorType.Validation, result.Error!.Type);
        Assert.Empty(_repository.Items);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsTodosOrderedByCreation()
    {
        await _sut.CreateAsync(new CreateTodoRequest("First"));
        _timeProvider.Advance(TimeSpan.FromMinutes(1));
        await _sut.CreateAsync(new CreateTodoRequest("Second"));

        var todos = await _sut.GetAllAsync();

        Assert.Equal(["First", "Second"], todos.Select(todo => todo.Title));
    }

    [Fact]
    public async Task GetByIdAsync_WhenMissing_ReturnsNotFound()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
    }

    [Fact]
    public async Task CompleteAsync_MarksTodoAsCompleted()
    {
        var created = await _sut.CreateAsync(new CreateTodoRequest("Buy milk"));

        var result = await _sut.CompleteAsync(created.Value.Id);

        Assert.True(result.Value.IsCompleted);
        Assert.Equal(_timeProvider.GetUtcNow(), result.Value.CompletedAt);
    }

    [Fact]
    public async Task ReopenAsync_MarksTodoAsActive()
    {
        var created = await _sut.CreateAsync(new CreateTodoRequest("Buy milk"));
        await _sut.CompleteAsync(created.Value.Id);

        var result = await _sut.ReopenAsync(created.Value.Id);

        Assert.False(result.Value.IsCompleted);
    }

    [Fact]
    public async Task CompleteAsync_WhenMissing_ReturnsNotFound()
    {
        var id = Guid.NewGuid();

        var result = await _sut.CompleteAsync(id);

        Assert.Equal(TodoErrors.NotFound(id), result.Error);
    }

    [Fact]
    public async Task DeleteAsync_RemovesTodo()
    {
        var created = await _sut.CreateAsync(new CreateTodoRequest("Buy milk"));

        var result = await _sut.DeleteAsync(created.Value.Id);

        Assert.True(result.IsSuccess);
        Assert.Empty(_repository.Items);
    }

    [Fact]
    public async Task DeleteAsync_WhenMissing_ReturnsNotFound()
    {
        var result = await _sut.DeleteAsync(Guid.NewGuid());

        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
    }
}

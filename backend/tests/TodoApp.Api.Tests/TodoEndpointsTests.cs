using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using TodoApp.Application.Todos;

namespace TodoApp.Api.Tests;

public class TodoEndpointsTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private const string BaseUrl = "/api/todos";

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Post_WithValidTitle_Returns201WithLocationAndBody()
    {
        var response = await _client.PostAsJsonAsync(BaseUrl, new CreateTodoRequest("Buy milk"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var todo = await response.Content.ReadFromJsonAsync<TodoDto>();
        Assert.Equal("Buy milk", todo!.Title);
        Assert.False(todo.IsCompleted);
        Assert.EndsWith($"{BaseUrl}/{todo.Id}", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task Post_WithEmptyTitle_Returns400ProblemDetails()
    {
        var response = await _client.PostAsJsonAsync(BaseUrl, new CreateTodoRequest(""));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("Todo.TitleRequired", problem!.Title);
    }

    [Fact]
    public async Task GetAll_ContainsCreatedTodo()
    {
        var created = await CreateTodoAsync("Listed");

        var todos = await _client.GetFromJsonAsync<List<TodoDto>>(BaseUrl);

        Assert.Contains(todos!, todo => todo.Id == created.Id);
    }

    [Fact]
    public async Task GetById_WhenExists_Returns200()
    {
        var created = await CreateTodoAsync("Find me");

        var todo = await _client.GetFromJsonAsync<TodoDto>($"{BaseUrl}/{created.Id}");

        Assert.Equal(created, todo);
    }

    [Fact]
    public async Task GetById_WhenMissing_Returns404()
    {
        var response = await _client.GetAsync($"{BaseUrl}/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Complete_Returns200AndPersistsCompletedState()
    {
        var created = await CreateTodoAsync("Finish me");

        var response = await _client.PatchAsync($"{BaseUrl}/{created.Id}/complete", content: null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var todo = await _client.GetFromJsonAsync<TodoDto>($"{BaseUrl}/{created.Id}");
        Assert.True(todo!.IsCompleted);
    }

    [Fact]
    public async Task Reopen_AfterComplete_ReturnsActiveTodo()
    {
        var created = await CreateTodoAsync("Reopen me");
        await _client.PatchAsync($"{BaseUrl}/{created.Id}/complete", content: null);

        var response = await _client.PatchAsync($"{BaseUrl}/{created.Id}/reopen", content: null);

        var todo = await response.Content.ReadFromJsonAsync<TodoDto>();
        Assert.False(todo!.IsCompleted);
    }

    [Fact]
    public async Task Complete_WhenMissing_Returns404()
    {
        var response = await _client.PatchAsync($"{BaseUrl}/{Guid.NewGuid()}/complete", content: null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenExists_Returns204AndRemovesTodo()
    {
        var created = await CreateTodoAsync("Delete me");

        var response = await _client.DeleteAsync($"{BaseUrl}/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var getResponse = await _client.GetAsync($"{BaseUrl}/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenMissing_Returns404()
    {
        var response = await _client.DeleteAsync($"{BaseUrl}/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<TodoDto> CreateTodoAsync(string title)
    {
        var response = await _client.PostAsJsonAsync(BaseUrl, new CreateTodoRequest(title));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TodoDto>())!;
    }
}

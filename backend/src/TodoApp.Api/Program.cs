using Scalar.AspNetCore;
using TodoApp.Api.Endpoints;
using TodoApp.Application;
using TodoApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure();

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapOpenApi();
app.MapScalarApiReference(options => options.DisableAgent());

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapTodoEndpoints();

app.MapFallbackToFile("index.html");

app.Run();

public partial class Program;

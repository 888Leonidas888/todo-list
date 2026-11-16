using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using TodoList.Application.DTOs;

namespace TodoList.Tests;

public class TodoApiTests
{
    private static WebApplicationFactory<Program> CreateFactory()
    {
        var dbName = $"TodoTestDb_{Guid.NewGuid()}";
        return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Testing:DatabaseName"] = dbName
                });
            });
        });
    }

    [Fact]
    public async Task Health_Endpoint_Returns_Healthy()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Full_Crud_Workflow_Succeeds()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        // 1. Create
        var createDto = new CreateTodoDto("Configurar CI/CD", "Revisar pipeline en GitHub Actions");
        var createResponse = await client.PostAsJsonAsync("/api/todos", createDto);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<TodoDto>();
        Assert.NotNull(created);
        Assert.Equal("Configurar CI/CD", created.Title);
        Assert.Equal("Revisar pipeline en GitHub Actions", created.Description);
        Assert.False(created.IsCompleted);

        // 2. Get all
        var list = await client.GetFromJsonAsync<List<TodoDto>>("/api/todos");
        Assert.NotNull(list);
        Assert.Single(list);

        // 3. Update & Complete
        var updateDto = new UpdateTodoDto("Configurar CI/CD en GCP", "Pipeline listo", true);
        var updateResponse = await client.PutAsJsonAsync($"/api/todos/{created.Id}", updateDto);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await updateResponse.Content.ReadFromJsonAsync<TodoDto>();
        Assert.NotNull(updated);
        Assert.Equal("Configurar CI/CD en GCP", updated.Title);
        Assert.True(updated.IsCompleted);

        // 4. Delete
        var deleteResponse = await client.DeleteAsync($"/api/todos/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // 5. Verify deleted
        var getResponse = await client.GetAsync($"/api/todos/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Create_With_Empty_Title_Returns_BadRequest()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/todos", new CreateTodoDto("   ", null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

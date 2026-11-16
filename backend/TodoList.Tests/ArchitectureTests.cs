using TodoList.Application.Services;
using TodoList.Domain.Entities;

namespace TodoList.Tests;

public class ArchitectureTests
{
    [Fact]
    public void Domain_Should_Not_Depend_On_Other_Layers_Or_EntityFramework()
    {
        var domainAssembly = typeof(TodoItem).Assembly;
        var referencedAssemblies = domainAssembly
            .GetReferencedAssemblies()
            .Select(a => a.Name ?? string.Empty)
            .ToList();

        Assert.DoesNotContain(referencedAssemblies, name => name.StartsWith("TodoList.Application"));
        Assert.DoesNotContain(referencedAssemblies, name => name.StartsWith("TodoList.Infrastructure"));
        Assert.DoesNotContain(referencedAssemblies, name => name.StartsWith("TodoList.Api"));
        Assert.DoesNotContain(referencedAssemblies, name => name.Contains("EntityFrameworkCore"));
    }

    [Fact]
    public void Application_Should_Only_Depend_On_Domain()
    {
        var applicationAssembly = typeof(TodoService).Assembly;
        var referencedAssemblies = applicationAssembly
            .GetReferencedAssemblies()
            .Select(a => a.Name ?? string.Empty)
            .ToList();

        Assert.Contains(referencedAssemblies, name => name == "TodoList.Domain");
        Assert.DoesNotContain(referencedAssemblies, name => name.StartsWith("TodoList.Infrastructure"));
        Assert.DoesNotContain(referencedAssemblies, name => name.StartsWith("TodoList.Api"));
        Assert.DoesNotContain(referencedAssemblies, name => name.Contains("EntityFrameworkCore"));
    }
}

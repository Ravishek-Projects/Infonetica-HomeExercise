using WorkflowEngine.Services;
using WorkflowEngine;

var builder = WebApplication.CreateBuilder(args);

// Register services for dependency injection.
builder.Services.AddSingleton<InMemoryStore>();
builder.Services.AddSingleton<WorkflowService>();

var app = builder.Build();
app.Run();
// --- API Endpoint Definitions ---

app.MapPost("/definitions", (WorkflowDefinition definition, WorkflowService service) =>
{
    var (createdDefinition, error) = service.CreateWorkflowDefinition(definition);
    if (error != null)
    {
        return Results.BadRequest(new { message = error });
    }
    return Results.CreatedAtRoute("GetDefinition", new { id = createdDefinition!.Id }, createdDefinition);
});

app.MapGet("/definitions/{id}", (string id, InMemoryStore store) => 
    store.WorkflowDefinitions.TryGetValue(id, out var definition)
        ? Results.Ok(definition)
        : Results.NotFound()
).WithName("GetDefinition");

app.MapPost("/instances", (StartInstanceRequest request, WorkflowService service) =>
{
    var (instance, error) = service.StartWorkflowInstance(request.DefinitionId);
    if (error != null)
    {
        return Results.BadRequest(new { message = error });
    }
    return Results.Ok(instance);
});

app.MapGet("/instances/{id}", (Guid id, InMemoryStore store) =>
    store.WorkflowInstances.TryGetValue(id, out var instance)
        ? Results.Ok(instance)
        : Results.NotFound()
);

app.MapPost("/instances/{id}/execute", (Guid id, ExecuteActionRequest request, WorkflowService service) =>
{
    var (instance, error) = service.ExecuteAction(id, request.ActionId);
    if (error != null)
    {
        return error.Contains("not found") ? Results.NotFound(new { message = error }) : Results.BadRequest(new { message = error });
    }
    return Results.Ok(instance);
});

public record StartInstanceRequest(string DefinitionId);
public record ExecuteActionRequest(string ActionId);


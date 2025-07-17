using System.Collections.Concurrent;

namespace WorkflowEngine.Services;

public class InMemoryStore
{
    // Using ConcurrentDictionary for thread safety, which is good practice for a singleton service.
    public ConcurrentDictionary<string, WorkflowDefinition> WorkflowDefinitions { get; } = new();
    public ConcurrentDictionary<Guid, WorkflowInstance> WorkflowInstances { get; } = new();
}
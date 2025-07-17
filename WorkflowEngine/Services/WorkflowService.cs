namespace WorkflowEngine.Services;

public class WorkflowService
{
    private readonly InMemoryStore _store;

    public WorkflowService(InMemoryStore store)
    {
        _store = store;
    }

    // Creates and validates a new workflow definition.
    public (WorkflowDefinition? Definition, string? Error) CreateWorkflowDefinition(WorkflowDefinition definition)
    {
        // Validation Rule: Reject invalid definitions (e.g., duplicate IDs)
        if (_store.WorkflowDefinitions.ContainsKey(definition.Id))
        {
            return (null, $"Workflow definition with ID '{definition.Id}' already exists.");
        }

        // Validation Rule: Must contain exactly one isInitial == true state.
        var initialStates = definition.States.Count(s => s.IsInitial);
        if (initialStates != 1)
        {
            return (null, $"A workflow definition must have exactly one initial state. Found {initialStates}.");
        }
        
        _store.WorkflowDefinitions[definition.Id] = definition;
        return (definition, null);
    }

    // Starts a new workflow instance.
    public (WorkflowInstance? Instance, string? Error) StartWorkflowInstance(string definitionId)
    {
        if (!_store.WorkflowDefinitions.TryGetValue(definitionId, out var definition))
        {
            return (null, $"Workflow definition with ID '{definitionId}' not found.");
        }

        var initialState = definition.States.Single(s => s.IsInitial);
        var instance = new WorkflowInstance
        {
            DefinitionId = definitionId,
            CurrentState = initialState.Id
        };

        _store.WorkflowInstances[instance.Id] = instance;
        return (instance, null);
    }

    // Executes an action on a workflow instance.
    public (WorkflowInstance? Instance, string? Error) ExecuteAction(Guid instanceId, string actionId)
    {
        if (!_store.WorkflowInstances.TryGetValue(instanceId, out var instance))
        {
            return (null, "Workflow instance not found.");
        }

        if (!_store.WorkflowDefinitions.TryGetValue(instance.DefinitionId, out var definition))
        {
            return (null, "Internal Server Error: Could not find definition for the instance.");
        }

        var currentState = definition.States.FirstOrDefault(s => s.Id == instance.CurrentState);
        
        if (currentState?.IsFinal == true)
        {
            return (null, "Action rejected: The workflow instance is in a final state.");
        }

        var action = definition.Actions.FirstOrDefault(a => a.Id == actionId);
        
        if (action == null)
        {
            return (null, $"Action '{actionId}' not found in the workflow definition.");
        }

        if (!action.Enabled)
        {
            return (null, $"Action '{actionId}' is disabled.");
        }

        if (!action.FromStates.Contains(instance.CurrentState))
        {
            return (null, $"Action '{actionId}' cannot be executed from the current state '{instance.CurrentState}'.");
        }
        
        var previousState = instance.CurrentState;
        instance.CurrentState = action.ToState;
        instance.History.Add(new WorkflowHistoryItem(actionId, previousState, action.ToState, DateTime.UtcNow));

        return (instance, null);
    }
}
namespace WorkflowEngine;

// A state within a workflow definition.
public record State(
    string Id,
    bool IsInitial = false,
    bool IsFinal = false,
    bool Enabled = true
);

// An action that transitions a workflow from a set of source states to a single target state.
public record Action(
    string Id,
    List<string> FromStates,
    string ToState,
    bool Enabled = true
);

// Defines the structure (states and actions) of a workflow.
public record WorkflowDefinition(
    string Id,
    List<State> States,
    List<Action> Actions
);

// Represents a single, running instance of a workflow definition.
public record WorkflowInstance
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string DefinitionId { get; init; }
    public required string CurrentState { get; set; }
    public List<WorkflowHistoryItem> History { get; } = [];
}

// A record of an action that was executed on an instance.
public record WorkflowHistoryItem(
    string ActionId,
    string FromState,
    string ToState,
    DateTime Timestamp
);
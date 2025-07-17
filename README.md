# Infonetica - Configurable Workflow Engine

[cite_start]This project is a minimal backend service that implements a configurable workflow engine based on a state-machine concept, as per the take-home exercise specification. [cite: 1] [cite_start]It's built with .NET 8 and ASP.NET Core Minimal APIs. [cite: 26, 27]

## Assumptions & Shortcuts

* [cite_start]**Persistence**: The service uses a simple, thread-safe in-memory store.  Data will be lost upon application restart. This was chosen for simplicity and to meet the 2-hour time constraint.
* [cite_start]**Validation**: Core validation rules are implemented. [cite: 21, 22] However, more comprehensive validation (e.g., ensuring all `toState` and `fromStates` in actions correspond to actual defined states) would be added in a production environment. This is noted as a `TODO` in the code.
* **IDs**: State and Action IDs are treated as `string`. Workflow Instance IDs are `Guid`.
* **Error Handling**: Errors are returned as simple JSON objects with a `message` property.

## How to Run

1.  Ensure you have the .NET 8 SDK installed.
2.  Clone the repository.
3.  Navigate to the project directory in your terminal.
4.  [cite_start]Run the application using the command: `dotnet run`. [cite: 40]
5.  The service will be available at `http://localhost:5000` (or a similar port).

## API Endpoints

### 1. Create Workflow Definition

Creates a new workflow definition.

* **Endpoint**: `POST /definitions`
* **Request Body**:

```json
{
  "id": "leave-request",
  "states": [
    { "id": "Submitted", "isInitial": true },
    { "id": "Approved", "isFinal": true },
    { "id": "Rejected", "isFinal": true }
  ],
  "actions": [
    { "id": "Approve", "fromStates": ["Submitted"], "toState": "Approved" },
    { "id": "Reject", "fromStates": ["Submitted"], "toState": "Rejected" }
  ]
}
```

### 2. Get Workflow Definition

Retrieves an existing workflow definition by its ID.

* **Endpoint**: `GET /definitions/{id}`

### 3. Start Workflow Instance

Starts a new instance of a specified workflow definition.

* **Endpoint**: `POST /instances`
* **Request Body**:

```json
{
  "definitionId": "leave-request"
}
```

### 4. Get Workflow Instance

Retrieves the current state and history of a workflow instance.

* **Endpoint**: `GET /instances/{id}`

### 5. Execute Action on Instance

Executes a state transition on a given instance.

* **Endpoint**: `POST /instances/{id}/execute`
* **Request Body**:

```json
{
  "actionId": "Approve"
}
```
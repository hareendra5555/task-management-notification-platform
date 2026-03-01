# Contributing

## Workflow
1. Create a feature branch from `main`.
2. Keep commits small and focused on one change.
3. Use clear commit titles in imperative style (example: `Add task summary endpoint`).
4. Open a pull request with a short change summary and test notes.

## Local Validation
1. Run `dotnet restore`.
2. Run `dotnet build TaskFlow.API.sln`.
3. Start dependencies (`docker compose up -d sqlserver redis`) or equivalent local services.
4. Run the API and validate requests using `TaskFlow.API/TaskFlow.API.http`.

## API Change Checklist
1. Update request/response contracts with validation attributes.
2. Add or update endpoint docs in `README.md`.
3. Invalidate cache entries when mutating task state.
4. Ensure notification events are published for create/update/delete flows.

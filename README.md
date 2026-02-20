# Task Management & Notification Platform

Enterprise-oriented backend platform for task lifecycle management, priority scoring, Redis-backed read caching, and event-style notifications.

## Why This Project
This project demonstrates how to take a simple CRUD API and evolve it into a production-style service with:
- layered service design
- predictable task prioritization logic
- cache-aware read endpoints
- notification event publishing hooks
- containerized local development

## Tech Stack
- ASP.NET Core Web API (.NET 9)
- Entity Framework Core + SQL Server
- Redis distributed cache
- Swagger / OpenAPI
- Docker Compose

## Core Features
- Task CRUD APIs with validation-ready request contracts
- Priority scoring engine (`IPriorityScoringService`) for ranking tasks
- Redis cache for `GET /api/tasks` and `GET /api/tasks/{id}`
- Notification event publishing on create/update/delete operations
- Global exception middleware for consistent API error responses
- Repository + Unit of Work patterns

## API Endpoints
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/tasks` | Get tasks with filtering and pagination |
| GET | `/api/tasks/{id}` | Get task by id (cache-aware) |
| POST | `/api/tasks` | Create task and publish creation event |
| PUT | `/api/tasks/{id}` | Update task, recompute score, publish update event |
| DELETE | `/api/tasks/{id}` | Delete task and publish deletion event |
| GET | `/api/tasks/notifications?count=20` | View recent notification events |
| GET | `/api/tasks/summary` | Get aggregate task metrics (total/completed/pending/overdue) |
| GET | `/api/health` | Health check endpoint for runtime monitoring |

## Example API Requests
Create task:

```bash
curl -X POST "http://localhost:8080/api/tasks" \
  -H "Content-Type: application/json" \
  -d "{\"title\":\"Ship Day 2 updates\",\"description\":\"Add CI and health endpoint\",\"dueDate\":\"2026-02-21T10:00:00Z\",\"isHighUrgency\":true}"
```

Get all tasks:

```bash
curl "http://localhost:8080/api/tasks"
```

Filter + pagination:

```bash
curl "http://localhost:8080/api/tasks?isCompleted=false&isHighUrgency=true&search=ship&dueFrom=2026-02-20T00:00:00Z&dueTo=2026-02-28T23:59:59Z&sortBy=dueDate&sortDirection=asc&page=1&pageSize=10"
```

Get task summary:

```bash
curl "http://localhost:8080/api/tasks/summary"
```

Get service health:

```bash
curl "http://localhost:8080/api/health"
```

## Local Run (Docker)
From repository root:

```bash
docker compose up --build
```

Services:
- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`
- SQL Server: `localhost:1433`
- Redis: `localhost:6379`

## Local Run (Without Docker)
1. Start SQL Server and Redis locally.
2. Update `TaskFlow.API/appsettings.json` connection strings if needed.
3. Run:

```bash
cd TaskFlow.API
dotnet restore
dotnet ef database update
dotnet run
```

## Architecture Notes
- `Controllers/`: API contracts and HTTP orchestration.
- `Repositories/` + `UnitOfWork/`: persistence abstraction and transaction boundary.
- `Services/`: domain-level behavior (priority, cache, notifications).
- `Middleware/`: centralized exception handling.
- `Data/`: EF Core context and model configuration (includes useful indexes).

## Roadmap
- Replace in-memory notification stream with message broker integration (Kafka/SQS).
- Add SignalR push notifications for realtime task updates.
- Add background workers for retry and dead-letter handling.
- Expand CI into full pipeline (tests, linting, release checks).

## Attribution
Initially bootstrapped from `B3nchi/B3nchi-ASP.NET-Core-MVC-Web-APIs---Simple-Task-Management` and then significantly refactored and extended for enterprise-style architecture and platform capabilities.

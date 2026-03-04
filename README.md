# ETSS Payroll API

Production-grade REST API for the ETSS HR Payroll system.
**Stack:** .NET 8 · ASP.NET Core · SQL Server 2022 · Entity Framework Core 8 · JWT Bearer

---

## Architecture

```
Clean Architecture (4-layer)
├── PayrollApi.Domain          ← Entities, interfaces (no dependencies)
├── PayrollApi.Application     ← Use cases, services, DTOs, validators, mappings
├── PayrollApi.Infrastructure  ← EF Core, repositories, Unit of Work, DB config
└── PayrollApi.API             ← Controllers, middleware, DI registration, Swagger
```

**Dependency rule:** outer layers depend on inner layers. Domain depends on nothing.

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server 2022 (`localhost\SQLEXPRESS`)
- Visual Studio 2022 or VS Code with C# Dev Kit

---

## Quick Start

```bash
# 1. Clone and navigate
cd payroll-api

# 2. Configure environment
cp .env.example .env
# Edit .env with your connection string and JWT secret

# 3. Restore packages
dotnet restore PayrollApi.sln

# 4. Apply EF Core migrations
cd src/PayrollApi.API
dotnet ef migrations add InitialCreate --project ../PayrollApi.Infrastructure
dotnet ef database update --project ../PayrollApi.Infrastructure

# 5. Run
dotnet run --project src/PayrollApi.API

# API:     https://localhost:7000/api/v1/
# Swagger: https://localhost:7000/swagger
# Health:  https://localhost:7000/health
```

---

## Environment Variables

| Variable | Description | Required |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | MSSQL connection string | Yes |
| `Jwt__SecretKey` | Min 32-char secret for JWT signing | Yes |
| `Jwt__Issuer` | JWT issuer claim | Yes |
| `Jwt__Audience` | JWT audience claim | Yes |
| `Jwt__ExpiryMinutes` | Token TTL in minutes (default: 60) | No |
| `AllowedOrigins__0` | CORS allowed origin (Angular dev) | Yes |

---

## API Conventions

### Response Envelope
All responses follow this structure:
```json
{
  "success": true,
  "data": { ... },
  "message": "Optional message",
  "meta": { "page": 1, "pageSize": 10, "totalCount": 100, "totalPages": 10 },
  "errors": null,
  "traceId": null
}
```

### Error Response
```json
{
  "success": false,
  "data": null,
  "errors": [{ "code": "Email", "message": "Email is already registered.", "field": "Email" }],
  "traceId": "0HN5F..."
}
```

### Versioning
URI-based: `/api/v1/employees`, `/api/v2/employees`
Header-based (alternative): `X-Api-Version: 1`

### Pagination
```
GET /api/v1/employees?page=1&pageSize=20&search=juan&sortBy=lastName&sortDirection=asc
```

### HTTP Status Codes
| Status | When |
|---|---|
| 200 OK | Successful GET, PUT, PATCH |
| 201 Created | Successful POST |
| 204 No Content | Successful DELETE |
| 400 Bad Request | Invalid request / business rule violation |
| 401 Unauthorized | Missing or invalid JWT |
| 403 Forbidden | Authenticated but insufficient role |
| 404 Not Found | Resource not found |
| 409 Conflict | Duplicate constraint violation |
| 422 Unprocessable Entity | FluentValidation failure |
| 429 Too Many Requests | Rate limit exceeded |
| 500 Internal Server Error | Unexpected server fault |

---

## Roles

| Role | Access |
|---|---|
| `Admin` | Full access |
| `PayrollAdmin` | Run payroll, approve, release |
| `HrStaff` | Manage employees |
| `Manager` | View only |

---

## Coding Standards

### Naming
| Artifact | Convention | Example |
|---|---|---|
| Files | PascalCase | `EmployeeService.cs` |
| Classes | PascalCase | `EmployeeService` |
| Interfaces | `I` prefix + PascalCase | `IEmployeeService` |
| Methods | PascalCase async with `Async` suffix | `GetByIdAsync` |
| Variables | camelCase | `employeeId` |
| Private fields | `_camelCase` | `_unitOfWork` |
| DB Tables | PascalCase plural | `Employees` |
| DB Columns | PascalCase | `FirstName` |
| Endpoints | kebab-case plural nouns | `/api/v1/payroll-records` |

### Error Handling
- All exceptions thrown from service layer using typed exceptions in `Application.Common.Exceptions`
- Global `ExceptionMiddleware` maps exceptions → HTTP status codes
- Never return raw exception messages in production

### Logging Levels
| Level | When to use |
|---|---|
| `Verbose` | Low-level DB query details (disabled in prod) |
| `Debug` | Service method entry/exit, parameter dumps |
| `Information` | Business events (employee created, payroll run) |
| `Warning` | Handled exceptions, degraded conditions |
| `Error` | Unhandled exceptions, integration failures |
| `Fatal` | Startup/shutdown failures |

### Async/Await Rules
- All I/O operations must be `async` with `CancellationToken` propagation
- Never use `.Result` or `.Wait()` — always `await`
- Pass `CancellationToken` from controller → service → repository

---

## Database Migrations

```bash
# Add migration (from solution root)
dotnet ef migrations add <MigrationName> \
  --project src/PayrollApi.Infrastructure \
  --startup-project src/PayrollApi.API

# Apply to DB
dotnet ef database update \
  --project src/PayrollApi.Infrastructure \
  --startup-project src/PayrollApi.API

# Rollback
dotnet ef database update <PreviousMigrationName> \
  --project src/PayrollApi.Infrastructure \
  --startup-project src/PayrollApi.API
```

---

## Testing

```bash
dotnet test PayrollApi.sln
dotnet test --collect:"XPlat Code Coverage"
```

---

## Security Checklist

- [ ] JWT SecretKey is ≥ 32 characters and stored in environment variables / secrets manager
- [ ] Connection string uses Windows Auth or a least-privilege SQL login — never `sa`
- [ ] `TrustServerCertificate=False` in production (use a proper cert)
- [ ] Sensitive fields (bank account, TIN, SSS) encrypted at application level before storage
- [ ] Payroll figures masked in logs — never log `NetPay`, `BasicSalary` at Info level
- [ ] HTTPS enforced in production
- [ ] Rate limiting enabled for payroll run endpoint
- [ ] CORS origin list locked down to known domains in production

# CaseTracker — System Architecture & Clean Layering

> **Document Version:** 1.2  
> **Framework:** ASP.NET Core (.NET 10)  
> **Pattern:** Clean Architecture (Onion / Hexagonal Architecture)  
> **Status:** Active Baseline  
> **Parent Documentation:** [Documentation Center](../README.md)

---

## 1. Architectural Philosophy

CaseTracker is structured according to the principles of **Clean Architecture** (Robert C. Martin) and the **Dependency Inversion Principle**:

* **Dependencies flow strictly inward**: Outer layers depend on inner layers; inner layers never depend on outer layers.
* **Domain Independence**: The Domain Layer (`CaseTrackerDomain`) and Application Layer (`CaseTrackerApplication`) are completely decoupled from concrete frameworks, web servers, databases, and UI implementations.
* **Separation of Concerns**: Business validation, database persistence, HTTP presentation, and external service gateways are isolated in distinct project assemblies.

```mermaid
graph TD
    subgraph PresentationLayer ["1. Presentation Layer (CaseTracker API)"]
        API["Controllers<br/>AuthController • UsersController • LawyersController"]
        Middleware["Middleware<br/>GlobalExceptionHandlerMiddleware • JWT Auth"]
        Swagger["Swagger & OpenAPI Documentation"]
    end

    subgraph ApplicationLayer ["2. Application Layer (CaseTrackerApplication)"]
        AppServices["Application Services<br/>AuthService • LawyerService • LawFirmService"]
        DTOs["Data Transfer Objects (DTOs)"]
        Interfaces["Repository & UnitOfWork Interfaces"]
        Exceptions["Domain Exceptions (NotFound, Conflict, Validation)"]
    end

    subgraph DomainLayer ["3. Domain Layer (CaseTrackerDomain)"]
        DomainEntities["Domain Entities<br/>User • Lawyer • LawFirm • Role • UserRole • UserLawFirm"]
        Enums["Domain Constants & Status Enums"]
    end

    subgraph InfrastructureLayer ["4. Infrastructure Layer (CaseTrackerInfrastructure)"]
        DbContext["EF Core ApplicationDbContext"]
        Repos["Concrete Repositories (UserRepository, LawyerRepository)"]
        UoW["UnitOfWork Implementation"]
        Security["BCrypt PasswordHasher • JWT TokenService"]
    end

    subgraph ExternalServices ["External Infrastructure"]
        Postgres[("Neon Cloud PostgreSQL<br/>(Serverless Pooler)")]
        IIS["MonsterASP.net IIS Web Server"]
    end

    API --> AppServices
    API --> Middleware
    Middleware --> AppServices
    AppServices --> DomainEntities
    AppServices --> Interfaces
    AppServices --> DTOs
    AppServices --> Exceptions
    InfrastructureLayer --> Interfaces
    InfrastructureLayer --> DomainEntities
    DbContext --> Postgres
    IIS --> API
```

---

## 2. Project Assemblies & Responsibilities

### 2.1 `CaseTrackerDomain` (Domain Core)
* **Zero External Dependencies**: Pure C# class library.
* **Contains**:
  * Core entity definitions with navigation properties.
  * Audit fields (`CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`).
  * Enforces state consistency across aggregate roots.

### 2.2 `CaseTrackerApplication` (Use Cases & Business Logic)
* **Dependencies**: References `CaseTrackerDomain` only.
* **Contains**:
  * **DTOs**: Request and response payloads (e.g. `RegisterRequest`, `LoginRequest`, `AuthResult`, `ApiResponse<T>`).
  * **Contracts**: `IUserRepository`, `ILawyerRepository`, `ILawFirmRepository`, `IRoleRepository`, `IUnitOfWork`, `IAuthService`, `IJwtService`, `IPasswordService`.
  * **Business Workflows**: Multi-step operations like lawyer registration, role assignment, and validation checks.
  * **Exceptions**: Custom domain exceptions mapped to HTTP response codes.

### 2.3 `CaseTrackerInfrastructure` (Data Access & Security)
* **Dependencies**: References `CaseTrackerApplication` and `CaseTrackerDomain`.
* **Contains**:
  * **`ApplicationDbContext`**: Configures PostgreSQL tables, schemas, relations, indexes, and seed data.
  * **Repositories**: Generic `Repository<T>` and specialized repositories implementing application contracts.
  * **`UnitOfWork`**: Manages EF Core database transactions (`BeginTransactionAsync`, `CommitTransactionAsync`, `RollbackTransactionAsync`).
  * **Security**: Concrete password hashing and JWT token issuance.

### 2.4 `CaseTracker` (Presentation Web API)
* **Dependencies**: References `CaseTrackerApplication` and `CaseTrackerInfrastructure`.
* **Contains**:
  * **Controllers**: Exposes REST endpoints (`/api/auth`, `/api/lawyers`, `/api/version`, etc.).
  * **Middleware**: `GlobalExceptionHandlerMiddleware` captures all unhandled exceptions and outputs consistent RFC-compliant JSON responses.
  * **Service Registration**: Clean extension methods in `Extensions/` for dependency injection.

---

## 3. End-to-End Request Execution Pipeline

The flowchart below demonstrates the path of an HTTP request through the system:

```mermaid
sequenceDiagram
    autonumber
    actor Client as Client / Mobile App
    participant IIS as IIS Web Server (MonsterASP)
    participant Pipe as ASP.NET Core Middleware Pipeline
    participant MW as GlobalExceptionHandlerMiddleware
    participant Auth as JWT Authentication Handler
    participant Ctrl as API Controller (e.g. LawyersController)
    participant Svc as Application Service (e.g. LawyerService)
    participant Repo as LawyerRepository
    participant DB as Neon PostgreSQL

    Client->>IIS: HTTP GET /api/lawyers/{id} (Bearer Token)
    IIS->>Pipe: Forward to Kestrel / AspNetCoreModuleV2
    Pipe->>MW: Enter GlobalExceptionHandlerMiddleware
    MW->>Auth: Validate JWT Signature & Claims
    alt Invalid or Expired Token
        Auth-->>Client: 401 Unauthorized
    else Valid Token
        Auth->>Ctrl: Invoke Controller Action with ClaimsPrincipal
        Ctrl->>Svc: GetLawyerByIdAsync(id)
        Svc->>Repo: GetByIdAsync(id)
        Repo->>DB: SELECT * FROM lawyers WHERE lawyer_id = @id
        DB-->>Repo: Query Result Row
        Repo-->>Svc: Lawyer Entity
        alt Lawyer Not Found
            Svc-->>MW: throw NotFoundException("Lawyer not found")
            MW-->>Client: 404 Not Found { success: false, message }
        else Lawyer Found
            Svc-->>Ctrl: LawyerDto
            Ctrl-->>Client: 200 OK { success: true, data: LawyerDto }
        end
    end
```

---

## 4. Error Handling Strategy

All system exceptions are processed centrally by [`GlobalExceptionHandlerMiddleware`](file:///d:/ProjectApp/CaseTracker/CaseTracker/Middleware/GlobalExceptionHandlerMiddleware.cs):

| Exception Type | HTTP Status | Response Schema |
| :--- | :---: | :--- |
| `ValidationException` | `400 Bad Request` | `{ success: false, message: "Validation failed", errors: [...] }` |
| `UnauthorizedAccessException` / `UnauthorizedException` | `401 Unauthorized` | `{ success: false, message: "Unauthorized" }` |
| `KeyNotFoundException` / `NotFoundException` | `404 Not Found` | `{ success: false, message: "Resource not found" }` |
| `InvalidOperationException` / `ConflictException` | `409 Conflict` | `{ success: false, message: "Conflict occurred" }` |
| Unhandled Exceptions | `500 Internal Server Error` | `{ success: false, message: "An unexpected error occurred" }` |

This prevents stack traces and database internal details from ever leaking to the client in production.

# CaseTracker — Coding Standards & Engineering Guidelines

> **Language:** C# (.NET 10)  
> **Nullable Reference Types:** Enabled (`<Nullable>enable</Nullable>`)  
> **Parent Documentation:** [Documentation Center](../README.md)

---

## 1. Clean Architecture Boundary Rules

To protect the integrity of the architecture, every engineer must follow these dependency rules:

1. **`CaseTrackerDomain` is Sacred**:
   - Zero third-party NuGet dependencies.
   - Zero references to Entity Framework Core or ASP.NET Core.
   - All entity models must be pure Plain Old CLR Objects (POCOs).

2. **`CaseTrackerApplication` Contains Logic, Not Infrastructure**:
   - No direct database queries or SQL. Data is accessed solely via interfaces (`IRepository`, `IUnitOfWork`).
   - Does not reference ASP.NET Core HTTP libraries (`HttpContext`, `ControllerBase`).

3. **Controllers Must Be Thin**:
   - Web API controllers are responsible solely for HTTP routing, request deserialization, calling the appropriate application service, and returning HTTP status codes.
   - Zero business validation or database operations in controllers.

4. **Use Centralized Domain Exceptions**:
   - Do **not** return raw HTTP error codes from services.
   - Throw structured domain exceptions defined in `CaseTrackerApplication.Exceptions`:
     - `ValidationException` &rarr; Returns `400 Bad Request`
     - `UnauthorizedException` &rarr; Returns `401 Unauthorized`
     - `ForbiddenException` &rarr; Returns `403 Forbidden`
     - `NotFoundException` &rarr; Returns `404 Not Found`
     - `ConflictException` &rarr; Returns `409 Conflict`
   - `GlobalExceptionHandlerMiddleware` automatically catches these exceptions and outputs standardized JSON responses:
     ```json
     {
       "success": false,
       "message": "An account with this mobile number already exists.",
       "errors": null,
       "data": null
     }
     ```

---

## 2. C# Code Style & Naming Conventions

* **Classes, Records, Interfaces, Methods, Properties**: `PascalCase`
* **Interfaces**: Prefix with `I` (e.g., `ILawyerRepository`, `IAuthService`).
* **Private Readonly Fields**: Prefix with an underscore and `camelCase` (e.g., `_userRepository`, `_unitOfWork`).
* **Parameters & Local Variables**: `camelCase` (e.g., `mobileNumber`, `lawFirmId`).
* **Async Methods**: Suffix with `Async` (e.g., `GetByMobileNumberAsync()`, `RegisterAsync()`). Always propagate `CancellationToken` when dealing with external I/O.

---

## 3. Asynchronous Programming Guidelines

* **Always use `async`/`await` for I/O operations** (database queries, network requests).
* **Never use `.Result` or `.Wait()`** as this causes thread-pool starvation and deadlocks.
* **Avoid `async void`** except for event handlers in UI controls.


# Case Tracker App — Project Scope

## 1. Scope Definition

This document defines the boundaries of the Case Tracker project.

It establishes:

* What is included in the current project.
* What is planned for later phases.
* What is explicitly outside the project.
* The boundaries that should be considered before adding new requirements.

Detailed business objectives are documented in `01-overview.md`.

Detailed application functionality is documented in `03-features.md`.

---

## 2. Current Project Scope

The current project will cover the complete foundation required to build and operate the Case Tracker application.

The project scope includes:

* Application design and planning.
* Database and entity design.
* Backend API development.
* Mobile application development.
* Authentication and authorization foundation.
* External service integration.
* File storage architecture.
* Deployment-ready application architecture.
* Data security and organizational data isolation.
* Testing and validation of the implemented functionality.

The exact functionality included in each release will be defined separately during release planning.

---

## 3. Release Boundaries

The project will be developed incrementally rather than implementing every planned capability at once.

### Phase 1 — Foundation

Establish the technical and data foundation:

* Project structure.
* Database design.
* Entity relationships.
* Authentication foundation.
* Authorization foundation.
* API foundation.
* Mobile application foundation.

### Phase 2 — Core Application

Implement the primary application workflow defined in `03-features.md`.

### Phase 3 — External Integrations

Implement and validate required external services and synchronization workflows.

### Phase 4 — Advanced Capabilities

Implement features identified as future scope after the core application is stable.

---

## 4. Geographic Boundary

The initial implementation will be limited to the geographical coverage required for the first release.

The architecture must not hard-code the system around the initial geographical coverage.

Additional geographical coverage will be treated as an expansion phase.

---

## 5. Platform Boundary

### Initial Platforms

* Backend: ASP.NET Core Web API
* Mobile: .NET MAUI
* Database: PostgreSQL

Other platforms or clients are not part of the initial implementation unless added to the project scope.

---

## 6. Data Boundary

The application will use a shared database model.

Data belonging to different organizations must remain logically isolated through application-level ownership and authorization rules.

The database design will be documented separately under:

```text
docs/database/
```

Entity-specific decisions will be documented under:

```text
docs/entities/
```

This document does not define individual tables or columns.

---

## 7. Integration Boundary

External services will be integrated only where they are required by the approved project scope.

External service limitations, authentication requirements, available operations, synchronization behavior, and failure handling will be documented separately within the relevant integration documents.

The application must not assume that an external service will always be available.

---

## 8. Out of Scope

The following are outside the current project unless explicitly approved as new requirements:

* Building or replacing an official government judicial system.
* Building court-side infrastructure.
* Providing legal advice or legal decisions.
* Replacing professional legal judgment.
* Unapproved third-party integrations.
* Features that require major changes to the agreed architecture without scope review.
* Unrelated financial, accounting, or business-management functionality.
* Features unrelated to the application's approved product direction.

---

## 9. Change of Scope

A new requirement should not automatically become part of the current implementation.

Before adding a new requirement, evaluate its impact on:

* Existing requirements.
* Database design.
* Entity relationships.
* API contracts.
* Application architecture.
* Security.
* External integrations.
* Development effort.
* Future maintenance.

If the requirement significantly changes the existing boundaries, it should be treated as a **scope change** and documented before implementation.

---

## 10. Scope vs Feature Documentation

This distinction must be maintained throughout the project:

```text
01-overview.md
    ↓
Project purpose and business direction

02-project-scope.md
    ↓
Project boundaries and delivery phases

03-features.md
    ↓
Application capabilities
```

The same information should not be maintained in multiple documents.

Each document should have a single responsibility.

---

## 11. Scope Decision Rule

Before implementing a new requirement, ask:

> "Is this already within the approved project scope?"

If **yes**, determine which feature or technical document it belongs to.

If **no**, evaluate it as a new scope item before implementation.

This keeps the project controlled and prevents features from being added without considering their impact on the overall design.

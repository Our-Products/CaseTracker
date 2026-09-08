# ADR-0001: Adoption of Clean Architecture

> **Status:** Accepted  
> **Date:** 2026-09-04  
> **Deciders:** Engineering Architecture Team  
> **Parent Documentation:** [Architecture Overview](../01_system_architecture.md)

---

## Context
CaseTracker is a long-term enterprise legal system that will manage high volumes of cases, integrate with external government court portals (eCourts), handle sensitive legal documents, and support multiple client apps (.NET MAUI mobile, future web portal). 

A monolithic "spaghetti" or traditional three-tier architecture (where business logic directly queries the database) leads to tight coupling, poor testability, and difficult migrations when database schemas or external APIs evolve.

---

## Decision
We adopted **Clean Architecture** (Dependency Inversion Principle) across four distinct assemblies:
1. `CaseTrackerDomain`: Pure POCO entities and business rules without external library dependencies.
2. `CaseTrackerApplication`: Application use cases, DTOs, domain exceptions, and repository interfaces.
3. `CaseTrackerInfrastructure`: Concrete EF Core DbContext, PostgreSQL mappings, UnitOfWork, and external security implementations.
4. `CaseTracker` (Presentation Web API) & `CaseTrackerMobile`: Consumer apps interacting exclusively via contracts and HTTP APIs.

---

## Consequences

### Positive
* **Independent of Database**: Swapping PostgreSQL or running automated unit tests with in-memory or mock repositories requires zero changes to core business logic.
* **Testability**: Use cases in `CaseTrackerApplication` can be tested in complete isolation.
* **Separation of Concerns**: Teams working on UI (MAUI) and backend services operate on well-defined DTO contracts.

### Negative
* **Boilerplate**: Requires interfaces, DTOs, and mapping layers for simple CRUD operations.
* **Learning Curve**: Requires strict discipline to prevent engineers from referencing infrastructure libraries in the domain or application projects.


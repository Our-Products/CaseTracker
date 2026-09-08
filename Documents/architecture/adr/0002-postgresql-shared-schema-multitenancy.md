# ADR-0002: PostgreSQL Shared-Table Multi-Tenancy Strategy

> **Status:** Accepted  
> **Date:** 2026-09-04  
> **Deciders:** Engineering Architecture Team  
> **Parent Documentation:** [Database Architecture](../02_database_architecture.md)

---

## Context
CaseTracker must serve thousands of independent advocates alongside medium-to-large law firms. Data belonging to distinct firms and lawyers must remain isolated.

We evaluated three multi-tenancy models:
1. **Database-per-tenant**: A separate PostgreSQL database for each law firm.
2. **Schema-per-tenant**: A single database with separate PostgreSQL schemas (`firm_a`, `firm_b`).
3. **Shared-table with logical isolation**: A single schema where all rows include tenant foreign keys (`law_firm_id`, `user_id`).

---

## Decision
We chose the **Shared-Table with Logical Isolation** model on PostgreSQL for Phase 1 and 2.

All records in shared tables (`lawyers`, `clients`, `cases`, `hearings`) belong logically to a firm or lawyer via foreign keys. Authorization and repository queries strictly filter on the authenticated user's organization context.

---

## Consequences

### Positive
* **Cost Effective**: Avoids managing thousands of idle database connections or running schema migrations across thousands of schemas.
* **Streamlined Migrations**: Running `dotnet ef database update` updates the entire application schema in one single operation.
* **Simpler Cross-Tenant Indexing & Analytics**: Aggregating court statistics across Tamil Nadu and pan-India does not require cross-database queries.

### Negative / Mitigations
* **Risk of Cross-Tenant Data Leak**: If a developer forgets a `Where(x => x.LawFirmId == currentFirmId)` filter, data could leak.
  * *Mitigation*: Global query filters in EF Core, repository abstraction, and future evaluation of PostgreSQL Row-Level Security (RLS) policies.


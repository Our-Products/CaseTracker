# CaseTracker — Database Migrations Guide

> **ORM:** Entity Framework Core  
> **Provider:** Npgsql.EntityFrameworkCore.PostgreSQL  
> **Target Project:** `CaseTrackerInfrastructure`  
> **Startup Project:** `CaseTracker`  
> **Parent Documentation:** [Documentation Center](../README.md)

---

## 1. Overview

Database schema changes in CaseTracker are managed using **EF Core Code-First Migrations**. 

* The `ApplicationDbContext` resides in the **`CaseTrackerInfrastructure`** project.
* The configuration and connection string reside in the **`CaseTracker`** (Web API) project.

---

## 2. Essential Migration Commands

Always execute migration commands from the solution root (`D:\CaseTracker\Backend`):

### 2.1 Applying Migrations to Database
Updates the PostgreSQL database schema to the latest migration:
```powershell
dotnet ef database update --project CaseTrackerInfrastructure --startup-project CaseTracker
```

### 2.2 Adding a New Migration
When you modify or add entity models in `CaseTrackerDomain` or update mappings in `ApplicationDbContext`:
```powershell
dotnet ef migrations add <MigrationName> --project CaseTrackerInfrastructure --startup-project CaseTracker
```
*Example:*
```powershell
dotnet ef migrations add AddClientAndCaseEntities --project CaseTrackerInfrastructure --startup-project CaseTracker
```

### 2.3 Removing the Last Migration
If a migration has been generated but **not yet applied** to the database:
```powershell
dotnet ef migrations remove --project CaseTrackerInfrastructure --startup-project CaseTracker
```

### 2.4 Rolling Back to a Specific Migration
To roll back the database schema to an earlier migration state:
```powershell
dotnet ef database update <TargetMigrationName> --project CaseTrackerInfrastructure --startup-project CaseTracker
```

---

## 3. Generating Raw SQL Scripts for Production

Never run `dotnet ef database update` directly against a production database in CI/CD. Instead, generate idempotent SQL deployment scripts:

```powershell
dotnet ef migrations script --idempotent --output ./deployment_script.sql --project CaseTrackerInfrastructure --startup-project CaseTracker
```

---

## 4. Best Practices for CaseTracker Migrations

1. **Review Generated Migration**: Always inspect the generated C# code in `CaseTrackerInfrastructure/Migrations/` before applying it.
2. **Explicit Column Types & Lengths**: Ensure all string columns specify explicit lengths (`HasMaxLength(200)`) and appropriate SQL column types (`timestamptz` for timestamps).
3. **Foreign Key Restraints**: Pay strict attention to `OnDelete` rules (`DeleteBehavior.Restrict` for audit keys, `DeleteBehavior.Cascade` for parent-child lifecycles, and `DeleteBehavior.SetNull` for optional associations).


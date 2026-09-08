# CaseTracker — Documentation Center

Welcome to the **CaseTracker** documentation repository. This directory houses all architectural, product, engineering, API, and mobile specifications following the **Docs-as-Code** philosophy.

---

## 🧭 Documentation Map

```
Documents/
├── product/              # Business vision, user personas, roadmap & functional scope
├── architecture/         # System design, data models, ER diagrams & ADRs
│   └── adr/              # Architecture Decision Records
├── development/          # Local machine setup, migrations, and coding conventions
├── api/                  # API contracts, authentication flows, and endpoint specs
├── mobile/               # .NET MAUI architecture, Shadcn design system & offline cache
└── changelog/            # Project change logs and milestone release notes
```

---

## 📂 Categories & Quick Links

### 1. [Product & Business Specifications](file:///D:/CaseTracker/Backend/Documents/product/)
* **[01. Product Vision](file:///D:/CaseTracker/Backend/Documents/product/01_product_vision.md)**: Market problem, target users (Lawyers, Law-Firm Staff, Admins), and geographical strategy (Tamil Nadu, Puducherry & all-India expansion).
* **[02. Project Scope](file:///D:/CaseTracker/Backend/Documents/product/02_project_scope.md)**: Delivery phases (Phase 1: Foundation to Phase 4: Advanced), boundaries, and scope governance rules.
* **[03. Functional Requirements](file:///D:/CaseTracker/Backend/Documents/product/03_functional_requirements.md)**: Feature breakdown for Advocate Accounts, Clients, Cases, Court Hierarchy, eCourts integration, Hearings, and Documents.

---

### 2. [Architecture & Technical Design](file:///D:/CaseTracker/Backend/Documents/architecture/)
* **[01. System Architecture](file:///D:/CaseTracker/Backend/Documents/architecture/01_system_architecture.md)**: Clean Architecture layering, dependency rules, project boundaries, and request execution pipelines.
* **[02. Database Architecture](file:///D:/CaseTracker/Backend/Documents/architecture/02_database_architecture.md)**: Shared-table multi-tenant strategy, PostgreSQL configurations, and complete Mermaid Entity Relationship Diagram (ERD).
* **[03. Security & Compliance](file:///D:/CaseTracker/Backend/Documents/architecture/03_security_and_compliance.md)**: JWT token signing, PBKDF2/BCrypt password hashing, role authorization policies, and Indian legal data privacy standards.
* **Architecture Decision Records (ADRs)**:
  * [ADR-0001: Clean Architecture Layering](file:///D:/CaseTracker/Backend/Documents/architecture/adr/0001-clean-architecture-layering.md)
  * [ADR-0002: PostgreSQL Shared-Table Multi-Tenancy](file:///D:/CaseTracker/Backend/Documents/architecture/adr/0002-postgresql-shared-schema-multitenancy.md)
  * [ADR-0003: .NET MAUI Custom Shadcn Design System](file:///D:/CaseTracker/Backend/Documents/architecture/adr/0003-maui-shadcn-custom-design-system.md)

---

### 3. [Developer & Engineering Guides](file:///D:/CaseTracker/Backend/Documents/development/)
* **[01. Getting Started & Onboarding](file:///D:/CaseTracker/Backend/Documents/development/01_getting_started.md)**: Prerequisites, environment setup, database spin-up, and running both API & MAUI apps locally.
* **[02. Database Migrations](file:///D:/CaseTracker/Backend/Documents/development/02_database_migrations.md)**: EF Core migration commands, schema synchronization, and rollback procedures.
* **[03. Coding Standards](file:///D:/CaseTracker/Backend/Documents/development/03_coding_standards.md)**: C# code style, Clean Architecture dependency enforcement, DTO patterns, and error handling guidelines.

---

### 4. [API Reference & Contracts](file:///D:/CaseTracker/Backend/Documents/api/)
* **[Authentication & Token Handshake](file:///D:/CaseTracker/Backend/Documents/api/authentication_flow.md)**: Flow diagram for Registration, Login, JWT claims, and endpoint security contracts.

---

### 5. [Mobile Client Specifications](file:///D:/CaseTracker/Backend/Documents/mobile/)
* **[01. MAUI Architecture & UI Design System](file:///D:/CaseTracker/Backend/Documents/mobile/01_maui_architecture.md)**: MVVM pattern via CommunityToolkit.Mvvm, Shell routing, Shadcn custom controls (`ShadcnEntry`, `ShadcnButton`, `ShadcnCard`, `ShadcnMetricCard`, `ShadcnBadge`), zero platform underlines, and offline local cache.

---

### 6. [Changelog & History](file:///D:/CaseTracker/Backend/Documents/changelog/)
* **[CHANGELOG.md](file:///D:/CaseTracker/Backend/Documents/changelog/CHANGELOG.md)**: Chronological record of features, architectural refactors, and UI enhancements.
* **[Architectural & UI Changes by Balaji](file:///D:/CaseTracker/Backend/Documents/changelog/2026-09-05-balaji-architecture-design-system.md)**: Design system unification, validation, and Shell routing fix log.

---

## ✍️ Contribution Guidelines for Docs
1. **Docs-as-Code**: All documentation is maintained in Markdown (`.md`) alongside the code in Git.
2. **Diagrams as Code**: Use [Mermaid.js](https://mermaid.js.org/) code blocks (`mermaid`) for flowcharts, sequence diagrams, and ER diagrams.
3. **No Redundancies**: Maintain a single source of truth; do not duplicate functional rules across multiple documents.
4. **ADRs for Big Decisions**: Propose significant architectural changes using the ADR format before implementation.


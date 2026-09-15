# CaseTracker — Engineering & Architecture Documentation Center

Welcome to the **CaseTracker** documentation center. This repository houses all architectural, product, engineering, API, and DevOps specifications following the **Docs-as-Code** philosophy.

---

## 🧭 Documentation Map & Index

```text
Documents/
├── architecture/         # Clean Architecture layering, request pipelines, security & ADRs
│   ├── 01_system_architecture.md
│   ├── 02_security_and_compliance.md
│   └── adr/
├── database/             # Relational design, ER diagrams, data dictionary & cloud specs
│   ├── 01_database_design_and_erd.md
│   └── 02_data_dictionary.md
├── api/                  # REST contracts, endpoint reference, authentication & error handling
│   ├── 01_api_reference.md
│   ├── 02_error_handling.md
│   └── authentication_flow.md
├── ci/                   # CI/CD pipelines, GitHub Actions, MSDeploy & troubleshooting
│   ├── 01_ci_cd_pipeline.md
│   ├── 02_troubleshooting.md
│   └── CONTRIBUTING.md
├── development/          # Local machine onboarding, migrations & coding standards
│   ├── 01_getting_started.md
│   ├── 02_database_migrations.md
│   └── 03_coding_standards.md
├── product/              # Business vision, user personas & functional requirements
│   ├── 01_product_vision.md
│   ├── 02_project_scope.md
│   └── 03_functional_requirements.md
└── ecourt/               # eCourts CIS API specifications & judicial synchronization
    ├── README.md
    └── 01_ARCHITECTURE_AND_AUTH.md
```

---

## 📂 Master Table of Contents

### 1. 🏛️ Architecture & System Design
* **[01. System Architecture & Layering](./architecture/01_system_architecture.md)**: Clean Architecture layering, dependency rules, project boundaries, and end-to-end request execution lifecycle with Mermaid sequence diagrams.
* **[02. Security & Compliance](./architecture/02_security_and_compliance.md)**: JWT token signing, PBKDF2/BCrypt hashing, role authorization policies, and Indian legal data privacy standards.
* **Architecture Decision Records (ADRs)**:
  * **[ADR-0001: Clean Architecture Layering](./architecture/adr/0001-clean-architecture-layering.md)**
  * **[ADR-0002: PostgreSQL Shared-Table Multi-Tenancy](./architecture/adr/0002-postgresql-shared-schema-multitenancy.md)**

---

### 2. 🗄️ Database & Persistence
* **[01. Database Design & ERD](./database/01_database_design_and_erd.md)**: PostgreSQL 16+ on Neon Cloud, shared-table multi-tenancy, complete Mermaid ERD, and relationship cascade rules.
* **[02. Data Dictionary](./database/02_data_dictionary.md)**: Column-by-column specifications, constraints, data types, nullability, and default values for all 6 tables (`users`, `lawyers`, `law_firms`, `roles`, `user_role`, `user_law_firms`).

---

### 3. 📡 API Specifications & Security
* **[01. REST API Reference & Catalog](./api/01_api_reference.md)**: Exhaustive endpoint documentation for Auth, Users, Lawyers, Law Firms, Roles, and Version health checks.
* **[02. Global Error Handling Standards](./api/02_error_handling.md)**: RFC 7807 problem details, `GlobalExceptionHandlerMiddleware`, and standardized JSON envelopes.
* **[03. Authentication & Token Handshake](./api/authentication_flow.md)**: Detailed sequence diagrams for Registration, Login, Token validation, and JWT payload claim structures.

---

### 4. 🚀 CI / CD & Cloud Deployment
* **[01. CI/CD Automated Deployment Guide](./ci/01_ci_cd_pipeline.md)**: Full GitHub Actions pipeline guide, `ci.yml` PR quality gates, `cd.yml` auto-deploy to MonsterASP.net IIS Web Server, MSDeploy commands, secret setup, and `.env` fallback.
* **[02. Deployment Troubleshooting Guide](./ci/02_troubleshooting.md)**: Step-by-step diagnostic guide for 401 Unauthorized, argument quoting, locked DLLs (`AppOffline`), and IIS configuration.
* **[03. Contributing & Branching Guidelines](./ci/CONTRIBUTING.md)**: Git branching model (`dev/soorya` &rarr; `master`), PR rules, and commit message conventions.

---

### 5. 💻 Developer & Engineering Guides
* **[01. Getting Started & Onboarding](./development/01_getting_started.md)**: Prerequisites, local setup, database spin-up, and running the Web API locally.
* **[02. Database Migrations](./development/02_database_migrations.md)**: EF Core migration commands, schema synchronization, and rollback procedures.
* **[03. Coding Standards](./development/03_coding_standards.md)**: C# code conventions, Clean Architecture dependency enforcement, DTO patterns, and error handling guidelines.

---

### 6. 📜 Product & Judicial Integrations
* **[01. Product Vision](./product/01_product_vision.md)**: Market opportunity, target user personas (Advocates, Firms, Staff), and geographic expansion.
* **[02. Project Scope](./product/02_project_scope.md)**: Delivery phases, boundaries, and scope governance rules.
* **[03. Functional Requirements](./product/03_functional_requirements.md)**: Core advocate, case matter, court hierarchy, and cause-list features.
* **[04. eCourts Integration Specs](./ecourt/README.md)**: National eCourts API contracts, CNR number format, cause-list scraping, and case status sync.

---

## ✍️ Documentation Conventions
1. **Docs-as-Code**: All documentation is maintained in Markdown (`.md`) alongside source code in Git.
2. **Diagrams as Code**: Flowcharts, sequence diagrams, and entity relationships must be written in standard [Mermaid.js](https://mermaid.js.org/) code blocks.
3. **Relative Links**: Always use relative paths for internal documentation links so they render properly both on GitHub and offline in editors.

# Case Tracker — Database Architecture


# Introduction

Case Tracker will use **PostgreSQL** as the primary relational database.

The application will use a **shared-table architecture**.

All application tenants will use the same database schema and shared tables.

The initial database structure will follow:

```text
PostgreSQL
└── public
    ├── lawyers
    ├── clients
    ├── cases
    ├── hearings
    ├── case_documents
    └── ...
```

It also provides a simpler foundation for:

* Database migrations
* Index management
* Reporting
* Maintenance
* Scaling
* Monitoring
* Backup and recovery

Stronger tenant isolation, including PostgreSQL Row-Level Security, can be evaluated as the system matures.

---
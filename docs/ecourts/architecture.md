# eCourts Integration Architecture

## Architectural Principles
1. **Cost-Sensitive Gateway**: Every API call to eCourts India Partner API (`https://webapi.ecourtsindia.com`) incurs credit consumption for case lookups and dockets. Master data queries (states, districts, court complexes) are 0 credits.
2. **Local Database as Source of Truth**: User-facing web and mobile applications NEVER query eCourts synchronously on page loads or detail views. All read traffic hits PostgreSQL.
3. **Asynchronous Background Ingestion**: Data enters CaseTracker via scheduled workers (`CourtMasterSyncWorker`, `CaseStatusSyncWorker`) or audited administrator-triggered jobs.

```mermaid
graph TD
    subgraph Client Application
        React[React / Mobile Frontend]
    end

    subgraph CaseTracker Web API
        API[REST API Layer]
        Auth[JWT Authorization]
    end

    subgraph CaseTracker Core
        CaseSvc[CaseService]
        MasterSvc[CourtMasterService]
        SyncSvc[ECourtSyncService]
    end

    subgraph Data Store
        PG[(PostgreSQL Database)]
        AuditLog[(ecourt_api_logs)]
    end

    subgraph Background Workers
        WorkerMaster[CourtMasterSyncWorker - Monthly]
        WorkerCases[CaseStatusSyncWorker - Daily Batch]
    end

    subgraph External Partner API
        ECourtAPI[eCourts India Partner API]
    end

    React -->|User Reads & Writes| API
    API --> Auth
    Auth --> CaseSvc
    CaseSvc --> PG

    WorkerMaster --> MasterSvc
    MasterSvc -->|GET Structure - 0 credits| ECourtAPI
    MasterSvc -->|Upsert States/Districts/Courts| PG

    WorkerCases --> SyncSvc
    SyncSvc -->|Fetch Eligible Cases| PG
    SyncSvc -->|GET Case By CNR - 1 Credit| ECourtAPI
    SyncSvc -->|Update Docket & Hearings| PG
    SyncSvc -->|Record Credit Audit| AuditLog
```

## Partner API Integration Details
* **Base URL**: `https://webapi.ecourtsindia.com`
* **Authorization Header**: `X-API-KEY: {ECourts:ApiKey}`
* **Rate Limits**: 10 requests per second burst, backed by HTTP 429 response handling with `Retry-After` header extraction and exponential backoff.
* **Timeout**: Configured to 30 seconds with transient fault handling.

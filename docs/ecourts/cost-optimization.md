# eCourts Integration Cost-Optimization Strategy

## The Challenge
eCourts India Partner APIs charge credits for docket queries and case tracking. Without strict architectural guardrails, naive polling or user-initiated synchronous calls will quickly exhaust API balances and incur unnecessary charges.

## Cost-Control Mechanisms Implemented

### 1. Local Database Caching First
* **Rule**: User requests NEVER call eCourts APIs directly.
* User browsing case lists, viewing hearings, checking past orders, or printing court cause lists hit local PostgreSQL indices only.

### 2. Configurable Sync Intervals & Checkpoints
* Only cases that have exceeded their `CaseSyncIntervalHours` (default: 24 hours) are eligible for background sync.
* Disposed or archived cases (`Status = 'Archived'`) are excluded from recurring syncs.

### 3. Controlled Batch Sizing (`MaxRequestsPerRun`)
* Background jobs operate with strict batch boundaries (default: 25 cases per job run).
* Prevents unbounded loops from draining credits if a large caseload is imported.

### 4. Exponential Backoff & Circuit Breaking
* If an HTTP 429 (Too Many Requests) or HTTP 503 is returned, the client parses the `Retry-After` header and sleeps rather than immediately hammering the gateway.
* If a CNR lookup returns 404 (invalid CNR or court system error), the failure is recorded in `cases.sync_error` and the case is skipped until manually corrected.

### 5. Credit Consumption Auditing (`ecourt_api_logs`)
* Every outbound call records:
  * HTTP Endpoint & Method
  * Target CNR or entity
  * Status code & latency (ms)
  * Credits consumed (0 for structure, 1 for case detail)
  * Error message if failed
* Administrators can inspect daily/weekly credit burn via `GET /api/ecourt-sync/usage-summary`.

### 6. Idempotent Upserts
* Repeated execution of sync routines does not produce duplicate hearings or case logs.

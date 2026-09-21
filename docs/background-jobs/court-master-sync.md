# Court Master Sync Worker

## Purpose
Maintains up-to-date master data for States, Districts, Court Complexes, and Court Benches for Tamil Nadu (`TN`) and Puducherry (`PY`).

## Key Characteristics
* **Class**: `CaseTrackerInfrastructure.BackgroundWorkers.CourtMasterSyncWorker`
* **Default Frequency**: Every 30 days (`CourtMasterSyncIntervalDays: 30`).
* **Initial Delay on Startup**: 2 minutes (allows database migrations and web server to warm up first).
* **Cost Impact**: **0 eCourts credits** (uses free `causelist/court-structure/*` endpoints).

## Workflow
1. Worker awakes and creates an `IServiceScope`.
2. Resolves `ICourtMasterService` and configuration options.
3. Iterates over target states (`TN`, `PY`):
   * Fetches district list from eCourts.
   * Upserts districts into `districts` table.
   * Fetches court complexes for each district.
   * Upserts complexes into `court_complexes` table.
   * Fetches court halls/benches for each complex.
   * Upserts courts into `courts` table.
4. Logs structured summary:
   ```text
   [CourtMasterSyncWorker] Completed master sync for TN: 1 state, 32 districts, 180 complexes, 650 courts processed in 4520ms.
   ```
5. Sleeps until the next monthly interval.

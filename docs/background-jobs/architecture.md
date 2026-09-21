# Background Jobs Architecture

## Overview
CaseTracker employs dedicated hosted background services implemented via ASP.NET Core `Microsoft.Extensions.Hosting.BackgroundService`. These workers run in decoupled execution contexts, maintaining isolated scopes and resilient error handling.

## Deployment Models Supported

### 1. In-Process Hosted Services (Current Default)
* Both `CourtMasterSyncWorker` and `CaseStatusSyncWorker` run inside the `CaseTrackerInfrastructure` assembly.
* They register in DI via `services.AddHostedService<CourtMasterSyncWorker>()` and `services.AddHostedService<CaseStatusSyncWorker>()`.
* Zero extra infrastructure cost; starts automatically alongside the web backend in Docker containers.

### 2. Standalone Azure Functions App (Optional Separate Deployment)
* For teams wishing to run background synchronization completely external to the Web API:
  * `CourtMasterSyncFunction`: Timer trigger `0 0 1 * *` (Runs on the 1st of every month at midnight UTC).
  * `CaseStatusSyncFunction`: Timer trigger `0 0 2 * * *` (Runs daily at 2:00 AM UTC).
* The business services (`ICourtMasterService`, `IECourtSyncService`) are pure and can be invoked from either an ASP.NET `BackgroundService` or an Azure Function Timer Trigger without architectural redesign.

## Failure Isolation Principle
* A failure in synchronizing one court or one case NEVER terminates the background job.
* The loop catches per-item exceptions, logs the error against that record (`SyncError`), and proceeds immediately to the next item in the batch.

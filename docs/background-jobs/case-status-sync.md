# Case Status Sync Worker

## Purpose
Performs incremental background synchronization of case statuses, hearing schedules, and docket changes for registered cases across all law firms.

## Key Characteristics
* **Class**: `CaseTrackerInfrastructure.BackgroundWorkers.CaseStatusSyncWorker`
* **Default Frequency**: Every 24 hours (`CaseSyncIntervalHours: 24`).
* **Batch Size**: 25 cases per iteration (`BatchSize: 25`).
* **Cost Impact**: Controlled credit consumption (1 credit per synced case detail).

## Selection Logic
```sql
SELECT * FROM cases
WHERE status = 'Active'
  AND cnr_number IS NOT NULL
  AND (last_synced_at IS NULL OR last_synced_at <= NOW() - INTERVAL '24 hours')
ORDER BY last_synced_at ASC NULLS FIRST
LIMIT 25;
```

## Resilience & Observability
* If eCourts is unreachable, returns 429, or times out, the worker updates `cases.sync_status = 'Failed'` and `cases.sync_error = {error_message}`, records the attempt in `ecourt_api_logs`, and moves immediately to the next case.
* Structured execution summary logged at the end of each run:
  ```text
  Job: CaseStatusSync
  Started: 2026-09-21T03:00:00Z
  Completed: 2026-09-21T03:00:15Z
  Selected: 25
  Synced: 23
  Skipped: 1
  Failed: 1
  Credits Deducted: 23
  Duration: 15420ms
  ```

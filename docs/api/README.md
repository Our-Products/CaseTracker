# CaseTracker API Documentation

## Overview
All CaseTracker APIs follow RESTful conventions, adhere to standard HTTP status codes, require JWT Bearer authentication (except public auth endpoints), and return the standardized envelope `ApiResponse<T>`.

### Base Standard Envelope
```json
{
  "success": true,
  "message": "Operation completed successfully.",
  "data": { ... },
  "errors": [],
  "statusCode": 200
}
```

## Controllers & Endpoints

### 1. CaseController (`/api/cases`)
Secured by `[Authorize]`. Enforces law firm tenancy and role-based permissions.
* `GET /api/cases`
  * Query parameters: `?lawFirmId={guid}` (optional for SuperAdmin, restricted to own firm for others).
  * Returns: `List<CaseDto>`.
* `GET /api/cases/{id}`
  * Returns: `CaseDetailDto` with linked clients, hearings, and orders.
* `GET /api/cases/cnr/{cnrNumber}`
  * Returns: `CaseDto` matched by CNR.
* `POST /api/cases`
  * Body: `CreateCaseRequest` (`caseNumber`, `caseType`, `caseTitle`, `caseStage`, `courtId`, `cnrNumber`, `filingDate`, etc.).
  * Validates CNR uniqueness and assigns creating user & law firm.
* `PUT /api/cases/{id}`
  * Body: `UpdateCaseRequest`.
* `DELETE /api/cases/{id}`
  * Soft-deletes case record (`Status = 'Archived'`).

### 2. ClientController (`/api/clients`)
* `GET /api/clients`
  * Query parameters: `?lawFirmId={guid}`.
* `GET /api/clients/{id}`
  * Returns: `ClientDetailDto` with active cases.
* `POST /api/clients`
  * Body: `CreateClientRequest` (`fullName`, `email`, `phone`, `clientType`, `address`, `city`, `state`, etc.).
* `PUT /api/clients/{id}`
  * Body: `UpdateClientRequest`.
* `DELETE /api/clients/{id}`
  * Soft-deletes client record.

### 3. HearingController (`/api/hearings`)
* `GET /api/hearings/case/{caseId}`
  * Returns all hearings for the given case ordered by hearing date descending.
* `GET /api/hearings/{id}`
  * Returns: `HearingDto`.
* `POST /api/hearings`
  * Body: `CreateHearingRequest` (`caseId`, `hearingDate`, `judgeName`, `courtNumber`, `purposeOfHearing`, `businessOnDate`, etc.).
* `PUT /api/hearings/{id}`
  * Body: `UpdateHearingRequest`.
* `DELETE /api/hearings/{id}`
  * Removes hearing record.

### 4. CourtController (`/api/courts`)
* `GET /api/courts/states`
  * Returns all synchronized states (TN, PY, etc.).
* `GET /api/courts/districts/{stateId}`
  * Returns districts within the state.
* `GET /api/courts/complexes/{districtId}`
  * Returns court complexes within the district.
* `GET /api/courts/by-complex/{complexId}`
  * Returns individual court halls/benches in the complex.
* `POST /api/courts/sync-master/{stateCode}`
  * `[Authorize(Roles = "SuperAdmin")]`
  * Triggers an on-demand master synchronization for a state (e.g. `TN` or `PY`).

### 5. ECourtSyncController (`/api/ecourt-sync`)
Secured by `[Authorize(Roles = "SuperAdmin,Admin")]`.
* `POST /api/ecourt-sync/trigger-case-status-sync`
  * Body: `TriggerCaseSyncRequest` (`batchSizeOverride`).
  * Triggers immediate batch synchronization of pending/due cases.
* `POST /api/ecourt-sync/sync-case/{caseId}`
  * On-demand single case synchronization (respects rate limiting and records API credit usage).
* `GET /api/ecourt-sync/usage-summary`
  * Query parameters: `?from=2026-09-01&to=2026-09-30`.
  * Returns total API requests, credits deducted, success rate, and latency.

### 6. Auth & User Controllers (`/api/auth`, `/api/users`, `/api/me`)
* Standard authentication, token refresh, and user profile management.

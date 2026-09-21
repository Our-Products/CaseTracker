# eCourts Synchronization Strategy

## Two-Tier Synchronization Strategy

### Tier 1: Master Court Hierarchy (TN + PY)
* **Scope**: States, Districts, Court Complexes, and Court Benches for **Tamil Nadu (`TN`)** and **Puducherry (`PY`)**.
* **API Endpoints**:
  * `GET /api/partner/causelist/court-structure/states`
  * `GET /api/partner/causelist/court-structure/districts?state={stateCode}`
  * `GET /api/partner/causelist/court-structure/complexes?state={stateCode}&district={distCode}`
  * `GET /api/partner/causelist/court-structure/courts?state={stateCode}&district={distCode}&complex={compCode}`
* **Credit Cost**: **0 Credits** (Free structure metadata).
* **Cadence**: Synchronized monthly or triggered manually on administrative demand.
* **Idempotent Upsert Logic**:
  * Records are identified by external composite codes (`StateCode`, `DistrictCode`, `ComplexCode`, `CourtCode`).
  * If a record exists, its name, judge, or bench metadata is updated without altering primary key UUIDs or breaking foreign key relations.
  * Deleted or renamed courts remain tracked with historical referential integrity.

---

### Tier 2: Case Status & Docket History
* **Scope**: Cases with valid 16-character CNR numbers (`CnrNumber`).
* **API Endpoints**:
  * `GET /api/partner/case/{cnr}` (or `POST /api/partner/causelist/cnr/batch` when supported).
* **Credit Cost**: **1 Credit per Case Detail**.
* **Eligibility Predicate**:
  ```csharp
  c.Status == "Active" &&
  !string.IsNullOrWhiteSpace(c.CnrNumber) &&
  (c.LastSyncedAt == null || c.LastSyncedAt <= cutoffTime)
  ```
  Where `cutoffTime = DateTimeOffset.UtcNow.AddHours(-SyncIntervalHours)`.
* **Batch Processing**:
  * Cases are processed in controlled batches (default: 25 cases per run).
  * Controlled concurrency prevents burst throttling and runaway credit depletion.
* **Data Synced**:
  * Case status (`Pending`, `Disposed`, `Hearing`, `Dismissed`, etc.).
  * Case number, registration number, filing dates.
  * Upcoming and historical hearing dates (`HistoryOfCaseHearings`).
  * Hearing judge, purpose of listing, and business recorded on date.
  * Sync timestamps: `LastSyncedAt`, `LastSuccessfulSyncAt`, `LastSyncAttemptAt`, `SyncStatus`, `SyncError`.

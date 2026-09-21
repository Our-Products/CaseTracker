# CaseTracker — Cases, Clients, Hearings & eCourt Integration Specification

> **Document Version:** 1.0  
> **Status:** Approved Production Specification  
> **Parent Documentation:** [Documentation Center](../README.md) | [Database Architecture](../database/01_database_design_and_erd.md) | [eCourt API Specifications](../ecourt/01_ARCHITECTURE_AND_AUTH.md)

---

## 1. Executive Summary

This document specifies the operational architecture and technical workflows connecting **Clients**, **Legal Matters (Cases)**, **Court Hearings / Daily Boards**, **eCourts CIS API Synchronization**, and **Digital Case File Vaults**.

```text
  [Law Firm / Advocate]
           │
     creates / retains
           │
           ▼
       [Clients] ────── linked to ──────► [Cases] ◄────── tracked via ────── [eCourts CIS API]
  (Individual / Corporate)              (CNR Number)                       (16-char CNR Sync)
                                             │                                     │
                                    ┌────────┴────────┐                   ┌────────┴────────┐
                                    ▼                 ▼                   ▼                 ▼
                             [Case Hearings]   [Case Documents]    [Daily Cause List] [Case Orders]
                            (Daily Proceedings) (Petitions, FIR)   (Item #, Hall, Coram) (PDFs & Markdown)
```

---

## 2. Client Management Lifecycle

### 2.1 Entity Model & Classification
Clients are modeled under `clients` and scoped either to a multi-tenant `law_firm_id` or to an independent advocate (`created_by`).
* **`client_type`**:
  * `Individual`: Natural persons represented in personal litigation, criminal bail, civil disputes, matrimonial matters.
  * `Corporate`: Companies, LLPs, banks, and institutions. Requires `contact_person` (authorized representative) and optional `gst_number`.
  * `Partnership` / `Trust` / `Government`: Entity-specific classification.

### 2.2 Multi-Party Representation (`case_clients`)
In Indian court litigation, advocates frequently represent multiple petitioners or defend multiple respondents. A junction table `case_clients` bridges this relationship:
* **`party_type`**: `Petitioner`, `Respondent`, `Appellant`, `Defendant`, `Accused`, `Complainant`.
* **`party_sequence`**: Serial order (e.g. *1st Petitioner*, *2nd Respondent*).
* **`is_primary`**: Identifies the lead client for invoice generation, SMS updates, and official communications.

---

## 3. Case Matter Architecture

### 3.1 Identification & Titling
* **CNR Number (16 characters)**: The gold standard eCourts unique alphanumeric identifier (e.g., `DLND020047882015` or `TNCH010012342024`).
  * Characters 1–2: State Code (e.g. `TN`, `DL`, `MH`).
  * Characters 3–6: Court Establishment Code.
  * Characters 7–12: Case Serial Number.
  * Characters 13–16: Year of Filing.
* **Case Number**: Court register designation (e.g. `WP/12345/2024`, `CRL OP/987/2024`, `OS/45/2023`).
* **Synthesized Title**: Generated automatically by the application from party names:
  $$\text{Title} = \text{Petitioner(s)} + \text{ " vs " } + \text{Respondent(s)}$$
  *(e.g., "M. Sundaram & Ors vs State of Tamil Nadu & Anr")*

### 3.2 Stage Tracking & Statutory Offenses
* **`case_stage`**: Tracks movement through the court lifecycle: `Admission` $\rightarrow$ `Notice to Respondent` $\rightarrow$ `Counter / Reply Affidavit` $\rightarrow$ `Framing Issues / Charges` $\rightarrow$ `Evidence & Cross-Examination` $\rightarrow$ `Final Arguments` $\rightarrow$ `Orders / Judgment Reserved` $\rightarrow$ `Disposed`.
* **`acts_sections`**: Plain-text list of statutes (e.g., `Section 302 IPC, Section 138 Negotiable Instruments Act, Order 39 Rule 1 CPC`).
* **Criminal Docketing**: `police_station`, `fir_number`, and `fir_year` allow tracking FIR matters from police station to Magistrate court and High Court bail petitions.

---

## 4. Daily Cause Lists & Hearing Management

### 4.1 Daily Court Board Workflow
Every morning before 10:00 AM, advocate chambers monitor the court cause list to organize appearances:
1. **`hearing_date`**: The date when the matter is listed on the board.
2. **`item_number`**: Serial number on the daily bench list (e.g., *Item No. 14* in the Urgent List, *Item No. 102* in the Regular List).
3. **`court_hall` & `judge_name`**: Courtroom allocation (e.g., *Court Hall 12, Hon'ble Mr. Justice K. Murali Shankar*).
4. **`purpose_of_hearing`**: Bench purpose: *For Admission, For Filing Counter, For Final Hearing, For Pronouncement of Orders*.

### 4.2 Proceedings & Adjournments
Upon conclusion of the call work or argument:
* **`business_on_date`**: Oral proceedings and directions recorded by the bench (e.g., *"Petitioner counsel argued for interim injunction. Notice ordered returnable by 2 weeks. Private notice permitted."*).
* **`next_hearing_date`**: The adjourned listing date given by the bench.
* **`hearing_status`**: Current state (`Scheduled`, `Adjourned`, `Passed Over`, `Disposed`, `Order Reserved`).
* **`daily_order_summary`**: Confidential internal chamber notes for advocate briefings.

---

## 5. eCourts CIS Synchronization Architecture

### 5.1 Synchronization Modes
```text
┌────────────────────────────────────────────────────────┐
│               eCourts Sync Execution Modes              │
├────────────────────────────────────────────────────────┤
│ 1. Scheduled Background Worker (Cron)                   │
│    • 05:00 AM IST: Ingest daily published cause lists. │
│    • 06:30 PM IST: Harvest daily business & order PDFs. │
│ 2. Real-Time On-Demand Pull                            │
│    • Advocate presses "Sync Docket" in Mobile App.     │
│ 3. Automated Webhook Trigger                           │
│    • Ingestion upon case admission or order dispatch.  │
└────────────────────────────────────────────────────────┘
```

### 5.2 Docket Payload Ingestion Pipeline
When querying `GET /api/partner/case/{cnr}`:
1. **Status & History Ingestion**:
   * Updates `cases.case_status` (`Pending`, `Disposed`, `Stayed`).
   * Iterates through `courtCaseData.historyOfCaseHearings` and upserts records into `case_hearings`.
2. **Daily Proceedings Sync**:
   * Maps `courtCaseData.businessOnDateEntries` to `case_hearings.business_on_date`.
3. **Court Orders Ingestion (`case_orders`)**:
   * Scans `courtCaseData.judgmentOrders` and `courtCaseData.interimOrders`.
   * Triggers download of certified order PDF via `GET /api/partner/case/{cnr}/order/{filename}`.
   * Stores encrypted PDF in cloud object storage (`pdf_storage_path`).
   * Extracts and caches `order_markdown_content` for instant full-text search and offline reading.
4. **Telemetry & Offline Fallback (`ecourt_sync_logs`)**:
   * Logs execution status, count of new hearings, and count of downloaded orders.
   * Stores full raw JSON response in `ecourt_sync_logs.raw_payload` (JSONB) so advocates can access complete docket data even in offline court basements.

---

## 6. Case File Vault & Document Management (`case_documents`)

Decouples file metadata in PostgreSQL from encrypted binary blobs in object storage:
* **Categories**: `Petition`, `Vakalatnama`, `Counter Affidavit`, `Rejoinder`, `Certified Order`, `FIR`, `Evidence / Documents List`, `Written Submissions`.
* **Attachment Flexibility**: Documents can belong directly to a `case_id` or be tagged to a specific `hearing_id` (e.g., an exhibit or affidavit filed on a particular date).
* **Storage**: Uploaded via pre-signed S3 URLs with encryption at rest (`AES-256`).

---

## 7. API Blueprint for Mobile & Web Applications

| HTTP Method | Endpoint | Description | Query / Body Parameters |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/clients` | List clients with search & pagination | `?search=Sundaram&clientType=Individual&page=1` |
| `POST` | `/api/clients` | Create new client record | Client JSON payload |
| `GET` | `/api/clients/{id}/cases` | Get all cases for a client | `clientId` GUID |
| `GET` | `/api/cases` | Search & filter case matters | `?courtId={guid}&status=Pending&search=WP` |
| `POST` | `/api/cases` | Register new case matter | Case JSON payload |
| `GET` | `/api/cases/{id}` | Full case docket with hearings & orders | `caseId` GUID |
| `POST` | `/api/cases/{id}/sync` | Trigger on-demand eCourts sync | `caseId` GUID |
| `GET` | `/api/hearings/daily-board` | Today's cause list board for chamber | `?date=2026-09-21&courtId={guid}` |
| `POST` | `/api/hearings` | Log court hearing proceedings | Hearing JSON payload |
| `GET` | `/api/cases/{id}/orders` | Retrieve interim & final orders | `caseId` GUID |
| `GET` | `/api/cases/{id}/documents` | Document vault file browser | `caseId` GUID |
| `POST` | `/api/cases/{id}/documents` | Upload case document | Multipart form data or pre-signed URL |

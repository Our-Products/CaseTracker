# CaseTracker — Database Data Dictionary

> **Document Version:** 2.0  
> **Database:** PostgreSQL 16+  
> **Parent Documentation:** [Documentation Center](../README.md) | [Database Architecture](./01_database_design_and_erd.md)

---

## Table of Contents
1. [`users`](#1-table-users)
2. [`lawyers`](#2-table-lawyers)
3. [`law_firms`](#3-table-law_firms)
4. [`roles`](#4-table-roles)
5. [`user_role`](#5-table-user_role)
6. [`user_law_firms`](#6-table-user_law_firms)
7. [`clients`](#7-table-clients)
8. [`case_clients`](#8-table-case_clients)
9. [`cases`](#9-table-cases)
10. [`case_lawyers`](#10-table-case_lawyers)
11. [`case_hearings`](#11-table-case_hearings)
12. [`ecourt_sync_logs`](#12-table-ecourt_sync_logs)
13. [`case_orders`](#13-table-case_orders)
14. [`case_documents`](#14-table-case_documents)

---

## 1. Table: `users`
Represents authentication and primary account credentials for all system participants.

| Column | Data Type | Nullable | Default | Constraints | Description |
| :--- | :--- | :---: | :--- | :--- | :--- |
| `user_id` | UUID | No | `gen_random_uuid()` | `PK` | Unique internal identifier for the user account |
| `mobile_number` | VARCHAR(20) | No | None | `UK`, `Index` | Primary login identifier (e.g. `+919876543210`) |
| `email` | VARCHAR(255) | No | None | `UK`, `Index` | Verified email address for notifications and login |
| `password` | TEXT | No | None | None | PBKDF2/BCrypt salted password hash |
| `status` | VARCHAR(20) | No | `'Active'` | None | Account status (`Active`, `Suspended`, `PendingVerification`) |
| `created_at` | TIMESTAMPTZ | No | `NOW()` | None | Timestamp of account creation in UTC |
| `updated_at` | TIMESTAMPTZ | No | `NOW()` | None | Timestamp of last account update in UTC |
| `created_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | User who created this record |
| `updated_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | User who last updated this record |

---

## 2. Table: `lawyers`
Stores professional Bar Council credentials and biographical details for legal practitioners.

| Column | Data Type | Nullable | Default | Constraints | Description |
| :--- | :--- | :---: | :--- | :--- | :--- |
| `lawyer_id` | UUID | No | `gen_random_uuid()` | `PK` | Unique internal identifier for the lawyer profile |
| `user_id` | UUID | No | None | `UK`, `FK` &rarr; `users.user_id` | 1-to-1 linkage to the login account |
| `law_firm_id` | UUID | Yes | None | `FK` &rarr; `law_firms.law_firm_id` | Optional association with a law firm / chamber |
| `full_name` | VARCHAR(200) | No | None | None | Legal full name as listed on the Bar Council roll |
| `bar_council_id` | VARCHAR(100) | Yes | None | `Index` | Bar Council Enrollment ID (e.g. `TN/1042/2018`) |
| `bar_council_name`| VARCHAR(200) | Yes | None | None | State Bar Council authority |
| `enrollment_date` | DATE | Yes | None | None | Date of enrollment to practice law |
| `status` | VARCHAR(20) | No | `'Active'` | None | Professional status (`Active`, `Retired`, `Suspended`) |
| `created_at` | TIMESTAMPTZ | No | `NOW()` | None | Timestamp of creation in UTC |
| `updated_at` | TIMESTAMPTZ | No | `NOW()` | None | Timestamp of last modification in UTC |
| `created_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | Audit creator user |
| `updated_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | Audit updater user |

---

## 3. Table: `law_firms`
Represents legal partnerships, LLPs, and advocate chambers.

| Column | Data Type | Nullable | Default | Constraints | Description |
| :--- | :--- | :---: | :--- | :--- | :--- |
| `law_firm_id` | UUID | No | `gen_random_uuid()` | `PK` | Unique internal identifier for the organization |
| `firm_name` | VARCHAR(255) | No | None | None | Registered legal name of the firm or chambers |
| `registration_number` | VARCHAR(255) | No | None | `UK` | Official government or Bar Council registration identifier |
| `address_line1` | VARCHAR(255) | Yes | None | None | Primary physical address line (office/chamber) |
| `address_line2` | VARCHAR(255) | Yes | None | None | Secondary physical address line (suite/floor) |
| `city` | VARCHAR(100) | Yes | None | None | City / Municipality |
| `district` | VARCHAR(100) | Yes | None | None | Judicial district |
| `state` | VARCHAR(100) | Yes | None | None | State or Union Territory |
| `pincode` | INTEGER | Yes | None | None | 6-digit postal code |
| `status` | VARCHAR(20) | No | `'Active'` | None | Operational status (`Active`, `Inactive`, `Dissolved`) |
| `created_at` | TIMESTAMPTZ | No | `NOW()` | None | Timestamp of creation in UTC |
| `updated_at` | TIMESTAMPTZ | No | `NOW()` | None | Timestamp of last modification in UTC |
| `created_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | Audit creator user |
| `updated_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | Audit updater user |

---

## 4. Table: `roles`
System-defined permission and authority levels.

| Column | Data Type | Nullable | Default | Constraints | Description |
| :--- | :--- | :---: | :--- | :--- | :--- |
| `role_id` | VARCHAR(20) | No | None | `PK` | Standardized role code (`R001`, `R002`, `R003`) |
| `role_name` | VARCHAR(50) | No | None | `UK` | Human-readable role name (`Lawyer`, `Staff`, `Admin`) |
| `status` | VARCHAR(20) | No | `'Active'` | None | Availability status (`Active`, `Disabled`) |
| `created_at` | TIMESTAMPTZ | No | `NOW()` | None | Timestamp of creation in UTC |
| `updated_at` | TIMESTAMPTZ | No | `NOW()` | None | Timestamp of last modification in UTC |

---

## 5. Table: `user_role`
Junction table managing Many-to-Many assignments between user accounts and system roles.

| Column | Data Type | Nullable | Default | Constraints | Description |
| :--- | :--- | :---: | :--- | :--- | :--- |
| `user_id` | UUID | No | None | `PK`, `FK` &rarr; `users.user_id` | Assigned user account |
| `role_id` | VARCHAR(20) | No | None | `PK`, `FK` &rarr; `roles.role_id` | Assigned system role |
| `created_at` | TIMESTAMPTZ | No | `NOW()` | None | Timestamp when role was granted |
| `updated_at` | TIMESTAMPTZ | No | `NOW()` | None | Timestamp when assignment was updated |

---

## 6. Table: `user_law_firms`
Junction table tracking membership and tenure of advocates and staff within law firms.

| Column | Data Type | Nullable | Default | Constraints | Description |
| :--- | :--- | :---: | :--- | :--- | :--- |
| `user_id` | UUID | No | None | `PK`, `FK` &rarr; `users.user_id` | Member user account |
| `law_firm_id` | UUID | No | None | `PK`, `FK` &rarr; `law_firms.law_firm_id`| Associated law firm |
| `joined_at` | TIMESTAMPTZ | No | `NOW()` | None | Date and time joined the organization |
| `status` | VARCHAR(20) | No | `'Active'` | None | Membership status (`Active`, `Inactive`, `Suspended`) |
| `created_at` | TIMESTAMPTZ | No | `NOW()` | None | Timestamp of membership creation |
| `updated_at` | TIMESTAMPTZ | No | `NOW()` | None | Timestamp of membership update |

---

## 7. Table: `clients`
Master directory of legal clients (individual litigants, corporations, trusts, and government bodies).

| Column | Data Type | Nullable | Default | Constraints | Description |
| :--- | :--- | :---: | :--- | :--- | :--- |
| `client_id` | UUID | No | `gen_random_uuid()` | `PK` | Unique internal client identifier |
| `law_firm_id` | UUID | Yes | None | `FK` &rarr; `law_firms.law_firm_id` | Law firm tenant (null for solo advocate clients) |
| `client_type` | VARCHAR(20) | No | `'Individual'` | None | `Individual`, `Corporate`, `Partnership`, `Trust`, `Government` |
| `full_name` | VARCHAR(200) | No | None | `Index` | Legal client name or registered entity name |
| `primary_phone` | VARCHAR(20) | No | None | `Index` | Primary mobile number for automated SMS notifications |
| `secondary_phone` | VARCHAR(20) | Yes | None | None | Alternate emergency or landline contact |
| `email` | VARCHAR(255) | Yes | None | `Index` | Contact email for cause-list and hearing reports |
| `address_line1` | VARCHAR(255) | Yes | None | None | Physical office or residential street address |
| `address_line2` | VARCHAR(255) | Yes | None | None | Suite, apartment, floor, or landmark |
| `city` | VARCHAR(100) | Yes | None | None | City / Municipality |
| `state` | VARCHAR(100) | Yes | None | None | State / Union Territory |
| `pincode` | INTEGER | Yes | None | None | 6-digit postal PIN code |
| `contact_person` | VARCHAR(200) | Yes | None | None | Authorized signatory or point of contact (for entities) |
| `gst_number` | VARCHAR(50) | Yes | None | None | GSTIN for professional legal fee invoicing |
| `pan_number` | VARCHAR(20) | Yes | None | None | PAN card for legal identification and KYC |
| `status` | VARCHAR(20) | No | `'Active'` | None | Relationship status (`Active`, `Inactive`, `Blocked`) |
| `created_at` | TIMESTAMPTZ | No | `NOW()` | None | Record creation timestamp |
| `updated_at` | TIMESTAMPTZ | No | `NOW()` | None | Last modification timestamp |
| `created_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | User who registered client |
| `updated_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | User who last updated client |

---

## 8. Table: `case_clients`
Junction table managing multi-party litigant representation for legal cases.

| Column | Data Type | Nullable | Default | Constraints | Description |
| :--- | :--- | :---: | :--- | :--- | :--- |
| `case_id` | UUID | No | None | `PK`, `FK` &rarr; `cases.case_id` | Associated legal matter |
| `client_id` | UUID | No | None | `PK`, `FK` &rarr; `clients.client_id` | Associated client profile |
| `party_type` | VARCHAR(50) | No | None | `Index` | `Petitioner`, `Respondent`, `Appellant`, `Defendant`, `Accused`, `Complainant` |
| `party_sequence` | INTEGER | No | `1` | None | Party serial (e.g. 1st Petitioner, 2nd Respondent) |
| `is_primary` | BOOLEAN | No | `true` | None | Denotes lead client for communications and billing |
| `status` | VARCHAR(20) | No | `'Active'` | None | Status (`Active`, `Discharged`, `Substituted`) |
| `created_at` | TIMESTAMPTZ | No | `NOW()` | None | Association creation timestamp |
| `updated_at` | TIMESTAMPTZ | No | `NOW()` | None | Association update timestamp |
| `created_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | Audit creator user |
| `updated_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | Audit updater user |

---

## 9. Table: `cases`
The central operational docket entity representing legal cases, writ petitions, civil suits, and criminal proceedings.

| Column | Data Type | Nullable | Default | Constraints | Description |
| :--- | :--- | :---: | :--- | :--- | :--- |
| `case_id` | UUID | No | `gen_random_uuid()` | `PK` | Unique internal case matter identifier |
| `law_firm_id` | UUID | Yes | None | `FK` &rarr; `law_firms.law_firm_id` | Multi-tenant organization (null for solo advocate) |
| `court_id` | UUID | No | None | `FK` &rarr; `courts.court_id`, `Index`| Presiding Court Hall / Bench establishment |
| `cnr_number` | VARCHAR(16) | Yes | None | `UK`, `Index` | 16-character unique eCourts CIS identifier (e.g. `DLND020047882015`) |
| `case_number` | VARCHAR(100) | No | None | `Index` | Formal court matter ID (e.g. `WP/12345/2024`, `OS/45/2023`) |
| `case_type` | VARCHAR(50) | No | None | `Index` | Category code (`Writ Petition`, `Civil Suit`, `Bail`, `Criminal Revision`) |
| `filing_number` | VARCHAR(100) | Yes | None | None | E-filing / Registry submission reference number |
| `filing_date` | DATE | Yes | None | None | Date original petition was filed in the registry |
| `registration_number` | VARCHAR(100) | Yes | None | None | Official court registration number upon docketing |
| `registration_date` | DATE | Yes | None | None | Official court registration date |
| `case_title` | VARCHAR(300) | No | None | None | Synthesized matter title: `Petitioner(s) vs Respondent(s)` |
| `case_stage` | VARCHAR(50) | No | None | None | Current stage: `Admission`, `Notice`, `Counter`, `Arguments`, `Orders` |
| `case_status` | VARCHAR(30) | No | `'Pending'` | `Index` | Legal status: `Pending`, `Disposed`, `Stayed`, `Transferred`, `Dismissed` |
| `acts_sections` | TEXT | Yes | None | None | Statutory sections involved (e.g. `IPC 302, NI Act Sec 138, CPC O-39`) |
| `police_station` | VARCHAR(100) | Yes | None | None | Jurisdictional police station (for criminal matters) |
| `fir_number` | VARCHAR(50) | Yes | None | None | Crime / FIR number |
| `fir_year` | INTEGER | Yes | None | None | Crime / FIR year |
| `is_ecourt_synced` | BOOLEAN | No | `false` | None | Denotes active automated synchronization with eCourts CIS API |
| `last_synced_at` | TIMESTAMPTZ | Yes | None | None | Timestamp of most recent eCourts docket update |
| `status` | VARCHAR(20) | No | `'Active'` | None | Internal system status (`Active`, `Archived`, `Deleted`) |
| `created_at` | TIMESTAMPTZ | No | `NOW()` | None | Record creation timestamp |
| `updated_at` | TIMESTAMPTZ | No | `NOW()` | None | Last modification timestamp |
| `created_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | User who created case record |
| `updated_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | User who last modified case record |

---

## 10. Table: `case_lawyers`
Manages counsel assignments, roles (Lead, Associate, Junior), and representation on record.

| Column | Data Type | Nullable | Default | Constraints | Description |
| :--- | :--- | :---: | :--- | :--- | :--- |
| `case_id` | UUID | No | None | `PK`, `FK` &rarr; `cases.case_id` | Target legal matter |
| `lawyer_id` | UUID | No | None | `PK`, `FK` &rarr; `lawyers.lawyer_id` | Assigned advocate |
| `lawyer_role_id` | VARCHAR(20) | No | None | `FK` &rarr; `case_lawyer_roles.role_id`| `CLR001: Lead`, `CLR002: Associate`, `CLR003: Junior` |
| `status` | VARCHAR(20) | No | `'Active'` | None | Assignment status (`Active`, `Withdrawn`) |
| `assigned_at` | TIMESTAMPTZ | No | `NOW()` | None | Date and time counsel was engaged |
| `created_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | Audit creator user |
| `updated_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | Audit updater user |

---

## 11. Table: `case_hearings`
Chronological court appearance log, daily cause board tracking, item numbers, proceedings, and next adjourned dates.

| Column | Data Type | Nullable | Default | Constraints | Description |
| :--- | :--- | :---: | :--- | :--- | :--- |
| `hearing_id` | UUID | No | `gen_random_uuid()` | `PK` | Unique hearing / cause list entry identifier |
| `case_id` | UUID | No | None | `FK` &rarr; `cases.case_id`, `Index`| Hearing target case |
| `hearing_date` | DATE | No | None | `Index` | Date listed on official court cause list |
| `item_number` | INTEGER | Yes | None | None | Serial / Item board number on daily cause list (e.g. Item #14) |
| `court_hall` | VARCHAR(100) | Yes | None | None | Court Hall / Bench room designation (e.g. Court Room No. 4) |
| `judge_name` | VARCHAR(200) | Yes | None | None | Presiding Judge / Coram designation |
| `purpose_of_hearing` | VARCHAR(150) | No | None | None | Listing purpose: `For Admission`, `For Counter`, `Arguments` |
| `business_on_date` | TEXT | Yes | None | None | Daily proceedings summary / oral directions recorded in court |
| `next_hearing_date` | DATE | Yes | None | `Index` | Next adjourned hearing date assigned by the bench |
| `next_purpose` | VARCHAR(150) | Yes | None | None | Purpose of listing on the next adjourned date |
| `hearing_status` | VARCHAR(30) | No | `'Scheduled'` | None | `Scheduled`, `Passed Over`, `Adjourned`, `Disposed`, `Order Reserved` |
| `daily_order_summary` | TEXT | Yes | None | None | Internal chamber notes & highlights of daily court orders |
| `created_at` | TIMESTAMPTZ | No | `NOW()` | None | Record creation timestamp |
| `updated_at` | TIMESTAMPTZ | No | `NOW()` | None | Record modification timestamp |
| `created_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | User or background sync worker |
| `updated_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | User who last updated hearing |

---

## 12. Table: `ecourt_sync_logs`
Audit log and telemetry tracker for automated docket background synchronizations against eCourts CIS.

| Column | Data Type | Nullable | Default | Constraints | Description |
| :--- | :--- | :---: | :--- | :--- | :--- |
| `sync_id` | UUID | No | `gen_random_uuid()` | `PK` | Unique synchronization run identifier |
| `case_id` | UUID | No | None | `FK` &rarr; `cases.case_id`, `Index`| Case synchronized |
| `cnr_number` | VARCHAR(16) | No | None | `Index` | 16-character CNR queried |
| `sync_status` | VARCHAR(30) | No | None | `Index` | `Success`, `Partial`, `Failed`, `InProgress` |
| `sync_source` | VARCHAR(50) | No | `'AutoCron'` | None | `AutoCron`, `ManualRefresh`, `WebhookNotification` |
| `hearings_updated_count` | INTEGER | No | `0` | None | Count of newly created/updated hearings from eCourts |
| `orders_downloaded_count` | INTEGER | No | `0` | None | Count of newly detected and ingested court orders |
| `raw_payload` | JSONB | Yes | None | None | Raw response JSON docket mirror for offline fallback |
| `error_details` | TEXT | Yes | None | None | Diagnostic error trace if sync failed |
| `synced_at` | TIMESTAMPTZ | No | `NOW()` | None | Synchronization completion timestamp |
| `triggered_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | User who triggered manual sync (null for cron) |

---

## 13. Table: `case_orders`
Interim orders, final judgments, bail orders, and certified true copies downloaded from court portals.

| Column | Data Type | Nullable | Default | Constraints | Description |
| :--- | :--- | :---: | :--- | :--- | :--- |
| `order_id` | UUID | No | `gen_random_uuid()` | `PK` | Unique court order record identifier |
| `case_id` | UUID | No | None | `FK` &rarr; `cases.case_id`, `Index`| Case to which order belongs |
| `hearing_id` | UUID | Yes | None | `FK` &rarr; `case_hearings.hearing_id`| Specific hearing on which order was pronounced |
| `order_date` | DATE | No | None | `Index` | Date order was passed by the bench |
| `order_type` | VARCHAR(50) | No | None | None | `Interim Order`, `Final Judgment`, `Bail Order`, `Directions` |
| `order_url` | VARCHAR(500) | Yes | None | None | Official eCourts download URL |
| `pdf_storage_path` | VARCHAR(500) | Yes | None | None | Encrypted S3 / Cloud Storage path of stored PDF |
| `is_certified` | BOOLEAN | No | `true` | None | Digitally signed, watermarked certified true copy flag |
| `order_markdown_content`| TEXT | Yes | None | None | Full text / OCR transcription of the judgment in Markdown |
| `created_at` | TIMESTAMPTZ | No | `NOW()` | None | Record creation timestamp |
| `updated_at` | TIMESTAMPTZ | No | `NOW()` | None | Record modification timestamp |
| `created_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | Audit creator user |
| `updated_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | Audit updater user |

---

## 14. Table: `case_documents`
Digital case file repository (pleadings, petitions, affidavits, evidence, and certified copies).

| Column | Data Type | Nullable | Default | Constraints | Description |
| :--- | :--- | :---: | :--- | :--- | :--- |
| `document_id` | UUID | No | `gen_random_uuid()` | `PK` | Unique document identifier |
| `case_id` | UUID | No | None | `FK` &rarr; `cases.case_id`, `Index`| Case matter this document belongs to |
| `hearing_id` | UUID | Yes | None | `FK` &rarr; `case_hearings.hearing_id`| Optional hearing where document was marked/tendered |
| `category` | VARCHAR(50) | No | None | `Index` | `Petition`, `Vakalatnama`, `Counter`, `Affidavit`, `Order`, `Evidence`, `FIR` |
| `document_title` | VARCHAR(255) | No | None | None | User-facing display title (e.g. `Interim Injunction Petition`) |
| `file_name` | VARCHAR(255) | No | None | None | Sanitized filename in storage |
| `file_url` | VARCHAR(500) | No | None | None | Secure S3 URI / Cloud Storage URL |
| `mime_type` | VARCHAR(100) | No | None | None | `application/pdf`, `image/jpeg`, `image/png` |
| `file_size_bytes` | BIGINT | No | None | None | File payload size in bytes |
| `status` | VARCHAR(20) | No | `'Active'` | None | Record status (`Active`, `Deprecated`, `Archived`) |
| `uploaded_at` | TIMESTAMPTZ | No | `NOW()` | None | Upload timestamp |
| `uploaded_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | User who uploaded file |

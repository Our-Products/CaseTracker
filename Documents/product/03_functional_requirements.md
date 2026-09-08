# CaseTracker — Functional Requirements Specification

> **Document Version:** 1.1  
> **Status:** Active Reference  
> **Parent Documentation:** [Documentation Center](../README.md)

---

## 1. Account & Identity Management

### 1.1 Advocate & Staff Registration
* **Primary Key Identifier**: Mobile Number (10 digits).
* **Registration Modes**:
  1. **Individual Advocate**: Full Name, Mobile Number, Email, Password, Bar Council ID (e.g., `TN/1042/2018`), Bar Council Name, Enrollment Date.
  2. **Law Firm / Organization**: Firm Name, Registration Number, Office Address (Lines 1 & 2, City, District, State, Pincode), Admin User Details.
* **Security**: Enforced password hashing (PBKDF2/BCrypt) and mobile/email uniqueness checks.

### 1.2 Authentication & Session Lifecycle
* **Authentication Method**: Mobile Number + Password.
* **Token Handshake**: Issues HS256-signed JWT containing:
  * `nameid` / `sub`: User GUID.
  * `email`: User Email.
  * `role`: Assigned roles (`Lawyer`, `Staff`, `Admin`).
  * `exp`: Token expiration timestamp (default: 60 minutes).
* **Mobile Client Fallback**: Offline authentication cache for field access in court basements or connectivity dead zones.

---

## 2. Client Management (Phase 2)

Maintains comprehensive master records of legal clients:
* **Client Types**: Individual Person, Corporate / Entity, Partnership / Trust.
* **Fields**: Client Name, Primary Phone, Alternative Phone, Email, Physical Address, Organization Name, Contact Person (for entities).
* **Association**: A single client can have multiple cases across different courts.
* **Operations**: Add, edit, soft-delete/deactivate, search by name/phone, and view matter history.

---

## 3. Case Matter Management (Phase 2)

The primary operational entity connecting clients, courts, and legal files:
* **Identification**:
  * **CNR Number**: 16-character alphanumeric unique eCourts identifier (e.g., `TNCH010012342024`).
  * **Case Number**: Case Type + Filing Number + Year (e.g., `WP/12345/2024`, `CRL OP/987/2024`, `OS/45/2023`).
* **Attributes**:
  * Case Title (Petitioner vs. Respondent, e.g., *M. Sundaram vs. State of Tamil Nadu*).
  * Client Role (Petitioner / Appellant vs Respondent / Defendant).
  * Case Type (Writ Petition, Civil Suit, Criminal Revision, Bail App, etc.).
  * Case Stage (Admission, Counter Filed, Framing Issues, Arguments, Orders).
  * Status (Pending, Disposed, Stayed, Transferred).
  * Filing Date, Registration Date.

---

## 4. Courtroom Hierarchy

Structured reference data mapping Indian court halls:

```text
India
 └── State (e.g., Tamil Nadu)
      └── District (e.g., Chennai)
           └── Court Complex (e.g., George Town Metropolitan Magistrate Court)
                └── Court Hall (e.g., Court Room No. 2 - II Metropolitan Magistrate)
```

Attributes include Court Name, Judge Designation (e.g., *Hon'ble Mr. Justice...* / *Principal District Judge*), and Court Hall Number.

---

## 5. Daily Cause Lists & Hearing Management

* **Cause List Scheduling**:
  * Listing Date, Item / Serial Board Number (e.g., Item #14).
  * Stage of Hearing (e.g., *For Hearing*, *For Orders*, *Notice to Respondent*).
  * Courtroom allocation.
* **Hearing History**:
  * Maintains an immutable chronological audit trail of all previous hearings.
  * Records previous hearing dates, daily proceedings summary, next adjourned date, and purpose of next hearing.
* **Daily Board Filter**: Real-time filtering on mobile devices by courtroom, urgent list, and advocate appearance.

---

## 6. eCourts CIS Integration (Phase 3)

Automates docket synchronization through the eCourts Case Information System:
* **Search by CNR Number**: Fetches complete case metadata, party names, and current case status.
* **Automated Sync Worker**: Background job queries eCourts daily for newly published cause lists and uploaded interim orders.
* **Offline-First Persistence**: All retrieved case details are mirrored in the local PostgreSQL database to ensure instant accessibility even when official government portals are down.

---

## 7. Document Storage Architecture (Phase 4)

Decouples file metadata from binary payload storage:
* **Database (PostgreSQL)**: Document ID, Case ID, Document Category (Petition, Affidavit, Certified Order, FIR), File Name, File Size, S3 Key / Storage Path, UploadedBy, UploadedAt.
* **Object Storage**: S3-compatible cloud bucket storing encrypted PDF / image binaries.


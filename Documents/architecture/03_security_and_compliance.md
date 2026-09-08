# CaseTracker — Security Architecture & Compliance

> **Document Version:** 1.0  
> **Status:** Active Baseline  
> **Parent Documentation:** [Documentation Center](../README.md)

---

## 1. Threat Model & Legal Data Classification

CaseTracker stores sensitive legal matters, attorney-client privileged notes, interim stay orders, and client identification details. Under Indian law (including the **Advocates Act, 1961** and the **Digital Personal Data Protection Act, 2023**), attorney-client communications are legally privileged.

### Data Classification Matrix

| Classification | Examples | Storage & Encryption Controls |
| :--- | :--- | :--- |
| **Highly Confidential** | Case briefs, privileged advice, FIR transcripts | Encrypted in transit (TLS 1.3), access restricted to assigned advocate |
| **Confidential** | Client contact info, lawyer personal mobile, passwords | Cryptographic hashing (passwords), JWT-authenticated queries |
| **Public / Court Records** | CNR numbers, publicly listed hearing dates, court halls | Public indexable reference, sanitized for public court cause lists |

---

## 2. Authentication & Credential Storage

### 2.1 Cryptographic Password Hashing
* Implemented in [`PasswordService.cs`](file:///D:/CaseTracker/Backend/CaseTrackerInfrastructure/Services/PasswordService.cs).
* Raw passwords are **never stored** in the database.
* Uses cryptographically secure salt generation and iterative key derivation (PBKDF2/BCrypt) to protect against rainbow-table and offline brute-force attacks.

### 2.2 Stateless JWT Bearer Tokens
* Implemented in [`JwtService.cs`](file:///D:/CaseTracker/Backend/CaseTrackerInfrastructure/Services/JwtService.cs).
* Signed using HMAC-SHA256 (`HS256`) with a minimum 256-bit secret key.
* **Token Claims**:
  * `ClaimTypes.NameIdentifier`: Unique `UserId` GUID.
  * `ClaimTypes.Email`: User email address.
  * `ClaimTypes.Role`: One or more assigned roles (`Lawyer`, `Staff`, `Admin`).
* **Expiration**: Enforced token expiration with strict zero clock skew (`ClockSkew = TimeSpan.Zero` in [`AuthenticationServiceExtensions.cs`](file:///D:/CaseTracker/Backend/CaseTracker/Extensions/AuthenticationServiceExtensions.cs)).

---

## 3. Authorization & Access Control

### 3.1 Role-Based Access Control (RBAC)
The platform defines three baseline roles seeded into the database:
* **Lawyer (`R001`)**: Full ownership of assigned clients, cases, and hearing cause-lists.
* **Staff (`R002`)**: Delegated permissions for docketing, filing updates, and document uploads.
* **Admin (`R003`)**: Organization-wide governance for managing firm rosters and branch configurations.

### 3.2 Tenant-Level Scoping
All query operations in repository and service layers enforce tenant boundaries:
```csharp
// Example: Restricting case query to authenticated advocate or firm
query = query.Where(c => c.LawyerId == authenticatedLawyerId || c.LawFirmId == authenticatedFirmId);
```

---

## 4. Audit Trail & Immutability

Every entity in [`CaseTrackerDomain/Models`](file:///D:/CaseTracker/Backend/CaseTrackerDomain/Models) inherits standard audit tracking attributes:
* `CreatedAt`: UTC timestamp when record was created.
* `UpdatedAt`: UTC timestamp of latest revision.
* `CreatedBy`: User GUID of the creator.
* `UpdatedBy`: User GUID of the modifier.

Audit foreign keys use `DeleteBehavior.Restrict` in PostgreSQL to ensure historical accountability is preserved even if user accounts are deactivated.


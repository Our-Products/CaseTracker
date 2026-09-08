# CaseTracker — Product Vision & Overview

> **Document Version:** 1.1  
> **Target Region:** Tamil Nadu & Puducherry (Initial), India (Expansion)  
> **Status:** Approved Baseline  
> **Parent Documentation:** [Documentation Center](../README.md)

---

## 1. Executive Summary

**CaseTracker** is a specialized legal case management application designed to empower advocates, legal clerks, and law-firm staff to manage clients, cases, courtroom hierarchies, daily cause lists, hearings, and associated legal documentation from a centralized, real-time ecosystem.

The application initially targets legal practitioners operating within the **High Court of Judicature at Madras (Madras & Madurai Benches)**, District Courts, Sub Courts, and Munsif/Magistrate courts across **Tamil Nadu and Puducherry**, with a foundational architecture engineered to scale nationwide across India.

A central differentiator is the planned synchronization with the **eCourts ecosystem (eCourts CIS / Case Information System)**, eliminating manual tracking and reducing missed hearings through automated notifications and alerts.

---

## 2. Core Objectives

* **Single Pane of Glass**: Provide a centralized platform for managing legal clients, court matters, and daily cause lists.
* **Role-Based Collaboration**: Allow senior advocates, junior associates, and authorized law-firm staff to access and update case information according to their permissions.
* **Automated Hearing Tracking**: Monitor scheduled hearings, courtroom allocations, board numbers, and hearing statuses.
* **eCourts Synchronization**: Automatically retrieve and update case history, interim orders, and next hearing dates from official eCourts services.
* **Proactive Notifications**: Trigger automated alerts for upcoming hearings, urgent listings, and pending compliance deadlines.
* **Legal Document Management**: Maintain organized repositories of FIRs, charge sheets, petitions, interim applications, and judgments.
* **Data Privacy & Multi-Tenancy**: Guarantee strict logical data separation between individual practitioners and competing law firms.

---

## 3. Target User Personas

### 3.1 Advocates / Lawyers (Primary Persona)
Advocates are the primary end-users who manage their own practice or work as part of a firm.
* **Key Tasks**:
  * Onboard clients and register new court cases.
  * Review daily cause lists and prioritized hearing schedules.
  * Access case briefs, orders, and legal documents in courtroom corridors.
  * Receive real-time push reminders for tomorrow's listed matters.
  * Connect case records with official Bar Council credentials (`TN/XXXX/YYYY`).

### 3.2 Law-Firm Staff & Junior Associates
Authorized paralegals, clerks, or junior lawyers who support daily legal administrative operations.
* **Key Tasks**:
  * Enter client contact information and initial case filing metadata.
  * Upload petitions, certified copies of orders, and case documentation.
  * Update daily hearing outcomes (e.g., adjourned, passed over, argument concluded).
  * Monitor court notice dates and compliance tasks.

### 3.3 Law-Firm Administrators
Senior partners or office managers responsible for organization-level governance.
* **Key Tasks**:
  * Manage advocate memberships, staff invitations, and role assignments.
  * Configure law firm details, branch offices, and shared firm libraries.
  * Supervise overall firm caseload, hearing volume, and metric KPIs.

---

## 4. Geographic & Court Hierarchy Strategy

### 4.1 Initial Focus: Tamil Nadu & Puducherry
The system is optimized for the legal hierarchy of Tamil Nadu and Puducherry:
* **High Court**: High Court of Judicature at Madras (Principal Seat, Chennai & Madurai Bench).
* **District Judiciary**: Principal District Courts, City Civil Courts (Chennai), Small Causes Courts, Sessions Courts.
* **Subordinate Courts**: Subordinate Judge Courts (Sub Courts), Assistant Sessions Courts.
* **Magisterial & Munsif Courts**: Judicial Magistrate Courts, District Munsif Courts, Metropolitan Magistrate Courts.
* **Specialized Tribunals**: NCLT Chennai, DRT Chennai/Coimbatore/Madurai, Consumer Forums, Labor Courts.

### 4.2 Pan-India Scalability
The data model avoids hardcoded state or court limitations, utilizing an extensible relational hierarchy:

```text
Country (India)
  └── State / Union Territory (e.g., Tamil Nadu, Puducherry, Karnataka, Delhi)
        └── District (e.g., Chennai, Coimbatore, Madurai, Puducherry)
              └── Court Complex (e.g., Madras High Court Campus, George Town Court Complex)
                    └── Court / Court Hall (e.g., Court Hall No. 14, 2nd Additional District Court)
```

---

## 5. Security & Legal Ethics Principles

Legal records are highly confidential and governed by the Advocates Act, 1961 and professional attorney-client privilege. The system enforces:
* **Tenant Isolation**: Strict organizational scoping preventing data leakage between law firms.
* **Encrypted Data in Transit**: Enforced TLS 1.3/HTTPS across all API communications.
* **Audit Logging**: Comprehensive audit trails (`CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`) on all core business records.
* **Least Privilege**: Granular role-based authorization restricting sensitive case documents to assigned counsel.

---

## 6. Next Steps & Related Documents

* [Project Scope & Phase Boundaries](02_project_scope.md)
* [Detailed Functional Requirements](03_functional_requirements.md)
* [System Architecture Overview](../architecture/01_system_architecture.md)


# CaseTracker — Database Data Dictionary

> **Document Version:** 1.0  
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
| `created_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | Audit creator user |
| `updated_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | Audit updater user |

---

## 5. Table: `user_role`
Junction table managing Many-to-Many assignments between user accounts and system roles.

| Column | Data Type | Nullable | Default | Constraints | Description |
| :--- | :--- | :---: | :--- | :--- | :--- |
| `user_id` | UUID | No | None | `PK`, `FK` &rarr; `users.user_id` | Assigned user account |
| `role_id` | VARCHAR(20) | No | None | `PK`, `FK` &rarr; `roles.role_id` | Assigned system role |
| `created_at` | TIMESTAMPTZ | No | `NOW()` | None | Timestamp when role was granted |
| `updated_at` | TIMESTAMPTZ | No | `NOW()` | None | Timestamp of last modification |
| `created_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | Administrator who assigned the role |
| `updated_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | Administrator who last modified assignment |

---

## 6. Table: `user_law_firms`
Junction table managing advocate and staff memberships within law firms.

| Column | Data Type | Nullable | Default | Constraints | Description |
| :--- | :--- | :---: | :--- | :--- | :--- |
| `user_id` | UUID | No | None | `PK`, `FK` &rarr; `users.user_id` | Member user account |
| `law_firm_id` | UUID | No | None | `PK`, `FK` &rarr; `law_firms.law_firm_id` | Associated law firm |
| `joined_at` | TIMESTAMPTZ | No | `NOW()` | None | Official date/time of association |
| `status` | VARCHAR(20) | No | `'Active'` | None | Membership status (`Active`, `OnLeave`, `Former`) |
| `created_at` | TIMESTAMPTZ | No | `NOW()` | None | Creation timestamp in UTC |
| `updated_at` | TIMESTAMPTZ | No | `NOW()` | None | Modification timestamp in UTC |
| `created_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | User who linked the account |
| `updated_by` | UUID | Yes | None | `FK` &rarr; `users.user_id` | User who last updated association |

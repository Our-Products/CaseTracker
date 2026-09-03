# Case Tracker — Project Overview

## 1. Project Overview

**Case Tracker** is a legal case management application designed to help lawyers and law-firm staff manage clients, cases, court information, hearings, documents, notifications, and related legal activities from a centralized system.

The application will initially target lawyers and law firms operating in **Tamil Nadu and Puducherry**, with the architecture designed to support expansion across **India** in the future.

The system will combine internal case-management capabilities with integration services such as the **eCourts ecosystem** to retrieve and synchronize relevant case and hearing information.

The primary goal is to reduce manual tracking of cases and upcoming hearings and provide lawyers with timely information through notifications and reminders.

---

# 2. Project Objectives

The main objectives of Case Tracker are:

* Provide a centralized platform for managing legal clients and cases.
* Allow lawyers and authorized law-firm staff to access relevant case information.
* Track upcoming hearings and important case events.
* Integrate with eCourts services to retrieve and update case and hearing information.
* Provide notifications for upcoming hearings and other important events.
* Store and manage legal documents associated with cases.
* Provide a scalable foundation for future video-conferencing functionality.
* Design the system for initial use in Tamil Nadu and Puducherry while keeping it suitable for future India-wide expansion.
* Maintain strong separation of data between different lawyers and organizations.

---

# 3. Target Users

The initial target users are:

### 3.1 Lawyers

Lawyers are the primary users of the application.

They should be able to:

* Manage their clients.
* Manage their cases.
* Track case status.
* View court information.
* Track upcoming hearings.
* Access case documents.
* Receive notifications and reminders.
* Retrieve and synchronize case information from eCourts.

### 3.2 Law-Firm Staff

Authorized staff members can assist lawyers with case-management activities.

Depending on their assigned permissions, staff may be able to:

* Manage client information.
* Update case information.
* Manage hearings.
* Upload documents.
* Monitor upcoming hearings.
* Manage administrative tasks.

### 3.3 Law-Firm Administrator

A future administrative role may be responsible for:

* Managing lawyers.
* Managing staff members.
* Managing organization-level settings.
* Controlling user permissions.
* Monitoring organization activity.

The permission model should be designed so additional roles can be introduced in the future without redesigning the complete authorization system.

---


# 4. Geographic Strategy

### Initial Target

The first release will focus on:

* Tamil Nadu
* Puducherry

### Future Expansion

The application should eventually support lawyers and courts across India.

Therefore, the core domain model should avoid assumptions that restrict the application to a particular state or court system.

The geographic model should support:

```text
India
 └── States / Union Territories
       └── Districts
             └── Court Complexes
                   └── Courts
```

---


# 5. Technology Stack

## Backend

* ASP.NET Core
* .NET 10
* C#
* RESTful Web APIs
* Entity Framework Core

## Mobile

* .NET MAUI

## Database

* PostgreSQL

## Authentication

* JWT

## ORM

* Entity Framework Core

## External Integration

* eCourts services

## File Storage

* Object/file storage provider — to be finalized

## Notifications

* Notification provider — to be finalized

---

# 6. Security Principles

Security is a core requirement because the application will contain sensitive legal and personal information.

The system should follow principles including:

* Secure authentication
* Password hashing
* JWT-based authorization
* Role-based access control
* Tenant/data ownership enforcement
* Input validation
* Secure document access
* HTTPS
* Audit information for important operations
* Protection against unauthorized data access
* Secure handling of external API credentials
* Secure storage of sensitive configuration

Security requirements will be expanded during the authentication, authorization, database, API, and document-management design phases.

---
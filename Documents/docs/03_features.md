# Case Tracker —  Core Application Features

## 1.  Lawyer Account Management

The application will provide account functionality for lawyers and authorized staff.

Planned capabilities include:

* Registration
* Login
* Logout
* Profile management
* Account status management
* Password management
* Mobile-number verification
* Role-based access control

### Authentication

The primary authentication mechanism will use:

**Mobile Number + Password**

JWT-based authentication will be used for securing API access.

Future authentication capabilities may include:

* OTP login
* Multi-factor authentication
* Additional identity providers

---


## 2. Client Management

The application will maintain client master information separately from case information.

A client may have multiple legal cases.

Planned capabilities include:

* Add client
* Edit client
* View client
* Search client
* View client history
* Deactivate client
* Manage client contact information

Client information should remain focused on the client's identity and profile.

Case-specific information should be maintained by the Case entity rather than being stored directly in the Client entity.

---

## 3. Case Management

Case Management is one of the core modules of the application.

Planned capabilities include:

* Create case
* Update case
* View case
* Search case
* Track case status
* Associate cases with clients
* Associate cases with courts
* Track case numbers
* Track case types
* Track case-related information
* Track important case dates

A single client may have multiple cases.

The Case entity will therefore maintain the relationship between the client and the legal matter.

---

## 4. Court Information

The application will maintain structured court information.

The initial geographical hierarchy will support:

```text
India
 └── State
      └── District
           └── Court Complex
                └── Court
```

The initial focus will be:

* Tamil Nadu
* Puducherry

The data model should not be restricted to these regions so that additional states and courts can be supported in the future.

Planned court-related information may include:

* State
* District
* Court Complex
* Court
* Case Type
* Court identifiers
* Other relevant court metadata

---

## 5. eCourts Integration

eCourts integration will be a core supporting capability of Case Tracker.

The application will use the available eCourts services to retrieve relevant case and hearing information.

The planned integration should support:

* Case search
* Retrieval of case information
* Importing relevant case information
* Retrieval of hearing information
* Updating upcoming hearing information
* Synchronizing relevant information with Case Tracker

The application should not depend entirely on real-time availability of external eCourts services.

The preferred flow is:

```text
eCourts
    ↓
eCourts Integration Layer
    ↓
Case Tracker Application
    ↓
PostgreSQL
    ↓
Mobile Application
```

Relevant information retrieved from external services should be stored locally where appropriate so that the application can continue to provide useful information even when the external service is temporarily unavailable.

The exact eCourts API capabilities and supported courts/case types will be documented separately in:

`docs/integrations/ecourts-integration.md`

---

## 6. Hearing Management

Hearings will be managed as a separate part of the case-management system.

The application should allow lawyers and authorized staff to:

* View upcoming hearings.
* Track hearing dates.
* View court information associated with a hearing.
* Track hearing status.
* View previous hearing information.
* Receive reminders for upcoming hearings.

Future capabilities may include:

* Hearing notes
* Hearing outcomes
* Adjournment tracking
* Video-conference links
* Automated hearing updates

The system should be designed so that hearing information retrieved from eCourts can update the application's upcoming hearing information.

---

## 7. Notification Mechanism

The application will provide a notification mechanism to keep users informed about important events.

Initial notification scenarios include:

* Upcoming hearings
* Case-related reminders
* Important case events
* Tasks and reminders

The notification architecture should be extensible so additional notification types can be introduced later.

Potential future notification channels include:

* Mobile push notifications
* Email
* SMS
* Other supported notification mechanisms

The initial implementation and notification provider will be defined during the notification-module design phase.

---

## 8. Document Management

Legal documents will be associated with cases.

Examples include:

* FIR
* Petitions
* Orders
* Judgments
* Other case-related documents

The application will separate **document metadata** from the actual file storage.

The planned architecture is:

```text
PostgreSQL
    │
    └── Document Metadata
             │
             ├── Document ID
             ├── Case ID
             ├── Document Type
             ├── File Name
             ├── Storage Reference
             └── Audit Information

File/Object Storage
    │
    └── Actual Documents
```

PostgreSQL should not be used as the primary storage location for large document files unless a specific requirement justifies it.

The final storage provider will be decided during the Document Management design phase.

---

## 9. Video Conferencing

Video-conferencing support is planned as a future capability.

The long-term concept is:

```text
Case
   ↓
Hearing
   ↓
Video Conference
   ↓
Video Conference Link
```

The initial version of the application does not need to implement the complete video-conferencing functionality.

The architecture should, however, allow a video-conference link or related information to be associated with a hearing in the future.

---

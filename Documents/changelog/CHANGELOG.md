# CaseTracker — Changelog

All notable changes to the CaseTracker solution will be documented in this file.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased] - Phase 2 In-Progress
### Planned
- Client management endpoints, domain models, and repositories.
- Case matter indexing with CNR numbers and court allocations.
- Daily hearing schedule logging and calendar view.

---

## [0.2.0] - 2026-09-06
### Added
- **Security & Authorization Architecture**:
  - Implemented clean repository pattern across `CaseTrackerApplication` and `CaseTrackerInfrastructure`.
  - Added `IUnitOfWork` with transactional commit and rollback support.
  - Configured JWT Bearer token generation with claims for `UserId`, `Email`, and `Roles`.
  - Added PBKDF2/BCrypt cryptographic password hashing in `PasswordService`.
  - Added centralized `GlobalExceptionHandlerMiddleware` for standard HTTP JSON error mappings.
- **REST Endpoints**:
  - `AuthController`: Registration (individual lawyer and organization law-firm) and Login.
  - `LawyerController`: Lawyer profile queries by ID, Bar Council ID, and Law Firm.
  * `LawFirmController`, `UserController`, `RoleController`, `UserRoleController`, `UserLawFirmController`.
- **Database Migrations**:
  - Applied initial PostgreSQL schema migration `UpdateAllEntities` defining tables, foreign keys, and indexes.

---

## [0.1.0] - 2026-09-05
### Added
- **Mobile Design System (.NET MAUI)**:
  - Implemented Shadcn UI components: `ShadcnEntry`, `ShadcnButton`, `ShadcnCard`, `ShadcnBadge`, and `ShadcnMetricCard`.
  - Global `EntryHandler` and `PickerHandler` removing native platform underlines across Android, iOS, and Windows.
  - Integrated `CommunityToolkit.Mvvm` with `ObservableValidator` for instant form validation.
  - Registered Shell routes (`Login`, `Register`, `Dashboard`) and advocate dashboard with cause list filtering.
  - Added offline/local authentication fallback in mobile `AuthService`.
- **Reference**: Detailed breakdown in [Balaji Architecture & Design System Changes](2026-09-05-balaji-architecture-design-system.md).


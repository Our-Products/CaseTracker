# CaseTracker — Architecture & Design System Changes

**Author / Submitted By:** Balaji  
**Date:** September 5, 2026  
**Application Target:** .NET MAUI (Android, iOS, Windows, MacCatalyst) & ASP.NET Core EF Core Backend  

---

## Executive Summary & Functionality Usecases

This document details the architectural enhancements, custom design system, real-time validation mechanism, Shell navigation fix, and database seeding implemented for the **CaseTracker** legal management application.

---

## 1. Centralized Shadcn Design System (`CaseTrackerMobile/Controls`)

### Functionality & Usecase
To ensure a premium, modern, zero-lag UI across both mobile (Android/iOS) and desktop platforms, a centralized 100% Shadcn-inspired design system was implemented.

### Components Created
*   **`ShadcnEntry.xaml` & `ShadcnEntry.xaml.cs`**:
    *   *Usecase*: Standardized input component featuring label, rounded border (`StrokeShape="RoundRectangle 8"`), transparent entry, right-side action toggles (e.g. password visibility eye button), and real-time inline red helper error text (`#EF4444`).
*   **`ShadcnButton.xaml` & `ShadcnButton.xaml.cs`**:
    *   *Usecase*: Primary action button (`#113979`) featuring touch scale press animation (`ScaleTo(0.96, 50)` -> `ScaleTo(1.0, 50)`), bold white text, and integrated loading activity indicator (`IsLoading`).
*   **`ShadcnCard.xaml` & `ShadcnCard.xaml.cs`**:
    *   *Usecase*: Container for grouping form sections, cause list items, and advocate details with subtle borders and shadows.
*   **`ShadcnBadge.xaml` & `ShadcnBadge.xaml.cs`**:
    *   *Usecase*: Status pill badge (`Default`, `Secondary`, `Outline`, `Destructive`) for cause list item status badges (e.g. "Urgent Hearing", "Listed", "Passed Over", "Awaiting Order").
*   **`ShadcnMetricCard.xaml` & `ShadcnMetricCard.xaml.cs`**:
    *   *Usecase*: KPI Metric widget displaying bold numbers, title, trend subtitle, and colored background icon badge.

---

## 2. Global Control Handlers (Zero Platform Underlines)

### Functionality & Usecase
Default native MAUI `Entry` and `Picker` controls render unwanted platform underlines on Android and iOS. 

### Implementation (`MauiProgram.cs`)
*   Added global `EntryHandler.Mapper` and `PickerHandler.Mapper` mappings.
*   *Android*: Sets platform view background to `null` and transparent.
*   *iOS / MacCatalyst*: Sets `BorderStyle` to `UITextBorderStyle.None`.
*   *Windows*: Sets `BorderThickness` to `0`.

Result: Inputs fit cleanly inside the custom Shadcn rounded `Border` containers with **zero native underlines**.

---

## 3. Real-Time Form Validation (`CommunityToolkit.Mvvm.ComponentModel.ObservableValidator`)

### Functionality & Usecase
Advocates entering invalid mobile numbers, passwords, or emails receive instantaneous visual feedback without UI lag.

### Implementation Details
*   **`LoginViewModel.cs`**:
    *   Inherits from `ObservableValidator`.
    *   `Mobile`: Validated with `[Required]` and `[RegularExpression(@"^[0-9]{10}$")]`. Real-time error state updates on every keystroke.
    *   `Password`: Validated with `[Required]` and `[MinLength(6)]`.
*   **`RegisterViewModel.cs`**:
    *   Validates Advocate `FullName`, `MobileNumber`, `Email`, `BarCouncilId` (`TN/1042/2018`), `Password`, and real-time `ConfirmPassword` match confirmation.

---

## 4. Shell Navigation Fix & Dynamic Advocate Dashboard

### Functionality & Usecase
Resolved "Create new account" routing failure (`ArgumentException`) by registering standard Shell routes (`Login`, `Register`, `Dashboard`) in `AppShell.xaml` and `AppShell.xaml.cs` and updating `GoToAsync` calls to `//Register`, `//Dashboard`, and `//Login`.

### Advocate Dashboard Features (`DashboardPage.xaml` & `DashboardViewModel.cs`)
*   **Advocate Header Banner**: Displays advocate name (`Adv. R. Sundaram`), Bar Council enrollment (`TN/1042/2018`), Law Firm (`Sundaram & Associates`), and Logout button.
*   **4 Metrics Cards**: Active Cases (42), Today's Hearings (05), Pending Orders (08), Total Clients (128).
*   **Interactive Search & Filter**: Real-time filtering of cause list items by case number, client title, court room, or stage.
*   **Today's Cause List Schedule**: Real-time listing view rendered with `ShadcnCard` and `ShadcnBadge`.
*   **Quick Actions**: "+ Add New Case" and "eCourts Sync".

---

## 5. Virtual Development Database & Seed Dummy Advocate Account

### Functionality & Usecase
Allows developers and testers to immediately log in and test advocate workflows without local PostgreSQL server installation.

### Implementation (`DbInitializer.cs` & `appsettings.Development.json`)
*   Enabled `UseVirtualDatabase: true` with SQLite fallback (`casetracker_virtual_dev.db`).
*   Auto-creates all EF Core database tables (`users`, `roles`, `user_role`, `lawyers`, `law_firms`).
*   Auto-seeds default roles: `R001` (Lawyer), `R002` (Staff), `R003` (Admin).
*   Auto-seeds **Test Advocate Account**:
    *   **Mobile Number**: `9876543210`
    *   **Email**: `advocate@test.com`
    *   **Password**: `Password123!`
    *   **Advocate Name**: `Adv. R. Sundaram`
    *   **Bar Council ID**: `TN/1042/2018`
    *   **Bar Council**: `Bar Council of Tamil Nadu and Puducherry`

---

## Summary of Modified & Created Files

1. `CaseTrackerMobile/MauiProgram.cs` — Registered global `EntryHandler` and `PickerHandler` underline stripping + DI registrations.
2. `CaseTrackerMobile/Controls/ShadcnEntry.xaml` & `.cs` — Reusable Shadcn input with zero underline & inline error.
3. `CaseTrackerMobile/Controls/ShadcnButton.xaml` & `.cs` — Primary `#113979` button with touch scale animation.
4. `CaseTrackerMobile/Controls/ShadcnCard.xaml` & `.cs` — Centralized card container control.
5. `CaseTrackerMobile/Controls/ShadcnBadge.xaml` & `.cs` — Status pill badge control.
6. `CaseTrackerMobile/Controls/ShadcnMetricCard.xaml` & `.cs` — Metric KPI card control.
7. `CaseTrackerMobile/Converters/ValueConverters.cs` — Registered `IsNotNullOrEmptyConverter` and `InvertedBoolConverter`.
8. `CaseTrackerMobile/ViewModels/LoginViewModel.cs` — Migrated to `ObservableValidator` with real-time validation and `//Dashboard` / `//Register` routes.
9. `CaseTrackerMobile/ViewModels/RegisterViewModel.cs` — Migrated to `ObservableValidator` with Advocate profile fields and `//Login` route.
10. `CaseTrackerMobile/ViewModels/DashboardViewModel.cs` — Advocate KPI metrics, cause list schedule, search filter, and logout command.
11. `CaseTrackerMobile/Views/LoginPage.xaml` & `RegisterPage.xaml` — Integrated Shadcn components and compiled bindings.
12. `CaseTrackerMobile/Views/DashboardPage.xaml` & `DashboardPage.xaml.cs` — Advocate Dashboard built 100% with Shadcn custom components.
13. `CaseTrackerMobile/AppShell.xaml` & `AppShell.xaml.cs` — Registered `Login`, `Register`, `Dashboard` Shell routes.
14. `CaseTrackerInfrastructure/Data/DbInitializer.cs` — Seeded dummy advocate (`9876543210` / `Password123!`).
15. `Documents/docs/CHANGES_BY_BALAJI.md` — Documented architectural changes by Balaji with date and usecases.

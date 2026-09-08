# ADR-0003: .NET MAUI Custom Shadcn-Inspired Design System

> **Status:** Accepted  
> **Date:** 2026-09-05  
> **Deciders:** Balaji, Frontend Mobile Team  
> **Parent Documentation:** [Mobile Architecture](../../mobile/01_maui_architecture.md)

---

## Context
Default .NET MAUI controls render native platform artifacts (e.g. thick underlines on Android entries, native borders on iOS/Windows) that cause inconsistent brand appearance and layout jitter. Third-party UI component suites often add significant binary bloat, license fees, and sluggish rendering.

Advocates require a crisp, professional, high-contrast, zero-lag interface for court corridors and mobile devices.

---

## Decision
We implemented a lightweight, custom **Shadcn-inspired Design System** natively in XAML and C#:
1. Created reusable atomic controls under `CaseTrackerMobile/Controls`:
   - `ShadcnEntry`: Custom rounded input with inline error validation and password visibility toggle.
   - `ShadcnButton`: Primary action button with touch-scale spring animations.
   - `ShadcnCard`, `ShadcnBadge`, and `ShadcnMetricCard`.
2. Stripped platform-specific underlines and native borders globally via `EntryHandler` and `PickerHandler` in `MauiProgram.cs`.
3. Integrated `CommunityToolkit.Mvvm.ComponentModel.ObservableValidator` for keystroke-level validation feedback without lagging the main thread.

---

## Consequences

### Positive
* **Unified Cross-Platform Polish**: Identical, modern look and feel across Android, iOS, Windows, and macOS.
* **Zero License Overhead**: 100% custom lightweight controls without third-party commercial dependencies.
* **Instant Validation**: Immediate visual feedback for invalid mobile numbers and Bar Council registration IDs.

### Negative
* Custom controls must be maintained internally as new input types (date pickers, dropdown selects) are introduced.


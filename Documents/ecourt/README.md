# eCourtsIndia API Integration Master Guide (v4.0)

Welcome to the comprehensive technical documentation for integrating the **eCourtsIndia Partner REST API** into the CaseTracker backend.

---

## 1. Executive Summary

| Attribute | Specification |
| :--- | :--- |
| **Base URL** | `https://webapi.ecourtsindia.com` |
| **Model Context Protocol (MCP)** | `https://mcp.ecourtsindia.com/mcp` (Remote streamable-HTTP) |
| **Statute API (IndiaCode)** | `https://indiacode.ecourtsindia.com/api/v1` |
| **API Version** | v4.0 |
| **Protocol & Data Format** | HTTPS / JSON |
| **Authentication** | Bearer Token (`Authorization: Bearer eci_live_...`) |
| **Total Endpoints** | 22 Endpoints |
| **Courts Covered** | Supreme Court of India, 25 High Courts, 700+ District Courts, NCLT, NCLAT, and Tribunals (28+ Crore cases) |
| **Free Tier** | ₹200 in free credits upon registration (no credit card required) |

---

## 2. Directory Structure

This documentation suite is split into modular, focused files:

```
document/ecourt/
├── README.md                           # Master Index & Quick Reference (This file)
├── 01_ARCHITECTURE_AND_AUTH.md         # Auth, Envelopes, Pricing, Rate Limits & Errors
├── 02_CASES_AND_ORDERS_API.md          # Case Detail, Search, Capabilities, Orders (PDF/AI/MD), Refreshes
├── 03_CAUSELIST_API.md                 # Court Structure Hierarchy, Causelist Search, Batch CNR Check, Dates
├── 04_LEGALCHECK_API.md                # Async Background Litigation Screening (Submit, Status, Report, Models)
├── 05_ELECTORAL_AND_STATUTE_API.md     # Electoral Roll Search, EPIC Lookup & IndiaCode Statute API
├── 06_GOTCHAS_AND_BEST_PRACTICES.md    # Critical Pitfalls, Enum Traps & C# Integration Guidelines
└── ecourtsindia_postman_collection.json # Complete Postman Collection v2.1.0 with all 22 endpoints
```

---

## 3. Quick Reference: All 22 Endpoints & Costs

| Module | Method | Endpoint Path | Billing / Credits | Description |
| :--- | :--- | :--- | :--- | :--- |
| **Cases** | `GET` | `/api/partner/case/{cnr}` | Paid (Credits) | Complete case timeline, parties, judges, orders, IAs |
| **Cases** | `GET` | `/api/partner/search` | Paid (Credits) | Solr full-text & faceted search across 24Cr+ records |
| **Cases** | `GET` | `/api/partner/search/capabilities` | **FREE** | Catalog of sortable, facetable, and projectable fields |
| **Cases** | `GET` | `/api/partner/case/{cnr}/order/{filename}` | Paid (Credits) | Download certified true-copy PDF or unsigned raw court PDF |
| **Cases** | `GET` | `/api/partner/case/{cnr}/order-ai/{filename}` | Paid (Credits) | On-demand OCR + AI extraction, ratio decidendi, statutes cited |
| **Cases** | `GET` | `/api/partner/case/{cnr}/order-md/{filename}` | Paid (Credits) | Real-time Markdown conversion & base64 watermarked PDF |
| **Cases** | `POST` | `/api/partner/case/{cnr}/refresh` | Paid (Credits) | Queue single case scrape from live court servers (5-10s) |
| **Cases** | `POST` | `/api/partner/case/bulk-refresh` | Paid (Credits) | Queue re-scrape for 2–50 CNRs in one call |
| **Cases** | `POST` | `/api/partner/case/bulk-refresh-status` | **FREE** | Check status of refreshes; auto-refunds failed CNRs |
| **Cases** | `GET` | `/api/partner/enums` | **FREE** | Live enum lookup dictionary (status, types, benches) |
| **Causelist** | `GET` | `/api/partner/causelist/court-structure/*` | **FREE** (Auth Req.) | Hierarchy: States → Districts → Complexes → Courts |
| **Causelist** | `GET` | `/api/partner/causelist/search` | Paid (Credits) | Search daily cause lists by case number, judge, advocate, date |
| **Causelist** | `POST` | `/api/partner/causelist/cnr/batch` | Paid (Per valid CNR) | Check upcoming cause list listings for up to 100 CNRs |
| **Causelist** | `GET` | `/api/partner/causelist/available-dates` | **FREE** (Auth Req.) | Discover which dates have cause list listings |
| **Electoral** | `GET` | `/api/partner/electoral/search` | Paid (Credits) | Search Indian voter roll by name, epic, relative, household |
| **Electoral** | `GET` | `/api/partner/electoral/epic/{epic}` | Paid (Credits) | Exact EPIC (Voter ID) lookup grouped across revisions |
| **Electoral** | `GET` | `/api/partner/electoral/capabilities` | **FREE** | Catalog of supported electoral facets and filters |
| **LegalCheck** | `POST` | `/api/partner/legal-check` | ₹99 PAYG / ₹33 Sub | Submit background litigation check (Individual/Company) |
| **LegalCheck** | `GET` | `/api/partner/legal-check/{code}` | **FREE** | Poll progress, stage, and safe counters (`Retry-After: 5`) |
| **LegalCheck** | `GET` | `/api/partner/legal-check/{code}/report` | **FREE** | Full risk report with match confidence & risk bands |
| **LegalCheck** | `GET` | `/api/partner/legal-check` | **FREE** | List all partner legal checks with filters & pagination |
| **LegalCheck** | `GET` | `/api/partner/legal-check/models` | **FREE** | List accepted LegalCheck models (e.g., `eCI-1.2`) |

---

## 4. Immediate Checklist Before Writing Code

1. **API Key Setup**: Store your `eci_live_...` key in `appsettings.json` or environment variables (`ECOURTS_API_KEY`).
2. **Handle Enum Codes Correctly**:
   - High Court codes **MUST** include bench suffix (e.g. `DLHC01`, not `DLHC`).
   - NCLT codes **MUST** end with `0` (e.g. `NCLTDL0`).
   - Passing invalid codes returns **0 results silently** with no HTTP error.
3. **Pagination Rules**:
   - Case Search uses `page` (1-indexed) & `pageSize` (max 200 for partners).
   - Cause List Search uses `offset` & `limit` (max 200).
4. **Order Document Downloads**:
   - Use the bare filename from `judgmentOrders[].orderUrl` (e.g. `order-1.pdf`), not the CNR-prefixed filename in `files.files[]`.
5. **Always Log Request ID**:
   - Every response contains `meta.request_id`. Always log this in your database/logs for diagnostics and support.

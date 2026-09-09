# 02. Cases and Orders API Specification

---

## 1. Case Detail (`GET /api/partner/case/{cnr}`)

Retrieves complete docket records, party names, advocates, hearing histories, interim orders, judgments, and interlocutory applications (IAs) for a specific case.

### Parameters
* **Path**: `cnr` (string, Required) — 16-character Case Number Record (e.g., `DLND020047882015`).

### Request Example
```bash
curl -X GET "https://webapi.ecourtsindia.com/api/partner/case/DLND020047882015" \
  -H "Authorization: Bearer eci_live_your_token_here"
```

### Key Response Fields Explained
* `courtCaseData.cnr`: 16-character identifier.
* `courtCaseData.caseNumber` / `registrationNumber`: Court registration numbers.
* `courtCaseData.petitioners`: `string[]` — Plain text array of petitioner names (NOT an object array).
* `courtCaseData.respondents`: `string[]` — Plain text array of respondent names.
* `courtCaseData.petitionerAdvocates` & `respondentAdvocates`: `string[]` — Counsel names.
* `courtCaseData.historyOfCaseHearings`: Array containing `judge`, `businessOnDate`, `hearingDate`, and `purposeOfListing`.
* `courtCaseData.businessOnDateEntries`: Daily proceedings, court orders, and reason for adjournment entries.
* `courtCaseData.judgmentOrders`: Array containing `orderDate`, `orderType`, and `orderUrl` (e.g., `order-10.pdf`).
* `courtCaseData.interimOrders`: Array containing interim order files.
* `files.files[].markdownContent`: **Full order text** in Markdown (5–40+ pages) already included in the case payload.

---

## 2. Case Search (`GET /api/partner/search`)

Solr-backed search across 24Cr+ cases with multi-value filtering, fuzzy name matching, faceted aggregation, and field projection.

### Query Parameters

| Parameter | Type | Required | Description |
| :--- | :--- | :--- | :--- |
| `query` | string | No | General full-text search across all case metadata and text. |
| `advocates` | string[] | No | Filter by advocate name. Repeat key for OR matching: `advocates=Sharma&advocates=Verma`. |
| `judges` | string[] | No | Filter by judge name. |
| `petitioners`| string[] | No | Filter by petitioner name. |
| `respondents`| string[] | No | Filter by respondent name. |
| `litigants` | string[] | No | Search both petitioner and respondent fields simultaneously. |
| `nameMatchMode`| string | No | `all` (default, all tokens must match), `any`, `phrase`, `fuzzy` (edit distance 1). |
| `courtCodes` | string[] | No | Full court establishment codes with bench suffix (e.g., `DLHC01`, `HCBM01`). |
| `caseStatuses`| string[] | No | Valid enum codes: `PENDING`, `DISPOSED`, `DISMISSED`. |
| `caseTypes` | string[] | No | Valid case type codes (e.g., `WP_C`, `CC`, `BA`, `RFA`). |
| `filingDateFrom` / `To` | date | No | Filing date bounds in `YYYY-MM-DD`. |
| `decisionDateFrom` / `To`| date | No | Disposal date bounds in `YYYY-MM-DD`. |
| `hasOrders` | boolean | No | Only return cases with orders (`true`/`false`). |
| `hasJudgments` | boolean | No | Only return cases with final judgments (`true`/`false`). |
| `page` | int | No | Page number (starts at 1). Default: `1`. |
| `pageSize` | int | No | Results per page (1 to 200 for partner accounts). Default: `20`. |
| `sortBy` | string | No | Any sortable field from capabilities (e.g., `decisionDate`, `filingDate`, `score`). |
| `sortOrder` | string | No | `asc` or `desc`. Default: `desc`. |

### Request Example
```bash
curl -X GET "https://webapi.ecourtsindia.com/api/partner/search?advocates=Sharma&courtCodes=DLHC01&caseStatuses=PENDING&pageSize=20" \
  -H "Authorization: Bearer eci_live_your_token_here"
```

### Critical Rules for Search
1. **No Case Title Field**: The API does **not** return a pre-concatenated `title`. Synthesize it in code: `string.Join(", ", caseObj.Petitioners) + " vs " + string.Join(", ", caseObj.Respondents)`.
2. **Array Parameter Passing**: Pass arrays by repeating query keys: `?courtCodes=DLHC01&courtCodes=HCBM01`. Commas inside values are treated literally.

---

## 3. Search Capabilities (`GET /api/partner/search/capabilities`)

Machine-readable dictionary of supported search fields, limits, and operators. **Cost: 0 Credits (Free to call).**

### Response Schema Summary
```json
{
  "data": {
    "sortableFields": ["score", "decisionDate", "filingDate", "caseDurationDays", "hearingCount", ...],
    "facetableFields": ["caseType", "caseStatus", "courtCode", "stateCode", "filingYear", ...],
    "projectableFields": ["cnr", "petitioners", "respondents", "advocates", "judges", ...],
    "existsFilterableFields": ["filingDate", "hasOrders", "hasJudgments", ...],
    "nameMatchModes": ["all", "any", "phrase", "fuzzy"],
    "courtLevels": ["SC", "HC", "DC", "TRIBUNAL"],
    "partnerMaxPageSize": 200,
    "maxFacetValues": 1000
  }
}
```

---

## 4. Order PDF Download (`GET /api/partner/case/{cnr}/order/{filename}`)

Downloads court order document metadata and download link. Returns digitally signed, watermarked certified true copies by default.

### Parameters
* **Path**: `cnr` (string, Required) — 16-character CNR.
* **Path**: `filename` (string, Required) — Bare filename from `judgmentOrders[].orderUrl` (e.g., `order-1.pdf`).
* **Query**: `signed` (boolean, Optional) — Default `true`. Set `false` to download raw court PDF without watermarking or digital certification.

### Response
```json
{
  "data": {
    "cnr": "DLHC010001232024",
    "filename": "order-1.pdf",
    "downloadFilename": "ecourtsindia-truecopy-DLHC010001232024-order-1.pdf",
    "message": "Use the standard document download endpoint to retrieve the PDF"
  },
  "meta": { "request_id": "550e8400-e29b-41d4-a716-446655440001" }
}
```

---

## 5. Order + AI Analysis (`GET /api/partner/case/{cnr}/order-ai/{filename}`)

Performs on-demand OCR and AI legal analysis on a specific order.

### Performance & Latency Warning
* **Initial Access**: Takes **10–60 seconds** on the first call to fetch and process from court nodes.
* **Subsequent Accesses**: Instantaneously served from cache.
* **Retry Guidance**: If `data.aiAnalysis` is `null`, retry up to 3 times with 15–30 second intervals.

### Extracted Analysis Elements
* `intelligent_insights_analytics.ai_generated_executive_summary`: Concise summary for lawyers.
* `plain_language_summary_for_litigants_outcome_focused`: Layman summary.
* `deep_legal_substance_context.core_legal_content_analysis.statutes_cited_and_applied`: Array of statutes and specific sections applied.
* `ratio_decidendi_extracted`: The binding legal principle of the decision with confidence score.
* `procedural_details_from_order.disposition_outcome_if_disposed`: Outcome (e.g., `Allowed`, `Dismissed`, `Disposed`).

---

## 6. Order Markdown & PDF (`GET /api/partner/case/{cnr}/order-md/{filename}`)

Real-time PDF conversion pipeline returning both raw markdown text and Base64-encoded PDF.

* **Latency**: Can take up to **300 seconds**.
* **Best Practice**: If you only need order text, read `files[].markdownContent` from the Case Detail response instead. Only call this endpoint if you require the Base64 signed PDF.

---

## 7. Case Refresh (`POST /api/partner/case/{cnr}/refresh`)

Triggers an asynchronous re-scrape of the case record directly from the official court CIS servers.

### Request Example
```bash
curl -X POST "https://webapi.ecourtsindia.com/api/partner/case/DLHC010001232024/refresh" \
  -H "Authorization: Bearer eci_live_your_token_here"
```

* **HTTP Status**: `202 Accepted`
* **Idempotency**: Duplicate calls within **15 seconds** are idempotent and not duplicate-charged.
* **Completion Window**: Fresh data is typically available in Case Detail within 5–10 seconds.

---

## 8. Bulk Case Refresh (`POST /api/partner/case/bulk-refresh`)

Queues 2 to 50 CNRs for live court re-scraping in a single request.

### Request Body
```json
{
  "cnrs": [
    "DLHC010024442025",
    "DLHC010351552024",
    "HCBM050012342023"
  ]
}
```

* **Constraints**: 2 to 50 CNRs per batch.
* **Response**: Categorizes CNRs into `refreshed` (already updated), `queued` (sent for scrape), and `invalid`.

---

## 9. Bulk Refresh Status (`POST /api/partner/case/bulk-refresh-status`)

Checks progress of queued refreshes. **Cost: 0 Credits (Free to call).**

### Request Body
```json
{
  "cnrs": [
    "DLHC010024442025",
    "DLHC010351552024"
  ]
}
```

### Status Values
* `PENDING`: Refresh currently scraping.
* `COMPLETED`: Fresh data is now live and queryable via `/api/partner/case/{cnr}`.
* `FAILED`: Re-scrape failed (`CNR_NOT_FOUND`, `TEMPORARY_ERROR`, `TIMEOUT`). Credits are automatically refunded (`refunded: true`).
* `NOT_REQUESTED`: You never submitted a refresh request for this CNR.

---

## 10. Live Enum Reference (`GET /api/partner/enums`)

Returns authoritative dictionaries for case statuses, case types, court codes, bench types, and judicial sections. **Cost: 0 Credits (Free to call).**

### Query Parameters
* `types` (string, Optional): Comma-separated enum categories (e.g., `caseStatus,benchType,caseType`). Omitting returns all.

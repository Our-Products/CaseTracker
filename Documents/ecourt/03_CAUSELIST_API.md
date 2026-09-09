# 03. Cause Lists & Court Structure API Specification

---

## 1. Court Structure Hierarchy (`GET /api/partner/causelist/court-structure/*`)

Discovers the hierarchical structure of Indian courts: **State → District → Court Complex → Court**. Used to retrieve valid codes for filtering daily cause lists.

> **CRITICAL BREAKING CHANGE**:
> The old unauthenticated path `/api/CauseList/court-structure/*` is decommissioned for partners. You **must** use `/api/partner/causelist/court-structure/*` with your `Authorization: Bearer <API_KEY>` header.
> **Cost: 0 Credits (Free to call).**

### 1.1 List States
```http
GET /api/partner/causelist/court-structure/states
```
**Sample Response:**
```json
[
  { "state": "DL", "stateName": "Delhi" },
  { "state": "UP", "stateName": "Uttar Pradesh" },
  { "state": "SC", "stateName": "India" }
]
```
*(Note: `SC` represents the Supreme Court of India, grouped under states as "India").*

### 1.2 List Districts in a State
```http
GET /api/partner/causelist/court-structure/states/{state}/districts
```
**Sample Response (`state=UP`):**
```json
[
  { "districtCode": "1", "districtName": "Prayagraj" },
  { "districtCode": "HC", "districtName": "Allahabad High Court" }
]
```
*(Note: High Courts appear as a special district with `districtCode="HC"`).*

### 1.3 List Court Complexes in a District
```http
GET /api/partner/causelist/court-structure/states/{state}/districts/{districtCode}/complexes
```
**Sample Response:**
```json
[
  { "courtComplexCode": "1130029", "courtComplexName": "Kheri District Court Complex" }
]
```

### 1.4 List Individual Courts in a Complex
```http
GET /api/partner/causelist/court-structure/states/{state}/districts/{districtCode}/complexes/{complexCode}/courts
```
**Sample Response:**
```json
[
  {
    "court": "11",
    "courtNo": "1",
    "courtName": "SUSHRI KAPILA RAGHAV-Presiding Officer MACT",
    "courtDivision": "Presiding Officer Motor Accident Claim Tribunal",
    "judgeName": "SUSHRI KAPILA RAGHAV"
  }
]
```

---

## 2. Cause List Search (`GET /api/partner/causelist/search`)

Search daily hearing dockets across Indian courts by date, advocate, judge, litigant, or case number.

### Query Parameters

| Parameter | Type | Required | Description |
| :--- | :--- | :--- | :--- |
| `q` | string | No* | Full-text query. Also accepts **case numbers directly** (e.g., `G.R.case/533/2023`). |
| `date` | date | No* | Exact hearing date in `YYYY-MM-DD`. |
| `startDate` | date | No* | Inclusive range start (`YYYY-MM-DD`). |
| `endDate` | date | No* | Inclusive range end (`YYYY-MM-DD`). |
| `judge` | string | No* | Search by judge name (full-text). |
| `advocate` | string | No* | Search by advocate name (full-text). |
| `litigant` | string | No* | Search by party/litigant name (full-text). |
| `state` | string | No* | 2-letter state code (e.g., `DL`, `JH`, `MH`, `SC`). |
| `districtCode` | string | No | District code from Court Structure (e.g., `1`, `HC`). |
| `courtComplexCode` | string | No | Court complex code (e.g., `1260010-1`). |
| `court` | string | No | Internal court identifier from Court Structure. |
| `courtNo` | string | No | Physical court room number (e.g., `16`). |
| `bench` | string | No | Bench identifier. |
| `listType` | string | No | `CIVIL` or `CRIMINAL`. |
| `includeCourtroom` | boolean| No | Include courtroom assignment data (default: `true`). |
| `limit` | int | No | Results per page (max: 200, default: `100`). |
| `offset` | int | No | Records to skip for pagination (default: `0`). |

*\*At least one search or filter parameter must be supplied.*

### Pagination Pattern: Offset-Based
Cause List Search uses **`offset` pagination**, NOT page numbers:
* **Page 1**: `?limit=20&offset=0`
* **Page 2**: `?limit=20&offset=20`
* **Next Page Formula**: `next_offset = current_offset + limit`
* **End Condition**: If `data.returnedCount < limit`, you have fetched the last page.

### Searching Directly by Case Number in `q`
You can look up a specific case in the cause list by providing its case number to `q`. Remember to URL-encode slashes (`/` → `%2F`):
```bash
curl -X GET "https://webapi.ecourtsindia.com/api/partner/causelist/search?q=G.R.case%2F533%2F2023&state=JH&limit=10" \
  -H "Authorization: Bearer eci_live_your_token_here"
```

### Key Field Distinction: `courtNo` vs `court`
* `courtNo`: Physical room number (e.g., `"1"`).
* `court`: Internal court establishment identifier (e.g., `"11"`).

---

## 3. Batch CNR Cause List Check (`POST /api/partner/causelist/cnr/batch`)

Checks for upcoming cause-list hearings across a portfolio of up to **100 CNRs** in a single call.

### Request Body
```json
{
  "cnrs": [
    "JHBO010001232024",
    "DLND020047882015",
    "MHAU030001112023"
  ]
}
```

* **Constraints**: 1 to 100 CNRs per batch. Exceeding 100 returns `400 BATCH_SIZE_EXCEEDED`.
* **Billing**: Billed **per unique valid CNR** processed (duplicates and blanks are ignored and not double-billed).

### Sample Response
```json
{
  "data": [
    {
      "cnr": "JHBO010001232024",
      "hasCauselist": true,
      "nextListing": {
        "id": 8506606,
        "court": "1",
        "courtType": "DISTRICT_COURT",
        "listType": "CRIMINAL",
        "listingFor": "EVIDENCE",
        "courtNo": "78",
        "date": "2026-07-08",
        "caseNumber": ["G.R.case/533/2023"],
        "party": "The State Of Jharkhand Vs. Dilip Ram",
        "judge": ["Manoj Kumar Prajapati"],
        "listingNo": 3,
        "courtName": "PRINCIPAL MAGISTRATE JJB"
      }
    },
    {
      "cnr": "DLND020047882015",
      "hasCauselist": false,
      "nextListing": null
    }
  ],
  "meta": { "request_id": "550e8400-e29b-41d4-a716-446655440012" }
}
```

---

## 4. Cause List Available Dates (`GET /api/partner/causelist/available-dates`)

Discovers which upcoming/past dates have published cause lists for a given court location.
**Cost: 0 Credits (Free to call, Bearer token required).**

### Query Parameters (At least one required)
* `state`: State code (e.g., `DL`).
* `districtCode`: District code.
* `courtComplexCode`: Complex code.
* `court`: Court ID.
* `courtNo`: Room number.

### Sample Response
```json
{
  "data": [
    "2026-09-08",
    "2026-09-07",
    "2026-09-05"
  ],
  "meta": { "request_id": "a3bb189e-8bf9-3888-9912-ace4e6543002" }
}
```
Dates are returned in descending chronological order (`YYYY-MM-DD`).

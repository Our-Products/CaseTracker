# 04. LegalCheck (Litigation Background Screening) API

---

## 1. Overview & Lifecycle

**LegalCheck** is an automated litigation background check and due-diligence engine. It screens individuals and corporate entities across India's judicial databases to detect civil disputes, criminal proceedings, insolvency petitions (NCLT), recovery suits (DRT), and consumer complaints.

```
┌────────────────────────┐      Returns 202 Accepted       ┌────────────────────────┐
│ 1. Submit Subject      ├────────────────────────────────►│  data.code: "LC-XXXX"  │
│ POST /partner/legal-ck │                                 │  status_url & report_url│
└────────────────────────┘                                 └───────────┬────────────┘
                                                                       │
┌────────────────────────┐      Poll every 5 seconds                   │
│ 2. Check Status        │◄────────────────────────────────────────────┘
│ GET /legal-check/{code}│      (Honor Retry-After: 5)
└───────────┬────────────┘
            │
            ▼ status == "completed"
┌────────────────────────┐
│ 3. Fetch Full Report   │
│ GET .../{code}/report  │
└────────────────────────┘
```

### Key Lifecycle Principles
1. **Asynchronous Processing**: LegalCheck takes ~60–90 seconds. Submitting returns `202 Accepted` immediately with a resource code (e.g., `LC-A1B2C3D`).
2. **Billing**:
   - **Cost**: **₹99 on PAYG** or **₹33 with an active subscription** per accepted submission.
   - **Free Operations**: All status checks, report retrievals, job listing, and model checks are **0 Credits (Free)**.
   - **Refunds**: Terminal processing failures (`FAILED`) are automatically refunded to your credit pool.
3. **Queue Limit**: At most **3 jobs** may be queued or running simultaneously per partner token. A 4th concurrent submission returns `429 LEGAL_CHECK_QUEUE_FULL`.
4. **Idempotent Submissions**: Always send an `Idempotency-Key: <unique-id>` header. If network times out, retrying with the same key and same body returns the original resource without double-charging. Retrying with the same key but a different body returns `409 IDEMPOTENCY_CONFLICT`.

---

## 2. Submit Legal Check (`POST /api/partner/legal-check`)

### Headers
* `Authorization`: `Bearer eci_live_...`
* `Content-Type`: `application/json`
* `Idempotency-Key`: string (1–255 chars, e.g., `vendor-1042-attempt-1`)

### Request Body Fields
* `subject_type` (string, Required): `individual` or `company`.
* **When `subject_type == "individual"`**:
  * `subject.name` (string, Required): Full name of the person.
  * `subject.father_name` (string, Required): Father or spouse name (critical for disambiguation).
  * `subject.aliases` (string[], Optional): Alternate names.
  * `subject.date_of_birth` (date, Optional): `YYYY-MM-DD`.
  * `subject.addresses` (string[], Optional): City/state addresses.
  * `subject.gender` (string, Optional): `male` / `female` / `other`.
* **When `subject_type == "company"`**:
  * `subject.company_name` (string, Required): Registered corporate entity name.
  * `subject.directors` (string[], Optional): Names of known directors.
  * `subject.registered_addresses` (string[], Optional): Corporate office addresses.
  * `subject.cin` / `gst` / `pan` (string, Optional): Corporate tax/registration IDs.
* `config.model` (string, Optional): Model ID. Default: `eCI-1.2`.
* `config.purpose` (string, Optional): Business purpose (e.g., `vendor_onboarding`, `employment_screening`).
* `config.client_ref_no` (string, Optional): Your internal reference ID (e.g., `VENDOR-1042`).

### Individual Check Example
```json
{
  "subject_type": "individual",
  "subject": {
    "name": "Amit Kumar",
    "father_name": "Ramesh Kumar",
    "aliases": ["Amit R Kumar"],
    "date_of_birth": "1988-04-12",
    "addresses": ["Patna, Bihar"],
    "gender": "male"
  },
  "config": {
    "model": "eCI-1.2",
    "purpose": "vendor_onboarding",
    "client_ref_no": "VENDOR-1042"
  }
}
```

### Submission Response (`202 Accepted`)
```json
{
  "data": {
    "code": "LC-A1B2C3D",
    "status": "queued",
    "model": "eCI-1.2",
    "status_url": "/api/partner/legal-check/LC-A1B2C3D",
    "report_url": "/api/partner/legal-check/LC-A1B2C3D/report",
    "estimated_seconds": 90
  },
  "meta": { "request_id": "req_abc123xyz" }
}
```

---

## 3. Legal Check Status (`GET /api/partner/legal-check/{code}`)

Polls current progress of the screening job. **Cost: 0 Credits (Free to call).**

### Parameters
* **Path**: `code` (string, Required) — Resource code (e.g., `LC-A1B2C3D`).

### Response Example
```json
{
  "data": {
    "code": "LC-A1B2C3D",
    "status": "running",
    "model": "eCI-1.2",
    "current_stage": "court_search",
    "progress": { "completed": 2, "total": 5 },
    "queued_at": "2026-09-02T12:00:00+00:00",
    "started_at": "2026-09-02T12:00:05+00:00",
    "completed_at": null,
    "client_ref_no": "VENDOR-1042",
    "status_url": "/api/partner/legal-check/LC-A1B2C3D",
    "report_url": "/api/partner/legal-check/LC-A1B2C3D/report",
    "error": null
  },
  "meta": { "request_id": "req_abc123xyz" }
}
```

### Polling Guideline
* If `status` is `queued` or `running`, wait according to the `Retry-After: 5` header before polling again.
* Once `status == "completed"`, proceed to fetch the report.

---

## 4. Legal Check Report (`GET /api/partner/legal-check/{code}/report`)

Retrieves the structured litigation screening report. **Cost: 0 Credits (Free to call).**

> **Note**: If called while status is still `queued` or `running`, it returns `409 NOT_READY`.

### Query Parameters
* `verbosity` (string, Optional):
  * `compact`: Only confirmed matches.
  * `standard` (Default): Confirmed and probable matches with summaries.
  * `full`: Complete uncapped lists and all order metadata.
* `bands` (string, Optional): Comma-separated risk bands to filter by (e.g., `high,medium`).
* `min_score` (int, Optional): Minimum match score between 0 and 100 (Default: `40`).
* `include` (string, Optional): Set to `excluded` to inspect rejected candidates.

### Report Payload Anatomy
```json
{
  "data": {
    "schema_version": "legal-check.v1",
    "code": "LC-A1B2C3D",
    "status": "completed",
    "risk": {
      "band": "MEDIUM",
      "headline": "MEDIUM court-record risk from 1 attributed or candidate matter.",
      "identity_confidence": 84,
      "coverage_confidence": 78,
      "search_confidence": 81
    },
    "summary": {
      "total_matches": 1,
      "confirmed_matches": 0,
      "probable_matches": 1,
      "records_screened": 24,
      "courts_covered": ["Patna High Court"]
    },
    "matches": [
      {
        "cnr": "BRHC010012342024",
        "match_band": "probable",
        "match_confidence": 84,
        "risk_band": "medium",
        "role_group": "against_subject",
        "court": {
          "name": "Patna High Court",
          "code": "BRHC01",
          "type": "high_court"
        },
        "case_type": "CWJC",
        "status": "PENDING",
        "parties": {
          "petitioners": ["Example Industries Ltd"],
          "respondents": ["Amit Kumar"]
        }
      }
    ],
    "disclaimer": "Results are based on public court records and probabilistic identity matching. Verify identity and court records before adverse action."
  }
}
```

---

## 5. List Legal Checks (`GET /api/partner/legal-check`)

Lists previously submitted legal check jobs with pagination and status filters. **Cost: 0 Credits (Free to call).**

### Query Parameters
* `page` (int, Default: `1`)
* `page_size` (int, 1–100, Default: `20`)
* `status` (string, Optional): `queued`, `running`, `completed`, `failed`
* `from` / `to` (date, Optional): `YYYY-MM-DD`
* `client_ref_no` (string, Optional): Filter by your reference number.

---

## 6. Supported Models (`GET /api/partner/legal-check/models`)

Lists supported legal check engines. **Cost: 0 Credits (Free to call).**
Currently active model: `eCI-1.2` for both `individual` and `company` subjects.

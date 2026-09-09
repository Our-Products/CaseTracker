# 01. Architecture, Authentication, Pricing & Errors

---

## 1. System Architecture

The eCourtsIndia platform provides unified REST APIs over India's multi-tiered judicial database:

```
                  ┌─────────────────────────────────────────┐
                  │      eCourtsIndia Partner API v4.0      │
                  │     https://webapi.ecourtsindia.com     │
                  └────────────────────┬────────────────────┘
                                       │
         ┌─────────────────────────────┼─────────────────────────────┐
         │                             │                             │
┌────────▼─────────┐          ┌────────▼─────────┐          ┌────────▼─────────┐
│ Cases & Orders   │          │   Cause Lists    │          │    LegalCheck    │
│ • Supreme Court  │          │ • Court Tree     │          │ • Individual /   │
│ • 25 High Courts │          │ • Daily Schedule │          │   Company Risk   │
│ • 700+ Districts │          │ • Batch Check    │          │ • Probabilistic  │
│ • Tribunals      │          │ • Hearing Dates  │          │   Identity Match │
└──────────────────┘          └──────────────────┘          └──────────────────┘
```

### Core Service Components:
1. **Case Engine**: Solr-indexed search engine containing 28+ Crore cases and orders from official CIS (Case Information System) nodes.
2. **Order Pipeline**: Document fetcher delivering original PDFs, certified digitally signed true copies, real-time Markdown conversion, and OCR extraction.
3. **AI Pipeline**: LLM-based ratio decidendi extractor, statute classifier, and summarizer.
4. **LegalCheck Service**: Asynchronous background litigation scoring engine with confidence bands.

---

## 2. Authentication

All requests to the partner API (except where explicitly marked as public) require HTTP Bearer token authentication in the `Authorization` header.

### Bearer Token Format
```http
Authorization: Bearer eci_live_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
```

* **Prefix**: `eci_live_`
* **Length**: 41 characters
* **Format**: Alphanumeric string
* **Scope**: Scoped to the partner account. Resources generated (like LegalCheck jobs or refresh statuses) are private to the token owner.

### C# HttpClient Authentication Example
```csharp
using System.Net.Http.Headers;

var client = new HttpClient
{
    BaseAddress = new Uri("https://webapi.ecourtsindia.com/")
};

client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", "eci_live_your_actual_token_here");
client.DefaultRequestHeaders.Accept.Add(
    new MediaTypeWithQualityHeaderValue("application/json"));
```

---

## 3. Response Envelope Structure

Every API response adheres to a strict standard envelope:

### Successful Response (HTTP 200 / 202)
```json
{
  "data": {
    /* Payload object or array */
  },
  "meta": {
    "request_id": "400006a5-0010-d800-b63f-84710c7967bb"
  }
}
```

### Error Response (HTTP 4xx / 5xx)
```json
{
  "error": {
    "code": "INVALID_CNR",
    "message": "CNR format is invalid",
    "details": []
  },
  "meta": {
    "request_id": "req_abc123xyz"
  }
}
```

> **IMPORTANT**: Always extract and log `meta.request_id`. Support inquiries, billing reconciliations, and ticket resolutions require this identifier.

---

## 4. Pricing, Credits & Billing Model

eCourtsIndia uses a credit-based billing system:

### Free Tier
- Every new developer account receives **₹200 in free trial credits** upon signing up at `ecourtsindia.com`.
- No credit card is required to sign up.
- Academic and research usage can request grant allowances.

### Endpoints Cost Breakdown

| Endpoint Category | Endpoints | Cost / Billing |
| :--- | :--- | :--- |
| **Completely Free (No Credits Charged)** | • `GET /api/partner/enums`<br>• `GET /api/partner/search/capabilities`<br>• `GET /api/partner/electoral/capabilities`<br>• `GET /api/partner/causelist/court-structure/*`<br>• `GET /api/partner/causelist/available-dates`<br>• `POST /api/partner/case/bulk-refresh-status`<br>• `GET /api/partner/legal-check/{code}`<br>• `GET /api/partner/legal-check/{code}/report`<br>• `GET /api/partner/legal-check`<br>• `GET /api/partner/legal-check/models` | **0 Credits** (Metadata, polling, and status reads are free) |
| **Case & Order Operations** | • `GET /api/partner/case/{cnr}`<br>• `GET /api/partner/search`<br>• `GET /api/partner/case/{cnr}/order/*`<br>• `GET /api/partner/case/{cnr}/order-ai/*`<br>• `GET /api/partner/case/{cnr}/order-md/*` | Standard credit deduction per successful query. |
| **Case Refresh Operations** | • `POST /api/partner/case/{cnr}/refresh`<br>• `POST /api/partner/case/bulk-refresh` | Charged per refresh request. If a refresh fails (`CNR_NOT_FOUND`, `TEMPORARY_ERROR`, `TIMEOUT`), credits are **automatically refunded** (`refunded: true`). |
| **Batch Cause List Check** | • `POST /api/partner/causelist/cnr/batch` | Billed **per unique, valid CNR** processed (duplicate and blank values in batch are collapsed and not double-charged). |
| **LegalCheck Submission** | • `POST /api/partner/legal-check` | **₹99 on PAYG** or **₹33 with active subscription** per accepted submission.<br>Terminal failures are **refunded once** to the original credit balance. |

---

## 5. Rate Limits & Concurrency

| Window | Limit |
| :--- | :--- |
| **Per Minute** | 100 requests |
| **Per Hour** | 3,000 requests |
| **Per Day** | 50,000 requests |
| **Concurrent Connections** | 10 simultaneous requests |
| **LegalCheck Active Queue** | Max **3 jobs** queued or running concurrently per partner |

### Handling Rate Limits in C#
When receiving `429 RATE_LIMIT_EXCEEDED` or `429 TOO_MANY_CONVERSIONS`, inspect the `Retry-After` header and apply exponential backoff:
```csharp
if (response.StatusCode == (HttpStatusCode)429)
{
    var retryAfter = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(2);
    await Task.Delay(retryAfter);
    // retry request
}
```

---

## 6. Comprehensive Error Codes Reference

| HTTP Status | Error Code | Category | Explanation |
| :--- | :--- | :--- | :--- |
| **400** | `INVALID_CNR` | Validation | CNR string does not match regex `^[A-Z]{4}\d{12}$`. |
| **400** | `INVALID_PARAMETER` | Validation | Parameter value out of bounds or bad syntax. |
| **400** | `MISSING_PARAMETER` | Validation | Mandatory parameter omitted. |
| **400** | `PAGE_SIZE_EXCEEDED` | Validation | Requested page size exceeds ceiling (200 for partner search, 100 for electoral). |
| **400** | `PAGE_EXCEEDED` | Validation | Requested page number exceeds maximum allowable window (100 for electoral). |
| **400** | `PAGE_INVALID` | Validation | Page is less than 1 (rejected rather than clamped). |
| **400** | `PAGE_SIZE_INVALID` | Validation | Page size is less than 1. |
| **400** | `MISSING_SEARCH_CRITERIA` | Electoral | Electoral search without primary criteria (`name`, `epic`, `q`, or complete household key). |
| **400** | `HOUSEHOLD_INCOMPLETE`| Electoral | Household lookup missing one of: `householdRollId`, `householdPartNumber`, `householdHouse`. |
| **400** | `HOUSEHOLD_EXCLUSIVE` | Electoral | Household lookup illegally combined with `name`, `epic`, or `q`. |
| **400** | `VALIDATION_ERROR` | LegalCheck | Malformed LegalCheck request payload. |
| **400** | `UNKNOWN_MODEL` | LegalCheck | Model in `config.model` is not supported (current valid model: `eCI-1.2`). |
| **400** | `EMPTY_REQUEST` | Validation | Bulk refresh or status called with 0 CNRs. |
| **400** | `TOO_MANY_CNRS` | Validation | More than 50 CNRs in bulk refresh or bulk refresh status. |
| **400** | `BATCH_SIZE_EXCEEDED` | Validation | More than 100 CNRs in Cause List batch check. |
| **400** | `INVALID_OFFSET` | Validation | Negative offset supplied in Cause List search. |
| **400** | `MISSING_FILTER` | Validation | Cause list available dates called without any court/location parameter. |
| **400** | `INVALID_FILENAME` | Validation | Filename contains invalid characters or CNR prefix. |
| **401** | `INVALID_TOKEN` | Auth | Bearer token format malformed or invalid. |
| **401** | `TOKEN_INACTIVE` | Auth | Token has been manually deactivated. |
| **401** | `TOKEN_EXPIRED` | Auth | Token has passed expiration date. |
| **402** | `INSUFFICIENT_CREDITS`| Billing | Account balance too low to execute request. |
| **402** | `SUBSCRIPTION_REQUIRED`| Billing | An active subscription is required to unlock this operation. |
| **403** | `ACCOUNT_INACTIVE` | Auth | Partner account suspended. |
| **404** | `CASE_NOT_FOUND` | Resource | CNR does not exist in the eCourts index. |
| **404** | `ORDER_NOT_FOUND` | Resource | Requested order document is not available on court storage. |
| **404** | `NOT_FOUND` | Resource | Resource not found or not owned by partner token. |
| **409** | `IDEMPOTENCY_CONFLICT`| LegalCheck | Same `Idempotency-Key` sent with a different body payload. |
| **409** | `NOT_READY` | LegalCheck | Report requested before job reached `completed` status. Poll status URL first. |
| **409** | `SEARCH_UNAVAILABLE` | LegalCheck | Court search pipeline temporarily unavailable during check. |
| **409** | `LLM_UNAVAILABLE` | LegalCheck | AI analysis pipeline temporarily unavailable during check. |
| **409** | `MAX_RESTARTS` | LegalCheck | Legal check exceeded internal retry ceiling. |
| **429** | `RATE_LIMIT_EXCEEDED`| Rate Limit | Request frequency exceeded account quota. |
| **429** | `TOO_MANY_CONVERSIONS`| Rate Limit | PDF conversion pipeline overloaded. Back off per `Retry-After`. |
| **429** | `LEGAL_CHECK_QUEUE_FULL`| LegalCheck | Partner already has 3 jobs queued or running. Wait for one to finish. |
| **500** | `INTERNAL_ERROR` | Server | Internal server error (unbilled/refunded). |
| **503** | `LEGAL_CHECK_UNAVAILABLE`| LegalCheck | Legal check intake temporarily paused; existing reports remain readable. |

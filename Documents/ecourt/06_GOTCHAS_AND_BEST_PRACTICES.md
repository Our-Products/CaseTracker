# 06. Critical Gotchas, Enum Traps & C# Best Practices

---

## 1. Critical Gotchas & Enum Traps

### 1.1 Wrong Enum Codes = Silent Zero Results
If you pass an invalid enum code to the search endpoint, the API does **not return a 400 error**. Instead, it executes the query and returns `0 results` silently.
* **Bad**: `GET /api/partner/search?caseTypes=BAIL_APP` → Returns 0 hits.
* **Good**: `GET /api/partner/search?caseTypes=BA` → Returns valid bail applications.
* **Rule**: Always validate codes using `GET /api/partner/enums` before passing them.

---

### 1.2 High Court Codes Must Include Bench Suffix
The live enum reference (`/api/partner/enums`) returns base court codes (e.g., `DLHC`, `HCBM`). However, the search index **requires** the full bench suffix:
* **Wrong**: `GET /api/partner/search?courtCodes=DLHC` → 0 results!
* **Correct**: `GET /api/partner/search?courtCodes=DLHC01` (Delhi High Court).
* **Common HC Codes**:
  * `DLHC01`: Delhi High Court
  * `HCBM01`: Bombay High Court (Principal Bench, Mumbai)
  * `HCBM02`: Bombay High Court (Nagpur Bench)
  * `HCBM03`: Bombay High Court (Aurangabad Bench)
  * `HCBM04`: Bombay High Court (Goa Bench)
  * `HCAL01`: Allahabad High Court
  * `HCAL02`: Allahabad High Court (Lucknow Bench)
  * `HCMB01`: Madras High Court
  * `HCKK01`: Karnataka High Court (Bengaluru)
  * `HCCB01`: Calcutta High Court

---

### 1.3 NCLT Bench Codes Require Trailing '0'
All National Company Law Tribunal bench codes must end with a trailing `0`:
* **Wrong**: `GET /api/partner/search?courtCodes=NCLTDL`
* **Correct**: `GET /api/partner/search?courtCodes=NCLTDL0` (NCLT New Delhi)

---

### 1.4 Supreme Court Representation
In court structures and state filters:
* Supreme Court is represented by the code **`SC`**.
* In state listings, it appears with state name **`India`** (`{ "state": "SC", "stateName": "India" }`).

---

### 1.5 Advocate Search & Bar Council Number Nuance
* **No Bar Council / Enrollment Number Filter**: The API does **not** index Bar Council registration numbers (e.g. `D/1234/2018`) as structured search fields.
* **Advocate Search**: Use `advocates=<Name>` with `nameMatchMode=fuzzy` or `phrase`.
* **Full-Text Fallback**: If you need to search an advocate's Bar enrollment number, pass it to the general `query` parameter (e.g., `?query=D%2F1234%2F2018`). This matches if the court order or FIR text explicitly records the enrollment number.

---

### 1.6 Acts and Sections: Avoid Exact Filter, Use `query`
The `actsAndSections` query parameter requires an exact stored text match (e.g., `INDIAN PENAL CODE - 302`) and suffers from sparse indexing in court records:
* **Poor**: `?actsAndSections=IPC+302` (0 hits)
* **Recommended**: Use the full-text parameter: `?query=IPC+302` (returns 432,000+ matching records across judgments and orders).

---

### 1.7 Order Document Latencies
1. **Order AI Summary (`/order-ai/`)**: Generated on-demand from government CIS servers. Latency is **10–60 seconds** on first access. Cache responses in your database!
2. **Order Markdown (`/order-md/`)**: Real-time PDF conversion pipeline can take up to **300 seconds**.
3. **Pro-Tip**: Case Detail (`/api/partner/case/{cnr}`) already contains the full text of orders in `files.files[].markdownContent`. Use Case Detail for instant text without triggering the 300s PDF converter!

---

### 1.8 Cause List Pagination vs Search Pagination
* **Case Search**: `page` (starts at 1) and `pageSize` (max 200).
* **Cause List Search**: `offset` (starts at 0) and `limit` (max 200). `next_offset = previous_offset + limit`.

---

## 2. C# Backend Integration Guidelines for CaseTracker

### 2.1 Register HttpClient with Typed Service
In `Program.cs` / `InfrastructureServiceExtensions.cs`:
```csharp
builder.Services.AddHttpClient("ECourtsClient", client =>
{
    client.BaseAddress = new Uri("https://webapi.ecourtsindia.com/");
    client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", builder.Configuration["ECourts:ApiKey"]);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
    client.Timeout = TimeSpan.FromSeconds(120);
});
```

### 2.2 Standard Generic Envelope Model
```csharp
public class ECourtsResponse<T>
{
    [JsonPropertyName("data")]
    public T Data { get; set; } = default!;

    [JsonPropertyName("meta")]
    public ECourtsMeta Meta { get; set; } = default!;
}

public class ECourtsMeta
{
    [JsonPropertyName("request_id")]
    public string RequestId { get; set; } = string.Empty;
}

public class ECourtsErrorResponse
{
    [JsonPropertyName("error")]
    public ECourtsError Error { get; set; } = default!;

    [JsonPropertyName("meta")]
    public ECourtsMeta Meta { get; set; } = default!;
}

public class ECourtsError
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("details")]
    public List<string> Details { get; set; } = new();
}
```

### 2.3 Synthesizing Case Display Title
Because Case Search results do not contain a single `title` field, format it using a C# helper:
```csharp
public static string FormatCaseTitle(IEnumerable<string> petitioners, IEnumerable<string> respondents)
{
    var pet = petitioners != null && petitioners.Any() ? string.Join(", ", petitioners) : "Unknown Petitioner";
    var resp = respondents != null && respondents.Any() ? string.Join(", ", respondents) : "Unknown Respondent";
    return $"{pet} vs {resp}";
}
```

### 2.4 Idempotent LegalCheck Invocations
When submitting a LegalCheck screening job in C#, always include an idempotency key:
```csharp
var request = new HttpRequestMessage(HttpMethod.Post, "api/partner/legal-check")
{
    Content = JsonContent.Create(checkPayload)
};
request.Headers.Add("Idempotency-Key", $"legalcheck-{subjectId}-{DateTime.UtcNow:yyyyMMdd}");

var response = await client.SendAsync(request);
```

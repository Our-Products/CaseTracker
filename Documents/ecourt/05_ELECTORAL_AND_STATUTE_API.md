# 05. Electoral Roll & IndiaCode Statute API

---

## 1. Electoral Roll API

Provides access to the Indian electoral roll database to corroborate voter identity, age, relative names, address, and polling station records.

### 1.1 Electoral Roll Search (`GET /api/partner/electoral/search`)

Searches electors with results grouped by EPIC (Voter ID). A person is returned once with an `occurrences` array covering revisions and roll types.

#### Primary Criteria Rule
Every search **MUST** include at least one primary criterion:
* `name`: Elector name (supports Indic script and Latin transliteration).
* `epic`: EPIC (Voter ID) number.
* `q`: Free-text across name and relative name.
* **Or** a complete household key: `householdRollId`, `householdPartNumber`, and `householdHouse` supplied together.

> **CRITICAL**: Supplying only filters (such as `relativeName`, `stateCode`, `age`) without a primary criterion is rejected with `400 MISSING_SEARCH_CRITERIA` (unbilled).

#### Key Query Parameters
* `name` (string): e.g., `ramesh kumar`.
* `epic` (string): e.g., `ABC1234567`.
* `epicFuzzy` (boolean): Default `false`. Allows near-matches for OCR-confused characters.
* `nameMatchMode` (string): `all` (default), `any`, `phrase`, `fuzzy`.
* `relativeName` (string): Father / spouse name.
* `stateCode` (string): e.g., `S01` (Andhra Pradesh), `U05` (Delhi).
* `districtCode` / `acNumber` (int): Assembly constituency.
* `age` & `ageTolerance` (int): Age matching window.
* `gender` (string): `M`, `F`, `O`.
* `page` & `pageSize` (int): 1 to 100. *(Ceiling is 100, lower than Case Search's 200).*

#### Sample Request
```bash
curl -X GET "https://webapi.ecourtsindia.com/api/partner/electoral/search?name=ramesh%20kumar&stateCode=S01&age=35&pageSize=20" \
  -H "Authorization: Bearer eci_live_your_token_here"
```

---

### 1.2 Electoral EPIC Lookup (`GET /api/partner/electoral/epic/{epic}`)

Exact voter ID lookup shortcut. Returns a single person object with all historical occurrences across revisions.

```bash
curl -X GET "https://webapi.ecourtsindia.com/api/partner/electoral/epic/ABC1234567" \
  -H "Authorization: Bearer eci_live_your_token_here"
```

---

### 1.3 Electoral Capabilities (`GET /api/partner/electoral/capabilities`)

Machine-readable dictionary of supported electoral facets, filters, and match modes. **Cost: 0 Credits (Free to call).**

---

## 2. IndiaCode Statute API (`indiacode.ecourtsindia.com`)

The eCourtsIndia platform provides a dedicated, keyless, read-only JSON API for Central and State Acts of India, subordinate legislation, and reported judgments that construe them.

* **Base URL**: `https://indiacode.ecourtsindia.com/api/v1`
* **Authentication**: **Keyless & Public** (No API key needed)
* **CORS**: `Access-Control-Allow-Origin: *` on all endpoints.
* **Envelope**: Includes `total`, `count`, `limit`, `offset`, and `next` URL.

### Key Statute Endpoints

| Endpoint | Description |
| :--- | :--- |
| `GET /api/v1/meta` | Index statistics and database build information. |
| `GET /api/v1/acts` | List all Central & State Acts with year, ministry, and section counts. |
| `GET /api/v1/acts/{act}` | Details of a specific Act (e.g., `bns`, `bnss`, `cpc`, `crpc`, `ipc`). |
| `GET /api/v1/{act}/section/{number}` | Full section text, legislative history, and explanations. |
| `GET /api/v1/search` | Full-text statutory search across provisions and acts. |
| `GET /api/v1/instruments` | Subordinate legislation (rules, regulations, notifications). |
| `GET /api/v1/judgments` | Reported judgments that cite or interpret a specific section. |
| `GET /api/v1/mappings` | **New Criminal Law Mappings**: Maps provisions between old acts (`ipc`, `crpc`, `iea`) and new acts (`bns`, `bnss`, `bsa`) with confidence scores. |
| `GET /api/v1/offences` | Offence schedule: cognizable, bailable, punishment, and triable court. |
| `GET /api/v1/cpc/order/{order}/rule/{rule}` | Civil Procedure Code (CPC) specific Order & Rule text. |

### Raw Markdown & XML Formatting
Section text is also available directly at the domain root:
* Markdown: `https://indiacode.ecourtsindia.com/{act}/section/{number}.md`
* Akoma Ntoso XML: `https://indiacode.ecourtsindia.com/{act}/section/{number}.xml`
*(Example: `https://indiacode.ecourtsindia.com/bns/section/103.md`)*

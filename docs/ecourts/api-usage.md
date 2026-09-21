# eCourts External API Usage Guide

## Configuration
Configure credentials in `appsettings.json` or environment variables:
```json
{
  "ECourts": {
    "BaseUrl": "https://webapi.ecourtsindia.com",
    "ApiKey": "eci_live_ab4fq583183q7k7aa5ywlge5ps7fkraf",
    "TargetStates": ["TN", "PY"],
    "Sync": {
      "BatchSize": 25,
      "MaxRequestsPerRun": 50,
      "RetryCount": 3,
      "RetryDelaySeconds": 2,
      "CaseSyncIntervalHours": 24,
      "CourtMasterSyncIntervalDays": 30
    }
  }
}
```

### Environment Variable Equivalents (Production)
```bash
ECOURTS__APIKEY=eci_live_ab4fq583183q7k7aa5ywlge5ps7fkraf
ECOURTS__BASEURL=https://webapi.ecourtsindia.com
ECOURTS__SYNC__BATCHSIZE=25
ECOURTS__SYNC__CASESYNCINTERVALHOURS=24
```

## Supported Endpoints
| Endpoint | Method | Credits | Description |
|---|---|---|---|
| `/api/partner/causelist/court-structure/states` | GET | 0 | Lists Indian States |
| `/api/partner/causelist/court-structure/districts?state={code}` | GET | 0 | Lists Districts for State |
| `/api/partner/causelist/court-structure/complexes?state={code}&district={dist}` | GET | 0 | Lists Complexes |
| `/api/partner/causelist/court-structure/courts?state={code}&district={dist}&complex={comp}` | GET | 0 | Lists Court Benches |
| `/api/partner/case/{cnr}` | GET | 1 | Fetches full case docket & hearings |

## Adding a New State Later
To expand coverage beyond Tamil Nadu and Puducherry:
1. Add the state code (e.g. `"KA"` for Karnataka, `"KL"` for Kerala) to `ECourts:TargetStates` in `appsettings.json`.
2. Trigger `POST /api/courts/sync-master/{stateCode}` to populate the state, district, complex, and court hierarchy.
3. Cases registered with CNRs from that state will automatically be included in subsequent case status syncs without code changes.

# CaseTracker — Global Error Handling & Response Standards

> **Document Version:** 1.0  
> **Standard:** RFC 7807 Problem Details for HTTP APIs  
> **Parent Documentation:** [Documentation Center](../README.md) | [API Reference](./01_api_reference.md)

---

## 1. Overview

CaseTracker implements a centralized, zero-leak error handling pipeline via [`GlobalExceptionHandlerMiddleware`](file:///d:/ProjectApp/CaseTracker/CaseTracker/Middleware/GlobalExceptionHandlerMiddleware.cs). 

All exceptions thrown anywhere in the application (controllers, application services, domain models, or repositories) are intercepted, logged, and transformed into a standardized, client-friendly JSON response.

---

## 2. Standardized JSON Envelope

### 2.1 Success Response Schema (`2xx`)
```json
{
  "success": true,
  "message": "Operation completed successfully.",
  "data": { ... }
}
```

### 2.2 Error Response Schema (`4xx`, `5xx`)
```json
{
  "success": false,
  "message": "Human-readable summary of the error.",
  "errors": [
    "Field 'mobileNumber' must be a valid 10-digit number with country code.",
    "Field 'email' is required."
  ],
  "traceId": "00-512b9d21c3858fa7b243cb8d752ad7c8-89c5651c6c602a83-00"
}
```

---

## 3. Exception to HTTP Status Mapping

| Domain Exception | HTTP Status Code | Description | Typical Scenario |
| :--- | :---: | :--- | :--- |
| `ValidationException` | `400 Bad Request` | Client request failed validation rules | Invalid phone number format, missing required fields |
| `UnauthorizedAccessException` / `UnauthorizedException` | `401 Unauthorized` | Missing, invalid, or expired credentials | Expired JWT token, wrong password |
| `ForbiddenException` | `403 Forbidden` | Authenticated user lacks required role | Regular lawyer attempting to access admin endpoints |
| `KeyNotFoundException` / `NotFoundException` | `404 Not Found` | Requested entity does not exist | Lawyer ID or User ID not found in database |
| `InvalidOperationException` / `ConflictException` | `409 Conflict` | Conflicting state in the system | Registering with a mobile number that already exists |
| All other unhandled exceptions | `500 Internal Server Error` | Unexpected server failure | Database timeout, unhandled null reference |

---

## 4. Security & Error Masking in Production

* In `Production` mode, internal stack traces, EF Core SQL queries, and server file paths are **strictly masked**.
* A unique `traceId` (W3C trace context) is returned in the response envelope, enabling frontend users to quote the ID when reporting issues to support.
* Production logs capture the full stack trace and correlation ID in application telemetry for diagnostic analysis.

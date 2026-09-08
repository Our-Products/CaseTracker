# CaseTracker — Authentication & Authorization API Flow

> **Base Route:** `/api/auth`  
> **Auth Scheme:** Bearer `<JWT_TOKEN>`  
> **Parent Documentation:** [Documentation Center](../README.md)

---

## 1. Overview

CaseTracker uses **JWT (JSON Web Token)** stateless bearer authentication. All API routes require authorization headers except `/api/auth/login` and `/api/auth/register`.

```text
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## 2. Registration Flow (`POST /api/auth/register`)

Supports registration for both **Individual Advocates** and **Law Firms / Organizations**.

### Sequence Diagram

```mermaid
sequenceDiagram
    actor Client as Mobile Client
    participant AuthCtrl as AuthController
    participant AuthSvc as AuthService
    participant PassSvc as PasswordService
    participant DB as PostgreSQL

    Client->>AuthCtrl: POST /api/auth/register
    AuthCtrl->>AuthSvc: RegisterAsync(request)
    AuthSvc->>DB: Check Mobile Number & Email uniqueness
    AuthSvc->>PassSvc: HashPassword(plainPassword)
    PassSvc-->>AuthSvc: hashedPassword
    AuthSvc->>DB: Begin Transaction & Save User, Role, Lawyer/Firm
    DB-->>AuthSvc: Committed
    AuthSvc-->>AuthCtrl: AuthResult (Token, Expiry, UserId)
    AuthCtrl-->>Client: 200 OK (AuthResult JSON)
```

### Request Payload (Individual Lawyer)
```json
{
  "registerType": 0,
  "mobileNumber": "9876543210",
  "email": "advocate@test.com",
  "password": "Password123!",
  "fullName": "Adv. R. Sundaram",
  "barCouncilId": "TN/1042/2018",
  "barCouncilName": "Bar Council of Tamil Nadu and Puducherry",
  "enrollmentDate": "2018-06-15T00:00:00Z"
}
```

### Request Payload (Organization / Law Firm)
```json
{
  "registerType": 1,
  "mobileNumber": "9876543211",
  "email": "admin@sundaramassociates.com",
  "password": "Password123!",
  "fullName": "Senior Managing Partner",
  "lawFirm": {
    "firmName": "Sundaram & Associates",
    "registrationNumber": "SR/TN/2020/001",
    "addressLine1": "No. 42, Armenian Street",
    "addressLine2": "George Town",
    "city": "Chennai",
    "district": "Chennai",
    "state": "Tamil Nadu",
    "pincode": 600001
  }
}
```

---

## 3. Login Flow (`POST /api/auth/login`)

Authenticates an existing user via mobile number and password.

### Request Payload
```json
{
  "mobileNumber": "9876543210",
  "password": "Password123!"
}
```

### Success Response (`200 OK`)
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2026-09-08T08:04:00Z",
  "userId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "lawFirmId": null
}
```

### Error Response (`401 Unauthorized`)
```json
{
  "success": false,
  "message": "Invalid credentials.",
  "errors": null,
  "data": null
}
```


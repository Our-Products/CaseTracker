# CaseTracker — REST API Reference & Specification

> **Document Version:** 1.0  
> **Base URL (Production):** `http://zylocorp.runasp.net/api`  
> **Interactive Docs:** [http://zylocorp.runasp.net/swagger](http://zylocorp.runasp.net/swagger/index.html)  
> **Authentication:** `Authorization: Bearer <JWT_TOKEN>`  
> **Parent Documentation:** [Documentation Center](../README.md)

---

## 1. System & Health Endpoints

### 1.1 Service Version & Health Check
* **Route**: `GET /api/version`
* **Authentication**: None (Anonymous)
* **Description**: Verifies service liveness, current deployed release version, and environment.

#### Response: `200 OK`
```json
{
  "service": "CaseTracker API",
  "version": "1.0.1",
  "environment": "Production",
  "deployedAt": "2026-09-16T02:27:35.123Z",
  "branch": "master (tested from dev/soorya)"
}
```

---

## 2. Authentication (`/api/auth`)

### 2.1 Register Lawyer or Law Firm
* **Route**: `POST /api/auth/register`
* **Authentication**: None (Anonymous)
* **Description**: Registers a new user account, creates the advocate profile, links the firm (if applicable), and assigns the default `Lawyer` role (`R001`).

#### Request Payload
```json
{
  "mobileNumber": "+919876543210",
  "email": "advocate.soorya@example.com",
  "password": "StrongPassword@123",
  "fullName": "Soorya Narayanan",
  "barCouncilId": "TN/1042/2018",
  "barCouncilName": "Bar Council of Tamil Nadu and Puducherry",
  "enrollmentDate": "2018-06-15",
  "lawFirmId": null
}
```

#### Response: `200 OK`
```json
{
  "success": true,
  "message": "User registered successfully.",
  "data": {
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "mobileNumber": "+919876543210",
    "email": "advocate.soorya@example.com",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresAt": "2026-09-16T04:30:00Z"
  }
}
```

#### Error Responses
* `400 Bad Request`: Validation failure (invalid mobile format, weak password).
* `409 Conflict`: Mobile number or email already registered in system.

---

### 2.2 Login
* **Route**: `POST /api/auth/login`
* **Authentication**: None (Anonymous)
* **Description**: Authenticates via mobile number and password, returns signed JWT.

#### Request Payload
```json
{
  "mobileNumber": "+919876543210",
  "password": "StrongPassword@123"
}
```

#### Response: `200 OK`
```json
{
  "success": true,
  "message": "Login successful.",
  "data": {
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "mobileNumber": "+919876543210",
    "email": "advocate.soorya@example.com",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresAt": "2026-09-16T04:30:00Z"
  }
}
```

#### Error Responses
* `401 Unauthorized`: Invalid credentials or inactive user status.

---

## 3. Users (`/api/users`)

### 3.1 Get All Users (Paginated)
* **Route**: `GET /api/users?page=1&pageSize=20`
* **Authentication**: Bearer (Role: `Admin`)
* **Response: `200 OK`**
```json
{
  "success": true,
  "data": [
    {
      "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "mobileNumber": "+919876543210",
      "email": "advocate.soorya@example.com",
      "status": "Active",
      "createdAt": "2026-09-16T01:00:00Z"
    }
  ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 1
}
```

### 3.2 Get User by ID
* **Route**: `GET /api/users/{id}`
* **Authentication**: Bearer

---

## 4. Lawyers (`/api/lawyers`)

### 4.1 Search Advocates
* **Route**: `GET /api/lawyers?search=Soorya`
* **Authentication**: Bearer

### 4.2 Get Lawyer Profile
* **Route**: `GET /api/lawyers/{id}`
* **Authentication**: Bearer
```json
{
  "success": true,
  "data": {
    "lawyerId": "b1b2b3b4-5717-4562-b3fc-2c963f66afa6",
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "fullName": "Soorya Narayanan",
    "barCouncilId": "TN/1042/2018",
    "barCouncilName": "Bar Council of Tamil Nadu and Puducherry",
    "enrollmentDate": "2018-06-15",
    "status": "Active",
    "lawFirm": null
  }
}
```

---

## 5. Law Firms (`/api/lawfirms`)

### 5.1 List Law Firms
* **Route**: `GET /api/lawfirms`
* **Authentication**: Bearer

### 5.2 Register Law Firm
* **Route**: `POST /api/lawfirms`
* **Authentication**: Bearer (Role: `Admin` or `Lawyer`)
```json
{
  "firmName": "Zylocorp Legal LLP",
  "registrationNumber": "LLP-TN-2026-9044",
  "addressLine1": "No. 42, High Court Complex",
  "city": "Chennai",
  "district": "Chennai",
  "state": "Tamil Nadu",
  "pincode": 600104
}
```

---

## 6. Roles & Role Management (`/api/roles`, `/api/userroles`)

### 6.1 List System Roles
* **Route**: `GET /api/roles`
* **Authentication**: Bearer
* **Response**: Returns available roles (`R001` Lawyer, `R002` Staff, `R003` Admin).

### 6.2 Assign Role to User
* **Route**: `POST /api/userroles`
* **Authentication**: Bearer (Role: `Admin`)
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "roleId": "R003"
}
```

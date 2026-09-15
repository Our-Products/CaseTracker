# CaseTracker — Authentication & Security Handshake

> **Document Version:** 1.1  
> **Scheme:** JSON Web Token (JWT) Bearer Authentication  
> **Algorithm:** HMAC-SHA256 (`HS256`)  
> **Hashing Algorithm:** PBKDF2 / BCrypt with Unique Salt  
> **Parent Documentation:** [Documentation Center](../README.md)

---

## 1. Authentication Architecture

CaseTracker uses **stateless token-based authentication**. Clients authenticate via credentials (Mobile Number / Email + Password) and receive a signed JWT Bearer Token, which must be presented in subsequent HTTP requests via the `Authorization: Bearer <token>` header.

```mermaid
sequenceDiagram
    autonumber
    actor User as User / Advocate
    participant Client as Client Application
    participant API as CaseTracker API (/api/auth)
    participant Hasher as PasswordService
    participant DB as PostgreSQL Database
    participant JWT as JwtService

    %% REGISTRATION FLOW
    rect rgb(240, 248, 255)
    Note over User, JWT: 1. Registration Flow
    User->>Client: Submit Full Name, Mobile, Email, Bar Council ID, Password
    Client->>API: POST /api/auth/register
    API->>DB: Check if Mobile or Email exists
    alt Mobile or Email Exists
        DB-->>API: Duplicate Found
        API-->>Client: 409 Conflict ("Account already exists")
    else New Account
        API->>Hasher: HashPassword(rawPassword)
        Hasher-->>API: Hashed Password + Salt
        API->>DB: Insert User, Lawyer, and UserRole records in single transaction
        DB-->>API: Transaction Committed
        API->>JWT: GenerateToken(User, Roles, Firm)
        JWT-->>API: Signed JWT Token
        API-->>Client: 200 OK { token, expiresAt, userId }
        Client->>Client: Store Token securely in keychain/storage
    end
    end

    %% LOGIN FLOW
    rect rgb(245, 255, 250)
    Note over User, JWT: 2. Login Flow
    User->>Client: Enter Mobile Number & Password
    Client->>API: POST /api/auth/login
    API->>DB: Query User by Mobile Number
    alt User Not Found or Inactive
        DB-->>API: Null or Status != Active
        API-->>Client: 401 Unauthorized ("Invalid credentials")
    else User Exists
        API->>Hasher: VerifyPassword(rawPassword, storedHash)
        alt Password Mismatch
            Hasher-->>API: Verification Failed
            API-->>Client: 401 Unauthorized ("Invalid credentials")
        else Password Valid
            Hasher-->>API: Verification Passed
            API->>DB: Fetch User Roles & Associated LawFirm
            DB-->>API: Roles: ['Lawyer']
            API->>JWT: GenerateToken(User, Roles, Firm)
            JWT-->>API: Signed JWT Token
            API-->>Client: 200 OK { token, expiresAt, userId }
        end
    end
    end

    %% AUTHENTICATED REQUEST FLOW
    rect rgb(255, 250, 240)
    Note over User, JWT: 3. Authenticated Request Flow
    Client->>API: GET /api/lawyers/{id}<br/>Authorization: Bearer <token>
    API->>API: Validate Token Signature & Expiry
    alt Token Invalid or Expired
        API-->>Client: 401 Unauthorized
    else Token Valid
        API->>API: Extract Claims (UserId, Roles, FirmId)
        API->>API: Evaluate Authorization Policy
        API-->>Client: 200 OK (Requested Resource)
    end
    end
```

---

## 2. JWT Token Structure & Claims

Every issued token contains the following cryptographically signed payload:

### Token Header
```json
{
  "alg": "HS256",
  "typ": "JWT"
}
```

### Token Payload
```json
{
  "sub": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "nameid": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "mobile_phone": "+919876543210",
  "email": "advocate.soorya@example.com",
  "role": "Lawyer",
  "law_firm_id": "b1b2b3b4-5717-4562-b3fc-2c963f66afa6",
  "jti": "d3b07384-d113-40e1-9d8e-0f0e0c0d0e0f",
  "iss": "CaseTracker",
  "aud": "CaseTrackerAudience",
  "exp": 1789531200,
  "iat": 1789527600
}
```

---

## 3. Password Security Standard

* Passwords must be at least **8 characters long** and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.
* Plaintext passwords are **never stored or logged**.
* Hashing utilizes PBKDF2 with HMAC-SHA256 (or BCrypt) using a cryptographically secure randomly generated salt.

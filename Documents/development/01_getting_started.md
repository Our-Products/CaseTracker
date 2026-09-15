# CaseTracker — Developer Onboarding & Local Setup

> **Document Version:** 1.1  
> **Target Audience:** Software Engineers, QA Engineers, DevOps  
> **Parent Documentation:** [Documentation Center](../README.md)

---

## 1. Prerequisites & Tooling

To develop, build, and debug CaseTracker, install the following:

* **[.NET 10 SDK](https://dotnet.microsoft.com/)**
* **[Visual Studio 2022 / 2025](https://visualstudio.microsoft.com/)** (v17.10+ / Preview) or **[VS Code](https://code.visualstudio.com/)**:
  * Workload: *ASP.NET and web development*
* **[PostgreSQL 16+](https://www.postgresql.org/)** (or cloud [Neon.tech](https://neon.tech/))
* **[Git](https://git-scm.com/)**
* **EF Core CLI Tool**:
  ```powershell
  dotnet tool install --global dotnet-ef
  ```

---

## 2. Step-by-Step Environment Setup

### 2.1 Clone the Repository
```powershell
git clone https://github.com/Our-Products/CaseTracker.git
cd CaseTracker
```

### 2.2 Configure PostgreSQL Database
You can use either a local PostgreSQL instance or the cloud Neon PostgreSQL database.

Open `CaseTracker/appsettings.Development.json` and verify your connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=ep-dry-silence-aepi0j4w-pooler.c-2.us-east-2.aws.neon.tech; Database=CaseTracker; Username=neondb_owner; Password=npg_KEhBgumG9Dv6; SSL Mode=VerifyFull; Channel Binding=Require;"
  },
  "Jwt": {
    "Key": "YOUR_SUPER_SECRET_JWT_KEY_MIN_32_CHARS_FOR_HS256!!!",
    "Issuer": "CaseTracker",
    "Audience": "CaseTrackerAudience",
    "ExpiryMinutes": 60
  }
}
```

### 2.3 Run Database Migrations
Apply the initial schema migrations to create all database tables and seed default roles:
```powershell
dotnet ef database update --project CaseTrackerInfrastructure --startup-project CaseTracker
```

---

## 3. Running the Backend Web API

Execute from the solution root:
```powershell
dotnet run --project CaseTracker/CaseTracker.csproj
```

The Web API will launch locally with endpoints:
* **HTTPS**: `https://localhost:7198`
* **HTTP**: `http://localhost:5222`
* **Swagger UI**: `https://localhost:7198/swagger/index.html`
* **Version API**: `https://localhost:7198/api/version`

---

## 4. Running Automated Tests & Builds

```powershell
# Restore dependencies
dotnet restore CaseTracker.slnx

# Compile in Release mode
dotnet build CaseTracker.slnx --configuration Release
```

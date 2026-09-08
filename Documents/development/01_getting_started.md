# CaseTracker — Developer Onboarding & Local Setup

> **Document Version:** 1.0  
> **Target Audience:** Software Engineers, QA Engineers, DevOps  
> **Parent Documentation:** [Documentation Center](../README.md)

---

## 1. Prerequisites & Tooling

To develop, build, and debug CaseTracker, install the following:

* **[.NET 10 SDK](https://dotnet.microsoft.com/)**
* **[Visual Studio 2022](https://visualstudio.microsoft.com/) (v17.10+ / Preview)** or **VS Code**:
  * Workload: *ASP.NET and web development*
  * Workload: *.NET Multi-platform App UI development (.NET MAUI)*
* **[PostgreSQL 16+](https://www.postgresql.org/)** (or Docker container running PostgreSQL)
* **[Git](https://git-scm.com/)**
* **EF Core CLI Tool**:
  ```powershell
  dotnet tool install --global dotnet-ef
  ```

---

## 2. Step-by-Step Environment Setup

### 2.1 Clone the Repository
```powershell
git clone <repository-url>
cd D:\CaseTracker\Backend
```

### 2.2 Configure PostgreSQL Database
Ensure your local PostgreSQL service is running on port 5432.

Open [`CaseTracker/appsettings.Development.json`](file:///D:/CaseTracker/Backend/CaseTracker/appsettings.Development.json) and verify your connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=CaseTracker;Username=postgres;Password=your_postgres_password;"
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

Execute the Web API via CLI or Visual Studio:
```powershell
dotnet run --project CaseTracker
```
* **Swagger UI**: Navigate to `https://localhost:7232/swagger`
* **API Base Address**: `https://localhost:7232/api/`

---

## 4. Running the .NET MAUI Mobile App

1. Open `CaseTracker.slnx` in Visual Studio.
2. Set **`CaseTrackerMobile`** as the Startup Project.
3. Select your target debugging platform:
   * **Android Emulator / Physical Device**: (Uses `10.0.2.2:7232` for emulator-to-host communication).
   * **Windows Machine**: Local native desktop execution.
   * **iOS Simulator / MacCatalyst**: Requires pairing with a networked Mac.
4. Press **F5** to build and deploy.

---

## 5. Seed Test Advocate Account

For immediate testing of login, dashboard, and cause list views, use the pre-configured credentials:

| Field | Value |
| :--- | :--- |
| **Mobile Number** | `9876543210` |
| **Password** | `Password123!` |
| **Advocate Name** | Adv. R. Sundaram |
| **Bar Council ID** | `TN/1042/2018` |
| **Law Firm** | Sundaram & Associates |
| **Active Cases** | 42 |
| **Today's Hearings** | 5 |


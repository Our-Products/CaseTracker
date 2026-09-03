Add-Migration {migrationName}
-Context Infrastructure.Data.ApplicationDbContext
-Project Infrastructure -StartupProject CaseTracker

Update-Database
-Project Infrastructure
-StartupProject CaseTracker 
-Context Infrastructure.Data.ApplicationDbContext
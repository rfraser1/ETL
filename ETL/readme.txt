Scaffolding the database - PowerShell
dotnet ef dbcontext scaffold "Data Source=RICHIE-PC\SQLEXPRESS;Initial Catalog=Rainfall;TrustServerCertificate=true;User ID=dev;Password=dev" Microsoft.EntityFrameworkCore.SqlServer --use-database-names --context-dir Data --output-dir DBModels --force

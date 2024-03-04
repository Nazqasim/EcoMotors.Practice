# Migrations
This command is to be executed from the Host Directory of the project.
> dotnet ef migrations add <CommitMessage> --project .././Migrators/Migrators.<Provider>/ --context ApplicationDbContext -o Migrations/Application
Migration Command
 C:\Users\Muhammad Mutahhar\source\repos\bbd-be\src\host> dotnet ef migrations add <CommitMessage> --project .././Migrators/Migrators.MSSQL/ --context ApplicationDbContext -o Migrations/Application
Revert Migration Command
  dotnet ef migrations remove --project .././Migrators/Migrators.MSSQL/ --context ApplicationDbContext

Update Migration in Database
dotnet ef database update <MigrationName>
dotnet ef migrations add InitialMigration `
--startup-project Tulahack.API `
--context TulahackContext `
--output-dir Context\Migrations --verbose

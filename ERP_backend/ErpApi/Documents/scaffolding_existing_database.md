# Scaffolding From an Existing Database

When the database schema is the source of truth, use Entity Framework Core's reverse-engineering tooling to scaffold a DbContext and entity classes directly from the live database.

## 1. Prerequisites

- Install the design-time packages (already referenced in `ErpApi.csproj`):
  - `Microsoft.EntityFrameworkCore.Design`
  - The provider package for your database (e.g., `Npgsql.EntityFrameworkCore.PostgreSQL` or `Microsoft.EntityFrameworkCore.SqlServer`).
- Ensure `dotnet-ef` is available (`dotnet tool install --global dotnet-ef` if needed).
- Have a valid connection string for the target database. The connection string can be stored in `appsettings.json`, secrets, or supplied inline.

## 2. Choose Output Locations

Keep scaffolded code away from custom business logic. A common layout is:

```
Data/Generated/AppDb.cs        // Scaffolded DbContext (partial)
Data/Generated/Entities/*.cs  // Generated entity partial types
Data/Models/*.cs              // Hand-written partial class extensions
```

The `Generated` folder holds the files that can be safely recreated. Your custom logic lives in a parallel folder (`Data/Models`) in partial class files.

## 3. Run the Scaffold Command

Use the .NET CLI and specify namespaces, output folders, and the `--data-annotations` flag if you want attributes in addition to fluent configuration.

```bash
DB_CONNECTION_STRING='<connection-string>' dotnet ef dbcontext scaffold \
  "$DB_CONNECTION_STRING" \
  Npgsql.EntityFrameworkCore.PostgreSQL \
  --context AppDb \
  --context-dir Data/Generated \
  --output-dir Data/Generated/Entities \
  --namespace ErpApi.Data.Generated.Entities \
  --context-namespace ErpApi.Data.Generated \
  --data-annotations
```

Notes:

- Replace the provider name if you use a different database.
- The `--context` option controls the generated context class name.
- The `--namespace` switches keep generated code out of your main namespaces, minimizing conflicts.

## 4. Extend with Partial Classes

After scaffolding, create matching partial classes for business logic:

```csharp
// Data/Models/Product.Partial.cs
namespace ErpApi.Data.Models;

public partial class Product
{
    public decimal NetPrice => Price * 0.9m;
}
```

```csharp
// Data/Models/AppDb.Partial.cs
using Microsoft.EntityFrameworkCore;

namespace ErpApi.Data.Models;

public partial class AppDb
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        // Custom configuration lives here and survives re-scaffolds.
    }
}
```

Generated files typically include partial method hooks (e.g., `OnModelCreatingPartial`). Use them to augment EF configuration without editing generated code.

## 5. Re-scaffold When the Schema Changes

- Whenever the DBA updates the schema, rerun the scaffold command.
- Delete the old contents of `Data/Generated` first or let `dotnet ef` overwrite them.
- Because your business logic resides in separate partial files, it remains intact.

## 6. Regenerate Snapshots & Migrations (Optional)

If you plan to continue using migrations on top of a scaffolded model:

- Update the generated context and entities.
- Recreate migrations (`dotnet ef migrations add <Name>`), or rely on raw scaffolding without migrations if the database remains authoritative.

## 7. Verify the Generated Model

- Build the project (`dotnet build`) to ensure the new classes compile.
- Optionally run a smoke test (e.g., `dotnet run` or integration tests) to validate the context can query the existing tables.

Following this structure keeps generated EF Core code isolated from your domain/business logic, allowing you to re-scaffold safely whenever the database changes.

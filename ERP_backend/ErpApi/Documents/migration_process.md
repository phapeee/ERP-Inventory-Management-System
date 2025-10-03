# Migration Process

This project uses Entity Framework Core migrations to translate model changes into database schema updates. Our pipeline includes a custom migrations generator that inserts analyzer suppressions and guard clauses automatically. Follow the steps below whenever you need to introduce or apply migrations.

## 1. Prepare the environment

- Ensure the API can start in design-time: set the `DB_CONNECTION_STRING` environment variable to a reachable database. For local development you can use a lightweight Postgres container.
- If you only want to scaffold code without touching a database, provide a dummy connection string and skip the update step later.

## 2. Remove obsolete migrations (if necessary)

- Delete prior migration files or run `dotnet ef migrations remove` when you want to replace the existing initial schema.
- Our current initial migration lives at `Migrations/20251003002624_InitialProducts.cs` and creates the `Products` table.

## 3. Scaffold a new migration

```bash
DB_CONNECTION_STRING='<connection-string>' dotnet ef migrations add <MigrationName>
```

- The custom generator (`Design/SuppressingCSharpMigrationsGenerator.cs`) automatically:
  - Adds `[SuppressMessage]` attributes to silence CA1812/CA1515.
  - Seals the generated migration class.
  - Injects `ArgumentNullException.ThrowIfNull(migrationBuilder);` guards into `Up` and `Down` methods.
- Inspect the output under `Migrations/` to confirm the scaffolded code matches expectations.

## 4. Apply the migration to the database

```bash
DB_CONNECTION_STRING='<connection-string>' dotnet ef database update
```

- EF Core will execute pending migrations in order. The current initial migration creates the `Products` table with `Id`, `Name`, and `Price` columns.

## 5. Verify the database state

- Open your database client (e.g., `psql`, TablePlus) and check that the `Products` table exists with the expected schema.
- Optionally seed data using the API or manual inserts to ensure CRUD endpoints behave as expected.

## 6. Keep migrations organized

- Commit both the migration `.cs` file and the updated `AppDbModelSnapshot.cs` so teammates get a consistent schema.
- If you regenerate migrations frequently during development, remember to remove test migrations with `dotnet ef migrations remove` to avoid clutter.

By following this workflow you ensure model changes propagate predictably from code to the database, and the custom generator keeps analyzer warnings at bay automatically.

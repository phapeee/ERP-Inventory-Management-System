using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace ErpApi.Design;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Registered via EF Core design-time service provider.")]
internal sealed class SuppressingCSharpMigrationsGenerator(
    MigrationsCodeGeneratorDependencies dependencies,
    CSharpMigrationsGeneratorDependencies csharpDependencies)
    : CSharpMigrationsGenerator(dependencies, csharpDependencies)
{
    public override string GenerateMigration(
        string? migrationNamespace,
        string migrationName,
        IReadOnlyList<MigrationOperation> upOperations,
        IReadOnlyList<MigrationOperation> downOperations)
    {
        var code = base.GenerateMigration(migrationNamespace, migrationName, upOperations, downOperations);

        if (string.IsNullOrEmpty(code))
        {
            return code;
        }

        code = EnsureUsing(code, "System");
        code = EnsureUsing(code, "System.Diagnostics.CodeAnalysis");
        code = AddClassDecorations(code, migrationName);
        code = AddArgumentNullGuard(code, "Up");
        code = AddArgumentNullGuard(code, "Down");

        return code;
    }

    private static string EnsureUsing(string code, string namespaceName)
    {
        var directive = $"using {namespaceName};";
        if (code.Contains(directive, StringComparison.Ordinal))
        {
            return code;
        }

        const string anchor = "using Microsoft.EntityFrameworkCore.Migrations;";
        if (code.Contains(anchor, StringComparison.Ordinal))
        {
            return code.Replace(anchor, $"{directive}{Environment.NewLine}{anchor}", StringComparison.Ordinal);
        }

        return $"{directive}{Environment.NewLine}{code}";
    }

    private static string AddClassDecorations(string code, string migrationName)
    {
        var classHeader = $"public partial class {migrationName} : Migration";
        if (!code.Contains(classHeader, StringComparison.Ordinal))
        {
            return code;
        }

        var decoratedHeader =
            "[SuppressMessage(\"Performance\", \"CA1812:Avoid uninstantiated internal classes\", Justification = \"Instantiated by EF Core via reflection.\")]" + Environment.NewLine +
            "[SuppressMessage(\"Design\", \"CA1515:Consider marking type as internal\", Justification = \"EF Core requires migrations to remain public.\")]" + Environment.NewLine +
            classHeader.Replace("public partial", "public sealed partial", StringComparison.Ordinal);

        return code.Replace(classHeader, decoratedHeader, StringComparison.Ordinal);
    }

    private static string AddArgumentNullGuard(string code, string methodName)
    {
        var signature = $"protected override void {methodName}(MigrationBuilder migrationBuilder)";
        var header = signature + Environment.NewLine + "    {";
        if (!code.Contains(header, StringComparison.Ordinal))
        {
            return code;
        }

        var guardLine = "        ArgumentNullException.ThrowIfNull(migrationBuilder);";
        if (code.Contains(header + Environment.NewLine + guardLine, StringComparison.Ordinal))
        {
            return code;
        }

        return code.Replace(header, header + Environment.NewLine + guardLine + Environment.NewLine, StringComparison.Ordinal);
    }
}

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding;

namespace ErpApi.Design;

#pragma warning disable EF1001
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by EF Core design-time services via reflection.")]
internal sealed class PostProcessingCSharpModelGenerator : IModelCodeGenerator, ILanguageBasedService
{
    private static readonly Regex DbSetPropertyRegex = new(
        @"(?<property>\b(?:public|internal|protected|private)\s+(?:virtual\s+)?DbSet<[^>]+>\s+\w+\s*{\s*get;\s*set;\s*})",
        RegexOptions.Compiled);

    private readonly IModelCodeGenerator _inner;
    private readonly ILanguageBasedService? _languageService;

    public PostProcessingCSharpModelGenerator(
        ModelCodeGeneratorDependencies dependencies,
        IOperationReporter reporter,
        IServiceProvider serviceProvider)
    {
        var csharpGeneratorType = typeof(IModelCodeGenerator).Assembly
            .GetType("Microsoft.EntityFrameworkCore.Scaffolding.Internal.CSharpModelGenerator")
            ?? throw new InvalidOperationException("Unable to locate EF Core CSharpModelGenerator type.");

        _inner = (IModelCodeGenerator?)Activator.CreateInstance(
            csharpGeneratorType,
            dependencies,
            reporter,
            serviceProvider) ?? throw new InvalidOperationException("Failed to instantiate EF Core CSharpModelGenerator.");

        _languageService = _inner as ILanguageBasedService;
    }

    public string Language => _languageService?.Language ?? "C#";

    public ScaffoldedModel GenerateModel(IModel model, ModelCodeGenerationOptions options)
    {
        var scaffoldedModel = _inner.GenerateModel(model, options);

        if (scaffoldedModel.ContextFile?.Code is { Length: > 0 } contextCode)
        {
            contextCode = RemoveOnConfiguring(contextCode);
            contextCode = EnsureDbSetInitialization(contextCode);
            contextCode = ApplyContextSuppressions(contextCode);
            contextCode = NormalizeWhitespace(contextCode);
            scaffoldedModel.ContextFile.Code = contextCode;
        }

        if (scaffoldedModel.AdditionalFiles.Count > 0)
        {
            foreach (var additionalFile in scaffoldedModel.AdditionalFiles)
            {
                if (additionalFile.Code is { Length: > 0 } entityCode)
                {
                    entityCode = EnsureInternalEntityType(entityCode);
                    entityCode = ApplyEntityAnnotations(entityCode);
                    entityCode = NormalizeEntityProperties(entityCode);
                    entityCode = NormalizeWhitespace(entityCode);
                    additionalFile.Code = entityCode;
                }
            }
        }

        return scaffoldedModel;
    }

    private static string ApplyContextSuppressions(string code)
    {
        const string attribute = "[System.Diagnostics.CodeAnalysis.SuppressMessage(\"Design\", \"CA1515:Consider marking type as internal\", Justification = \"Generated context must remain public for DI.\")]";
        const string marker = "public partial class AppDb : DbContext";
        if (!code.Contains(attribute, StringComparison.Ordinal))
        {
            code = code.Replace(marker, attribute + "\n" + marker, StringComparison.Ordinal);
        }

        return code;
    }

    private static string ApplyEntityAnnotations(string code)
    {
        const string suppression = "[System.Diagnostics.CodeAnalysis.SuppressMessage(\"Performance\", \"CA1852:Type can be sealed\", Justification = \"Partial generated type to allow extensions.\")]";

        if (!code.Contains(suppression, StringComparison.Ordinal))
        {
            code = code.Replace("internal sealed partial class", suppression + "\ninternal sealed partial class", StringComparison.Ordinal);
            code = code.Replace("internal sealed partial record", suppression + "\ninternal sealed partial record", StringComparison.Ordinal);
        }

        return code;
    }

    private static readonly Regex TrailingWhitespaceRegex = new(@"[ \t]+(?=\r?\n)", RegexOptions.Compiled);
    private static readonly Regex MultipleBlankLinesRegex = new(@"(\r?\n){3,}", RegexOptions.Compiled);

    private static string NormalizeWhitespace(string code)
    {
        var withoutTrailing = TrailingWhitespaceRegex.Replace(code, string.Empty);
        return MultipleBlankLinesRegex.Replace(withoutTrailing, "\n\n");
    }

    private static readonly Regex DbSetAccessModifierRegex = new(@"\b(public|protected|private)\s+(?=(?:virtual\s+)?DbSet<)", RegexOptions.Compiled);
    private static readonly Regex EntityTypeDeclarationRegex = new(@"\bpublic\s+(?<partial>partial\s+)?(?<kind>class|record)\s+", RegexOptions.Compiled);
    private static readonly Regex EntityPropertyRegex = new(@"(?<indent>\s*)public\s+(?!virtual)(?!(?:partial\s+)?class\b|record\b)(?<rest>[^\n{]+\{[^\n]*get;[^\n]*\})", RegexOptions.Compiled);

    private static string EnsureDbSetInitialization(string code)
    {
        return DbSetPropertyRegex.Replace(code, match =>
        {
            var property = match.Groups["property"].Value;
            property = DbSetAccessModifierRegex.Replace(property, "internal ");
            if (!property.Contains("= null!;", StringComparison.Ordinal))
            {
                property += " = null!;";
            }

            return property;
        });
    }

    private static string EnsureInternalEntityType(string code)
    {
        if (!code.Contains("public ", StringComparison.Ordinal))
        {
            return code;
        }

        return EntityTypeDeclarationRegex.Replace(code, match =>
        {
            var partial = match.Groups["partial"].Success ? "partial " : string.Empty;
            var kind = match.Groups["kind"].Value;
            return $"internal sealed {partial}{kind} ";
        });
    }

    private static string NormalizeEntityProperties(string code)
    {
        code = code.Replace("public virtual ", "public ", StringComparison.Ordinal);
        return EntityPropertyRegex.Replace(code, match =>
        {
            var indent = match.Groups["indent"].Value;
            var rest = match.Groups["rest"].Value;
            return $"{indent}public {rest}";
        });
    }

    private static string RemoveOnConfiguring(string code)
    {
        const string signature = "protected override void OnConfiguring";
        var index = code.IndexOf(signature, StringComparison.Ordinal);
        if (index < 0)
        {
            return code;
        }

        var start = code.IndexOf('{', index);
        if (start < 0)
        {
            return code;
        }

        var end = FindMatchingBrace(code, start);
        if (end < 0)
        {
            return code;
        }

        var builder = new StringBuilder(code.Length - (end - index + 1));
        builder.Append(code, 0, index);
        builder.Append(code, end + 1, code.Length - end - 1);
        return builder.ToString();
    }

    private static int FindMatchingBrace(string text, int start)
    {
        var depth = 0;
        for (var i = start; i < text.Length; i++)
        {
            var ch = text[i];
            if (ch == '{')
            {
                depth++;
            }
            else if (ch == '}')
            {
                depth--;
                if (depth == 0)
                {
                    // consume trailing newline
                    if (i + 1 < text.Length && text[i + 1] == '\r')
                    {
                        i++;
                    }
                    if (i + 1 < text.Length && text[i + 1] == '\n')
                    {
                        i++;
                    }

                    return i;
                }
            }
        }

        return -1;
    }
}

#pragma warning restore EF1001

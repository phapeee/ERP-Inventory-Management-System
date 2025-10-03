using System;
using System.Collections.Generic;
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
internal sealed partial class PostProcessingCSharpModelGenerator : IModelCodeGenerator
{
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

        UpdateContextFile(scaffoldedModel.ContextFile);
        UpdateEntityFiles(scaffoldedModel.AdditionalFiles);

        return scaffoldedModel;
    }

    private static void UpdateContextFile(ScaffoldedFile? contextFile)
    {
        if (contextFile?.Code is not { Length: > 0 } contextCode)
        {
            return;
        }

        contextCode = RemoveOnConfiguring(contextCode);
        contextCode = EnsureDbSetInitialization(contextCode);
        contextCode = ApplyContextSuppressions(contextCode);
        contextCode = NormalizeWhitespace(contextCode);
        contextFile.Code = contextCode;
    }

    private static void UpdateEntityFiles(IList<ScaffoldedFile> additionalFiles)
    {
        if (additionalFiles.Count == 0)
        {
            return;
        }

        foreach (var additionalFile in additionalFiles)
        {
            if (additionalFile.Code is not { Length: > 0 } entityCode)
            {
                continue;
            }

            entityCode = EnsureInternalEntityType(entityCode);
            entityCode = ApplyEntityAnnotations(entityCode);
            entityCode = NormalizeEntityProperties(entityCode);
            entityCode = NormalizeWhitespace(entityCode);
            additionalFile.Code = entityCode;
        }
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

    private static string NormalizeWhitespace(string code)
    {
        var withoutTrailing = TrailingWhitespaceRegex().Replace(code, string.Empty);
        return MultipleBlankLinesRegex().Replace(withoutTrailing, "\n\n");
    }

    private static string EnsureDbSetInitialization(string code)
    {
        return DbSetPropertyRegex().Replace(code, match =>
        {
            var property = match.Groups["property"].Value;
            property = DbSetAccessModifierRegex().Replace(property, "internal ");
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

        return EntityTypeDeclarationRegex().Replace(code, match =>
        {
            var partial = match.Groups["partial"].Success ? "partial " : string.Empty;
            var kind = match.Groups["kind"].Value;
            return $"internal sealed {partial}{kind} ";
        });
    }

    private static string NormalizeEntityProperties(string code)
    {
        code = code.Replace("public virtual ", "public ", StringComparison.Ordinal);
        return EntityPropertyRegex().Replace(code, match =>
        {
            var indent = match.Groups["indent"].Value;
            var rest = match.Groups["rest"].Value;
            return $"{indent}public {rest}";
        });
    }

    private static Regex DbSetPropertyRegex() => DbSetPropertyRegexImpl();
    private static Regex TrailingWhitespaceRegex() => TrailingWhitespaceRegexImpl();
    private static Regex MultipleBlankLinesRegex() => MultipleBlankLinesRegexImpl();
    private static Regex DbSetAccessModifierRegex() => DbSetAccessModifierRegexImpl();
    private static Regex EntityTypeDeclarationRegex() => EntityTypeDeclarationRegexImpl();
    private static Regex EntityPropertyRegex() => EntityPropertyRegexImpl();

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
            switch (text[i])
            {
                case '{':
                    depth++;
                    break;
                case '}':
                    depth--;
                    if (depth == 0)
                    {
                        return ConsumeTrailingNewLine(text, i);
                    }

                    break;
            }
        }

        return -1;
    }

    private static int ConsumeTrailingNewLine(string text, int index)
    {
        var next = index + 1;
        if (next < text.Length && text[next] == '\r')
        {
            index = next;
            next++;
        }

        if (next < text.Length && text[next] == '\n')
        {
            index = next;
        }

        return index;
    }

    [GeneratedRegex(@"(?<property>\b(?:public|internal|protected|private)\s+(?:virtual\s+)?DbSet<[^>]+>\s+\w+\s*{\s*get;\s*set;\s*})", RegexOptions.Compiled)]
    private static partial Regex DbSetPropertyRegexImpl();

    [GeneratedRegex(@"[ \t]+(?=\r?\n)", RegexOptions.Compiled)]
    private static partial Regex TrailingWhitespaceRegexImpl();

    [GeneratedRegex(@"(\r?\n){3,}", RegexOptions.Compiled)]
    private static partial Regex MultipleBlankLinesRegexImpl();

    [GeneratedRegex(@"\b(public|protected|private)\s+(?=(?:virtual\s+)?DbSet<)", RegexOptions.Compiled)]
    private static partial Regex DbSetAccessModifierRegexImpl();

    [GeneratedRegex(@"\bpublic\s+(?<partial>partial\s+)?(?<kind>class|record)\s+", RegexOptions.Compiled)]
    private static partial Regex EntityTypeDeclarationRegexImpl();

    [GeneratedRegex(@"(?<indent>\s*)public\s+(?!virtual)(?!(?:partial\s+)?class\b|record\b)(?<rest>[^\n{]+\{[^\n]*get;[^\n]*\})", RegexOptions.Compiled)]
    private static partial Regex EntityPropertyRegexImpl();
}

#pragma warning restore EF1001

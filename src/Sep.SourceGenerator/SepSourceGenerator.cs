using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace nietras.SeparatedValues.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class SepSourceGenerator : IIncrementalGenerator
{
    const string AttributeMetadataName = "nietras.SeparatedValues.SepSourceGenerationAttribute";
    const string ColumnAttributeMetadataName = "nietras.SeparatedValues.SepColAttribute";
    // Enough for any realistic enum name or flags combination, so the pooled fallback is never hit.
    const int EnumStackallocCharCount = 256;

    // Fully qualified names used by the emitted source, so each name exists in exactly one place.
    const string SepNamespace = "global::nietras.SeparatedValues.";
    const string SepReaderName = SepNamespace + "SepReader";
    const string SepReaderRowName = SepReaderName + ".Row";
    const string SepReaderExtensionsName = SepNamespace + "SepReaderExtensions";
    const string SepWriterName = SepNamespace + "SepWriter";
    const string SepWriterRowName = SepWriterName + ".Row";
    const string SepWriterColName = SepWriterName + ".Col";
    const string EnumName = "global::System.Enum";
    const string SpanName = "global::System.Span";
    const string ReadOnlySpanName = "global::System.ReadOnlySpan";
    const string MemoryExtensionsName = "global::System.MemoryExtensions";
    const string ArgumentNullExceptionName = "global::System.ArgumentNullException";
    const string NotSupportedExceptionName = "global::System.NotSupportedException";
    const string ArrayPoolName = "global::System.Buffers.ArrayPool<char>";
    const string EnumerableName = "global::System.Collections.Generic.IEnumerable";
    const string EnumeratorName = "global::System.Collections.Generic.IEnumerator";
    const string AsyncEnumerableName = "global::System.Collections.Generic.IAsyncEnumerable";
    const string CancellationTokenName = "global::System.Threading.CancellationToken";
    const string ValueTaskName = "global::System.Threading.Tasks.ValueTask";
    const string TaskAsyncEnumerableExtensionsName = "global::System.Threading.Tasks.TaskAsyncEnumerableExtensions";
    const string NonGenericEnumerableName = "global::System.Collections.IEnumerable";
    const string NonGenericEnumeratorName = "global::System.Collections.IEnumerator";

    /// <summary>
    /// Identifies a diagnostic. The value is the numeric part of the diagnostic id and the index
    /// into <see cref="s_descriptors"/>, so ids exist in exactly one place.
    /// </summary>
    internal enum IssueId
    {
        InvalidAdapter = 1,
        InvalidModel,
        UnsupportedMember,
        NoMembers,
        InvalidColumn,
        DuplicateColumn,
        IndexLayout,
        NoUsableConstructor,
        AmbiguousConstructor,
        UnbindableConstructor,
        GenericModel,
        InaccessibleSetter,
        RequiresCSharp14,
    }

    static readonly ImmutableArray<DiagnosticDescriptor> s_descriptors = ImmutableArray.Create(
        CreateDescriptor(IssueId.InvalidAdapter, "Invalid Sep source-generation adapter",
            "Adapter '{0}' must be a non-generic, non-file-local, top-level static partial class"),
        CreateDescriptor(IssueId.InvalidModel, "Invalid Sep source-generation model",
            "Model '{0}' must be an accessible non-abstract class or struct"),
        CreateDescriptor(IssueId.UnsupportedMember, "Unsupported Sep source-generation member",
            "Member '{0}' {1}"),
        CreateDescriptor(IssueId.NoMembers, "No Sep source-generation members",
            "Model '{0}' has no public instance fields or properties"),
        CreateDescriptor(IssueId.InvalidColumn, "Invalid Sep column mapping",
            "Member '{0}' has an invalid Sep column mapping: {1}"),
        CreateDescriptor(IssueId.DuplicateColumn, "Duplicate Sep column mapping",
            "Column {0} is mapped by both '{1}' and '{2}'"),
        CreateDescriptor(IssueId.IndexLayout, "Invalid Sep physical column layout",
            "Model '{0}' has an invalid indexed layout: {1}"),
        CreateDescriptor(IssueId.NoUsableConstructor, "No usable Sep source-generation constructor",
            "Model '{0}' has no accessible construction plan"),
        CreateDescriptor(IssueId.AmbiguousConstructor, "Ambiguous Sep source-generation constructor",
            "Model '{0}' has multiple equally suitable accessible constructors"),
        CreateDescriptor(IssueId.UnbindableConstructor, "Unbindable Sep source-generation constructor",
            "Model '{0}' cannot bind '{1}' to an accessible construction plan"),
        CreateDescriptor(IssueId.GenericModel, "Unsupported generic Sep source-generation model",
            "Model '{0}' is generic; generic models are not supported"),
        CreateDescriptor(IssueId.InaccessibleSetter, "Inaccessible Sep source-generation setter",
            "Member '{0}' does not have an accessible setter and is not bound to a constructor parameter"),
        CreateDescriptor(IssueId.RequiresCSharp14, "Sep source generation requires C# 14",
            "Adapter '{0}' requires C# 14 or later because generated APIs use static extension members"));

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static context =>
            context.AddSource("SepSourceGenerationAttribute.g.cs", SourceText.From(AttributeSource, Encoding.UTF8)));

        var models = context.SyntaxProvider.ForAttributeWithMetadataName(
            AttributeMetadataName,
            static (node, _) => node is ClassDeclarationSyntax,
            static (context, cancellationToken) => CreateModel(context, cancellationToken))
            .Where(static model => model is not null);

        context.RegisterSourceOutput(models, static (context, model) => Emit(context, model!));
    }

    static DiagnosticDescriptor CreateDescriptor(IssueId id, string title, string message) =>
        new(DiagnosticId(id), title, message, "Usage", DiagnosticSeverity.Error, isEnabledByDefault: true);

    internal static string DiagnosticId(IssueId id) =>
        "SEPGEN" + ((int)id).ToString("D3", CultureInfo.InvariantCulture);

    static Model? CreateModel(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var adapter = (INamedTypeSymbol)context.TargetSymbol;
        if (!IsValidAdapter(adapter, cancellationToken))
        {
            return Model.Invalid(Issue.Create(IssueId.InvalidAdapter, adapter.Locations.FirstOrDefault(), adapter.Name));
        }
        var parseOptions = (CSharpParseOptions)context.SemanticModel.SyntaxTree.Options;
        if (!SupportsStaticExtensionMembers(parseOptions))
        {
            return Model.Invalid(Issue.Create(
                IssueId.RequiresCSharp14,
                adapter.Locations.FirstOrDefault(),
                adapter.Name));
        }

        var model = context.Attributes[0].ConstructorArguments[0].Value as INamedTypeSymbol;
        if (model is null)
        {
            return Model.Invalid(Issue.Create(IssueId.InvalidModel, adapter.Locations.FirstOrDefault(), "<unknown>"));
        }
        if (IsGenericModel(model))
        {
            return Model.Invalid(Issue.Create(IssueId.GenericModel, model.Locations.FirstOrDefault(), model.Name));
        }
        // Accessibility must be evaluated from the adapter, since the generated code lives there
        // and the model may come from another assembly.
        var compilation = context.SemanticModel.Compilation;
        if (!IsValidModel(model) || !compilation.IsSymbolAccessibleWithin(model, adapter))
        {
            return Model.Invalid(Issue.Create(IssueId.InvalidModel, model.Locations.FirstOrDefault(), model.Name));
        }

        var symbols = GetPublicInstanceMembers(model, cancellationToken);
        if (symbols.Length == 0)
        {
            return Model.Invalid(Issue.Create(IssueId.NoMembers, model.Locations.FirstOrDefault(), model.Name));
        }

        var members = ImmutableArray.CreateBuilder<Member>(symbols.Length);
        var names = new Dictionary<string, string>(StringComparer.Ordinal);
        var indices = new Dictionary<int, string>();
        for (var order = 0; order < symbols.Length; ++order)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var symbol = symbols[order];
            if (symbol is IPropertySymbol property && property.IsIndexer)
            {
                return Model.Invalid(Issue.Create(IssueId.UnsupportedMember, property.Locations.FirstOrDefault(), property.Name,
                    "is an indexer, which cannot be mapped"));
            }
            if (!TryCreateMember(symbol, order, compilation, adapter, out var member, out var error))
            {
                return Model.Invalid(Issue.Create(IssueId.UnsupportedMember, symbol.Locations.FirstOrDefault(), symbol.Name, error));
            }
            if (!TryGetColumnMapping(symbol, member.IsString, out var mapping, out error))
            {
                return Model.Invalid(Issue.Create(IssueId.InvalidColumn, symbol.Locations.FirstOrDefault(), symbol.Name, error));
            }
            if (names.TryGetValue(mapping.Name, out var existingName))
            {
                return Model.Invalid(Issue.Create(IssueId.DuplicateColumn, symbol.Locations.FirstOrDefault(),
                    $"name '{mapping.Name}'", existingName, symbol.Name));
            }
            names.Add(mapping.Name, symbol.Name);
            if (mapping.Index is int index)
            {
                if (indices.TryGetValue(index, out var existingIndex))
                {
                    return Model.Invalid(Issue.Create(IssueId.DuplicateColumn, symbol.Locations.FirstOrDefault(),
                        $"index {index}", existingIndex, symbol.Name));
                }
                indices.Add(index, symbol.Name);
            }
            members.Add(member.WithMapping(mapping));
        }

        var immutableMembers = members.ToImmutable();
        var usesIndexes = indices.Count > 0;
        if (usesIndexes && !HasCompleteIndexedLayout(immutableMembers))
        {
            return Model.Invalid(Issue.Create(IssueId.IndexLayout, model.Locations.FirstOrDefault(), model.Name,
                "all mapped members must specify contiguous indexes starting at zero"));
        }

        if (!TryCreateConstructionPlan(model, immutableMembers, compilation, adapter, cancellationToken, out var construction, out var issue))
        {
            return Model.Invalid(issue!);
        }

        return new Model(
            NamespaceName(adapter.ContainingNamespace),
            AccessibilityText(adapter.DeclaredAccessibility),
            EscapeIdentifier(adapter.Name),
            adapter.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            model.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            immutableMembers,
            construction!,
            usesIndexes);
    }

    static bool SupportsStaticExtensionMembers(CSharpParseOptions parseOptions) =>
        !CSharpSyntaxTree.ParseText(
            "static class C { extension(object) { public static void M() { } } }",
            parseOptions).GetDiagnostics().Any(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);

    static ImmutableArray<ISymbol> GetPublicInstanceMembers(INamedTypeSymbol model, CancellationToken cancellationToken)
    {
        var members = new List<ISymbol>();
        var names = new HashSet<string>(StringComparer.Ordinal);
        for (var current = model; current is not null && current.SpecialType != SpecialType.System_Object; current = current.BaseType)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach (var symbol in current.GetMembers())
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (symbol.IsStatic || symbol.DeclaredAccessibility != Accessibility.Public ||
                    symbol is not IPropertySymbol and not IFieldSymbol)
                {
                    continue;
                }

                // C# member lookup resolves a hidden member to the most-derived declaration.
                // Keeping that declaration avoids emitting an inaccessible base initializer.
                if (names.Add(symbol.Name))
                {
                    members.Add(symbol);
                }
            }
        }
        members.Sort(static (left, right) =>
        {
            var leftStart = left.Locations.FirstOrDefault()?.SourceSpan.Start ?? int.MaxValue;
            var rightStart = right.Locations.FirstOrDefault()?.SourceSpan.Start ?? int.MaxValue;
            var byLocation = leftStart.CompareTo(rightStart);
            return byLocation != 0 ? byLocation : StringComparer.Ordinal.Compare(left.Name, right.Name);
        });
        return members.ToImmutableArray();
    }

    static bool TryCreateMember(
        ISymbol symbol,
        int order,
        Compilation compilation,
        INamedTypeSymbol adapter,
        out Member member,
        out string error)
    {
        var type = symbol is IPropertySymbol property ? property.Type : ((IFieldSymbol)symbol).Type;
        // Every mapped member is formatted, so an inaccessible getter would silently drop its
        // column and make parsing the formatted output fail.
        if (symbol is IPropertySymbol readableProperty && !IsAccessible(compilation, adapter, readableProperty.GetMethod))
        {
            member = default!;
            error = "does not have an accessible getter";
            return false;
        }
        var canWrite = symbol is IPropertySymbol writableProperty
            ? IsAccessible(compilation, adapter, writableProperty.SetMethod)
            : !((IFieldSymbol)symbol).IsReadOnly;

        var isNullableValue = type is INamedTypeSymbol namedType &&
            namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
        var valueType = isNullableValue ? ((INamedTypeSymbol)type).TypeArguments[0] : type;
        var isString = valueType.SpecialType == SpecialType.System_String;
        var isEnum = valueType.TypeKind == TypeKind.Enum;
        var isNullableReference = !isNullableValue && !valueType.IsValueType &&
            type.NullableAnnotation == NullableAnnotation.Annotated;
        if (!isString && !isEnum && !SupportsSpanConversion(valueType))
        {
            member = default!;
            error = "must be a string, enum, or implement ISpanParsable<TSelf> and ISpanFormattable";
            return false;
        }

        member = new Member(
            symbol.Name,
            type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            type.WithNullableAnnotation(NullableAnnotation.None).ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            valueType.WithNullableAnnotation(NullableAnnotation.None).ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            isString,
            isEnum,
            isNullableValue || isNullableReference,
            isNullableValue,
            isEnum ? GetEnumMemberNames(valueType) : ImmutableArray<string>.Empty,
            canWrite,
            symbol is IPropertySymbol,
            IsRequired(symbol),
            order);
        error = string.Empty;
        return true;
    }

    static ImmutableArray<string> GetEnumMemberNames(ITypeSymbol type)
    {
        var names = ImmutableArray.CreateBuilder<string>();
        // Aliases share a constant value and would become duplicate switch labels.
        var values = new HashSet<object>();
        foreach (var symbol in type.GetMembers())
        {
            if (symbol is IFieldSymbol { IsConst: true, ConstantValue: not null } field &&
                values.Add(field.ConstantValue))
            {
                names.Add(field.Name);
            }
        }
        return names.ToImmutable();
    }

    // Hand rolled accessibility checks cannot see assembly identity, so an internal member of a
    // model in another assembly would look accessible and emit code that does not compile.
    static bool IsAccessible(Compilation compilation, INamedTypeSymbol adapter, IMethodSymbol? method) =>
        method is not null && compilation.IsSymbolAccessibleWithin(method, adapter);

    static bool IsRequired(ISymbol symbol) =>
        symbol is IPropertySymbol property ? property.IsRequired : ((IFieldSymbol)symbol).IsRequired;

    static bool SupportsSpanConversion(ITypeSymbol type) =>
    type.AllInterfaces.Any(@interface =>
        @interface.OriginalDefinition.MetadataName == "ISpanParsable`1" &&
        @interface.OriginalDefinition.ContainingNamespace.ToDisplayString() == "System" &&
        @interface.TypeArguments.Length == 1 &&
        SymbolEqualityComparer.Default.Equals(@interface.TypeArguments[0], type)) &&
    type.AllInterfaces.Any(@interface =>
        @interface.MetadataName == "ISpanFormattable" &&
        @interface.ContainingNamespace.ToDisplayString() == "System");

    static bool TryGetColumnMapping(ISymbol symbol, bool isString, out ColumnMapping mapping, out string error)
    {
        var attributes = symbol.GetAttributes()
            .Where(static attribute => attribute.AttributeClass?.ToDisplayString() == ColumnAttributeMetadataName)
            .ToArray();
        if (attributes.Length == 0)
        {
            mapping = new ColumnMapping(symbol.Name, null, null);
            error = string.Empty;
            return true;
        }
        if (attributes.Length > 1)
        {
            mapping = default;
            error = "only one SepCol attribute is allowed";
            return false;
        }

        string? name = null;
        string? format = null;
        int? index = null;
        var attribute = attributes[0];
        var constructor = attribute.AttributeConstructor;
        if (constructor is not null)
        {
            for (var parameterIndex = 0; parameterIndex < constructor.Parameters.Length; ++parameterIndex)
            {
                SetColumnMappingValue(
                    constructor.Parameters[parameterIndex].Name,
                    attribute.ConstructorArguments[parameterIndex].Value,
                    ref name,
                    ref index,
                    ref format);
            }
        }
        foreach (var argument in attribute.NamedArguments)
        {
            SetColumnMappingValue(argument.Key, argument.Value.Value, ref name, ref index, ref format);
        }

        name ??= symbol.Name;
        if (string.IsNullOrWhiteSpace(name))
        {
            mapping = default;
            error = "the column name cannot be empty";
            return false;
        }
        if (index is < 0)
        {
            mapping = default;
            error = "the column index must be zero or greater";
            return false;
        }

        // Strings are formatted verbatim, so a format would be silently ignored.
        if (isString && format is not null)
        {
            mapping = default;
            error = "a format cannot be specified for a string member";
            return false;
        }

        mapping = new ColumnMapping(name, index, format);
        error = string.Empty;
        return true;
    }

    static void SetColumnMappingValue(
        string name,
        object? value,
        ref string? columnName,
        ref int? columnIndex,
        ref string? format)
    {
        if (string.Equals(name, "name", StringComparison.OrdinalIgnoreCase))
        {
            columnName = value as string;
        }
        else if (string.Equals(name, "index", StringComparison.OrdinalIgnoreCase) && value is int index)
        {
            columnIndex = index;
        }
        else if (string.Equals(name, "format", StringComparison.OrdinalIgnoreCase))
        {
            format = value as string;
        }
    }

    static bool HasCompleteIndexedLayout(ImmutableArray<Member> members)
    {
        for (var index = 0; index < members.Length; ++index)
        {
            if (!members.Any(member => member.Index == index))
            {
                return false;
            }
        }
        return true;
    }

    static bool TryCreateConstructionPlan(
        INamedTypeSymbol model,
        ImmutableArray<Member> members,
        Compilation compilation,
        INamedTypeSymbol adapter,
        CancellationToken cancellationToken,
        out ConstructionPlan? plan,
        out Issue? issue)
    {
        if (model.TypeKind == TypeKind.Struct && members.All(static member => member.CanWrite))
        {
            // Every member is assignable, so the implicit parameterless constructor plus an object
            // initializer for all members is always the best plan.
            var allMembers = ImmutableArray.CreateBuilder<int>(members.Length);
            for (var memberIndex = 0; memberIndex < members.Length; ++memberIndex)
            {
                allMembers.Add(memberIndex);
            }
            plan = new ConstructionPlan(ImmutableArray<ConstructorParameter>.Empty, allMembers.ToImmutable());
            issue = null;
            return true;
        }

        var accessibleConstructors = model.InstanceConstructors
            .Where(constructor => compilation.IsSymbolAccessibleWithin(constructor, adapter))
            .ToImmutableArray();
        if (accessibleConstructors.Length == 0)
        {
            plan = null;
            issue = Issue.Create(IssueId.NoUsableConstructor, model.Locations.FirstOrDefault(), model.Name);
            return false;
        }

        var candidates = new List<ConstructionPlan>();
        foreach (var constructor in accessibleConstructors)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (TryCreateConstructionPlan(constructor, members, out var candidate))
            {
                candidates.Add(candidate);
            }
        }
        if (candidates.Count == 0)
        {
            var unassignable = members.FirstOrDefault(static member => !member.CanWrite);
            plan = null;
            issue = unassignable is { IsProperty: true }
                ? Issue.Create(IssueId.InaccessibleSetter, model.Locations.FirstOrDefault(), unassignable.Name)
                : unassignable is not null
                    ? Issue.Create(IssueId.UnbindableConstructor, model.Locations.FirstOrDefault(), model.Name, unassignable.Name)
                : Issue.Create(IssueId.UnbindableConstructor, model.Locations.FirstOrDefault(), model.Name, "constructor parameters");
            return false;
        }

        var bestBoundCount = candidates.Max(static candidate => candidate.Parameters.Length);
        var best = candidates.Where(candidate => candidate.Parameters.Length == bestBoundCount).ToArray();
        if (best.Length != 1)
        {
            plan = null;
            issue = Issue.Create(IssueId.AmbiguousConstructor, model.Locations.FirstOrDefault(), model.Name);
            return false;
        }

        plan = best[0];
        issue = null;
        return true;
    }

    static bool TryCreateConstructionPlan(
        IMethodSymbol constructor,
        ImmutableArray<Member> members,
        out ConstructionPlan plan)
    {
        var bindings = ImmutableArray.CreateBuilder<ConstructorParameter>();
        var boundMembers = new HashSet<int>();
        for (var parameterIndex = 0; parameterIndex < constructor.Parameters.Length; ++parameterIndex)
        {
            var parameter = constructor.Parameters[parameterIndex];
            var memberIndex = FindConstructorMember(parameter, members);
            if (memberIndex < 0)
            {
                if (parameter.HasExplicitDefaultValue)
                {
                    continue;
                }
                plan = default!;
                return false;
            }
            if (!boundMembers.Add(memberIndex))
            {
                plan = default!;
                return false;
            }
            bindings.Add(new ConstructorParameter(EscapeIdentifier(parameter.Name), memberIndex));
        }

        for (var memberIndex = 0; memberIndex < members.Length; ++memberIndex)
        {
            if (!members[memberIndex].CanWrite && !boundMembers.Contains(memberIndex))
            {
                plan = default!;
                return false;
            }
        }

        var initializers = ImmutableArray.CreateBuilder<int>();
        var setsRequiredMembers = constructor.GetAttributes().Any(static attribute =>
            attribute.AttributeClass?.ToDisplayString() == "System.Diagnostics.CodeAnalysis.SetsRequiredMembersAttribute");
        for (var memberIndex = 0; memberIndex < members.Length; ++memberIndex)
        {
            var member = members[memberIndex];
            if (!boundMembers.Contains(memberIndex) ||
                (member.IsRequired && !setsRequiredMembers && member.CanWrite))
            {
                initializers.Add(memberIndex);
            }
        }
        plan = new ConstructionPlan(bindings.ToImmutable(), initializers.ToImmutable());
        return true;
    }

    static int FindConstructorMember(IParameterSymbol parameter, ImmutableArray<Member> members)
    {
        for (var memberIndex = 0; memberIndex < members.Length; ++memberIndex)
        {
            var member = members[memberIndex];
            if (string.Equals(parameter.Name, member.Name, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(
                    parameter.Type.WithNullableAnnotation(NullableAnnotation.None)
                        .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    member.TypeIdentityName,
                    StringComparison.Ordinal))
            {
                return memberIndex;
            }
        }
        return -1;
    }

    static bool IsValidAdapter(INamedTypeSymbol adapter, CancellationToken cancellationToken)
    {
        if (!adapter.IsStatic || adapter.Arity != 0 || adapter.ContainingType is not null)
        {
            return false;
        }
        // Multiple parts are expected since the adapter is partial and users may add own members.
        var isPartial = false;
        foreach (var syntaxReference in adapter.DeclaringSyntaxReferences)
        {
            var declaration = (ClassDeclarationSyntax)syntaxReference.GetSyntax(cancellationToken);
            if (declaration.Modifiers.Any(static modifier => modifier.IsKind(SyntaxKind.FileKeyword)))
            {
                return false;
            }
            isPartial |= declaration.Modifiers.Any(static modifier => modifier.IsKind(SyntaxKind.PartialKeyword));
        }
        return isPartial;
    }

    static bool IsValidModel(INamedTypeSymbol model) =>
        !model.IsAbstract &&
        !model.IsStatic &&
        !model.IsRefLikeType &&
        model.TypeKind is TypeKind.Class or TypeKind.Struct;

    static bool IsGenericModel(INamedTypeSymbol model)
    {
        for (var current = model; current is not null; current = current.ContainingType)
        {
            if (current.Arity != 0)
            {
                return true;
            }
        }
        return false;
    }

    static string? NamespaceName(INamespaceSymbol @namespace)
    {
        if (@namespace.IsGlobalNamespace)
        {
            return null;
        }

        var names = new Stack<string>();
        for (var current = @namespace; !current.IsGlobalNamespace; current = current.ContainingNamespace)
        {
            names.Push(EscapeIdentifier(current.Name));
        }
        return string.Join(".", names);
    }

    static string AccessibilityText(Accessibility accessibility) =>
        accessibility == Accessibility.Public ? "public" : "internal";

    static string EscapeIdentifier(string identifier) => "@" + identifier;

    static void Emit(SourceProductionContext context, Model model)
    {
        if (model.Issue is not null)
        {
            context.ReportDiagnostic(CreateDiagnostic(model.Issue));
            return;
        }

        var source = new StringBuilder();
        source.AppendLine("// <auto-generated/>");
        source.AppendLine("#nullable enable");
        if (model.Namespace is not null)
        {
            source.Append("namespace ").Append(model.Namespace).AppendLine(";");
            source.AppendLine();
        }
        source.Append(model.Accessibility).Append(" static partial class ").Append(model.AdapterName).AppendLine();
        source.AppendLine("{");
        source.Append("    extension(").Append(model.ModelName).AppendLine(")");
        source.AppendLine("    {");
        var extensions = new StringBuilder();
        EmitParse(extensions, model);
        extensions.AppendLine();
        EmitTryParse(extensions, model);
        extensions.AppendLine();
        EmitEnumerate(extensions, model);
        extensions.AppendLine();
        EmitEnumerateAsync(extensions, model);
        extensions.AppendLine();
        EmitFormat(extensions, model);
        extensions.AppendLine();
        EmitWrite(extensions, model);
        extensions.AppendLine();
        EmitWriteAsyncOverloads(extensions, model);
        AppendIndented(source, extensions, "    ");
        source.AppendLine("    }");
        if (model.Members.Any(static member => member.IsEnum))
        {
            source.AppendLine();
            EmitSetEnum(source);
        }
        source.AppendLine();
        EmitEnumerator(source, model);
        source.AppendLine("}");

        context.AddSource(HintName(model), SourceText.From(source.ToString(), Encoding.UTF8));
    }

    static void AppendIndented(StringBuilder source, StringBuilder value, string indent)
    {
        using var reader = new System.IO.StringReader(value.ToString());
        while (reader.ReadLine() is { } line)
        {
            source.Append(indent).AppendLine(line);
        }
    }

    internal static Diagnostic CreateDiagnostic(Issue issue) =>
        Diagnostic.Create(
            s_descriptors[(int)issue.Id - 1],
            issue.Location?.ToLocation(),
            issue.Arguments.Cast<object>().ToArray());

    static void EmitParse(StringBuilder source, Model model)
    {
        source.Append("    public static ").Append(model.ModelName)
            .Append(" Parse(").Append(SepReaderRowName).AppendLine(" row)");
        source.AppendLine("    {");
        for (var memberIndex = 0; memberIndex < model.Members.Length; ++memberIndex)
        {
            var member = model.Members[memberIndex];
            if (!member.IsNullable)
            {
                continue;
            }
            // Hoist the column so the empty check and the parse share a single column lookup.
            source.Append("        var ").Append(ColumnLocal(memberIndex)).Append(" = ")
                .Append(ColumnAccess(model, member)).AppendLine(";");
            source.Append("        ");
            AppendLocalType(source, member);
            source.Append(' ').Append(ValueLocal(memberIndex)).Append(" = ")
                .Append(ColumnLocal(memberIndex)).Append(".Span.IsEmpty ? null : ");
            AppendValueExpression(source, member, ColumnLocal(memberIndex));
            source.AppendLine(";");
        }
        source.Append("        return ");
        AppendConstruction(source, model, memberIndex => AppendParseValue(source, model, memberIndex));
        source.AppendLine(";");
        source.AppendLine("    }");
    }

    static void AppendParseValue(StringBuilder source, Model model, int memberIndex)
    {
        var member = model.Members[memberIndex];
        if (member.IsNullable)
        {
            source.Append(ValueLocal(memberIndex));
            return;
        }
        AppendValueExpression(source, member, ColumnAccess(model, member));
    }

    static string ColumnLocal(int memberIndex) => "__col" + memberIndex;

    static string ValueLocal(int memberIndex) => "__sep" + memberIndex;

    static string ColumnAccess(Model model, Member member) => model.UsesIndexes
        ? "row[" + member.Index!.Value + "]"
        : "row[" + SymbolDisplay.FormatLiteral(member.ColumnName, quote: true) + "]";

    static void AppendLocalType(StringBuilder source, Member member)
    {
        source.Append(member.TypeName);
        if (!member.IsNullableValue)
        {
            source.Append('?');
        }
    }

    static void AppendValueExpression(StringBuilder source, Member member, string column)
    {
        if (member.IsString)
        {
            source.Append(column).Append(".ToString()");
        }
        else if (member.IsEnum)
        {
            source.Append(EnumName).Append(".Parse<").Append(member.ValueTypeName).Append(">(")
                .Append(column).Append(".Span)");
        }
        else
        {
            source.Append(column).Append(".Parse<").Append(member.ValueTypeName).Append(">()");
        }
    }

    static void EmitTryParse(StringBuilder source, Model model)
    {
        source.Append("    public static bool TryParse(").Append(SepReaderRowName).Append(" row, out ")
            .Append(model.ModelName).AppendLine(" value)");
        source.AppendLine("    {");
        for (var memberIndex = 0; memberIndex < model.Members.Length; ++memberIndex)
        {
            EmitTryParseMember(source, model, model.Members[memberIndex], memberIndex);
        }
        source.AppendLine();
        source.Append("        value = ");
        AppendConstruction(source, model, memberIndex => source.Append(ValueLocal(memberIndex)));
        source.AppendLine(";");
        source.AppendLine("        return true;");
        source.AppendLine("    }");
    }

    static void EmitTryParseMember(StringBuilder source, Model model, Member member, int memberIndex)
    {
        var local = ValueLocal(memberIndex);
        if (member.IsNullable)
        {
            // Hoist the column so the empty check and the parse share a single column lookup.
            var column = ColumnLocal(memberIndex);
            source.Append("        var ").Append(column).Append(" = ")
                .Append(ColumnAccess(model, member)).AppendLine(";");
            source.Append("        ");
            AppendLocalType(source, member);
            source.Append(' ').Append(local).AppendLine(";");
            source.Append("        if (").Append(column).AppendLine(".Span.IsEmpty)");
            source.AppendLine("        {");
            source.Append("            ").Append(local).AppendLine(" = null;");
            source.AppendLine("        }");
            source.AppendLine("        else");
            source.AppendLine("        {");
            if (member.IsString)
            {
                source.Append("            ").Append(local).Append(" = ").Append(column).AppendLine(".ToString();");
            }
            else
            {
                EmitTryParseValue(source, member, column, local + "Value", "            ");
                source.Append("            ").Append(local).Append(" = ").Append(local).AppendLine("Value;");
            }
            source.AppendLine("        }");
            return;
        }

        if (member.IsString)
        {
            source.Append("        var ").Append(local).Append(" = ")
                .Append(ColumnAccess(model, member)).AppendLine(".ToString();");
            return;
        }

        EmitTryParseValue(source, member, ColumnAccess(model, member), local, "        ");
    }

    static void EmitTryParseValue(StringBuilder source, Member member, string column, string local, string indent)
    {
        source.Append(indent).Append("if (!");
        if (member.IsEnum)
        {
            source.Append(EnumName).Append(".TryParse<").Append(member.ValueTypeName).Append(">(")
                .Append(column).Append(".Span, out var ").Append(local).AppendLine("))");
        }
        else
        {
            source.Append(column).Append(".TryParse<").Append(member.ValueTypeName)
                .Append(">(out var ").Append(local).AppendLine("))");
        }
        source.Append(indent).AppendLine("{");
        source.Append(indent).AppendLine("    value = default!;");
        source.Append(indent).AppendLine("    return false;");
        source.Append(indent).AppendLine("}");
    }

    static void EmitEnumerate(StringBuilder source, Model model)
    {
        // A struct enumerable and enumerator keep parsing value type models free of heap
        // allocations, unlike an iterator based IEnumerable<T>.
        source.Append("    public static ModelEnumerable Enumerate(").Append(SepReaderName)
            .AppendLine(" reader) => new ModelEnumerable(reader);");
    }

    static void EmitEnumerator(StringBuilder source, Model model)
    {
        var enumerableInterface = EnumerableName + "<" + model.ModelName + ">";
        var enumeratorInterface = EnumeratorName + "<" + model.ModelName + ">";
        var readerField = "        readonly " + SepReaderName + " _reader;";
        source.Append("    public readonly struct ModelEnumerable : ").AppendLine(enumerableInterface);
        source.AppendLine("    {");
        source.AppendLine(readerField);
        source.AppendLine();
        source.Append("        internal ModelEnumerable(").Append(SepReaderName).AppendLine(" reader)");
        source.AppendLine("        {");
        source.Append("            ").Append(ArgumentNullExceptionName).AppendLine(".ThrowIfNull(reader);");
        source.AppendLine("            _reader = reader;");
        source.AppendLine("        }");
        source.AppendLine();
        source.AppendLine("        public ModelEnumerator GetEnumerator() => new ModelEnumerator(_reader);");
        source.AppendLine();
        source.Append("        ").Append(enumeratorInterface).Append(' ').Append(enumerableInterface)
            .AppendLine(".GetEnumerator() => GetEnumerator();");
        source.AppendLine();
        source.Append("        ").Append(NonGenericEnumeratorName).Append(' ').Append(NonGenericEnumerableName)
            .AppendLine(".GetEnumerator() => GetEnumerator();");
        source.AppendLine("    }");
        source.AppendLine();
        source.Append("    public struct ModelEnumerator : ").AppendLine(enumeratorInterface);
        source.AppendLine("    {");
        source.AppendLine(readerField);
        source.Append("        ").Append(model.ModelName).AppendLine(" _current;");
        source.AppendLine();
        source.Append("        internal ModelEnumerator(").Append(SepReaderName).AppendLine(" reader)");
        source.AppendLine("        {");
        source.AppendLine("            _reader = reader;");
        source.AppendLine("            _current = default!;");
        source.AppendLine("        }");
        source.AppendLine();
        source.Append("        public ").Append(model.ModelName).AppendLine(" Current => _current;");
        source.AppendLine();
        source.Append("        object? ").Append(NonGenericEnumeratorName).AppendLine(".Current => _current;");
        source.AppendLine();
        source.AppendLine("        public bool MoveNext()");
        source.AppendLine("        {");
        source.AppendLine("            if (_reader.MoveNext())");
        source.AppendLine("            {");
        source.Append("                _current = ").Append(model.ModelName).AppendLine(".Parse(_reader.Current);");
        source.AppendLine("                return true;");
        source.AppendLine("            }");
        source.AppendLine("            _current = default!;");
        source.AppendLine("            return false;");
        source.AppendLine("        }");
        source.AppendLine();
        source.AppendLine("        public void Dispose() { }");
        source.AppendLine();
        source.Append("        void ").Append(NonGenericEnumeratorName).Append(".Reset() => throw new ")
            .Append(NotSupportedExceptionName).AppendLine("();");
        source.AppendLine("    }");
    }

    static void EmitSetEnum(StringBuilder source)
    {
        // Enum.TryFormat is a static generic taking the concrete enum type, so unlike the
        // ISpanFormattable implementation on System.Enum it neither boxes nor allocates.
        var signature = "<TEnum>(" + SepWriterColName + " col, TEnum value, " + ReadOnlySpanName + "<char> format)";
        var constraint = "        where TEnum : struct, " + EnumName;
        source.Append("    static void SetEnum").AppendLine(signature);
        source.AppendLine(constraint);
        source.AppendLine("    {");
        source.Append("        ").Append(SpanName).Append("<char> chars = stackalloc char[")
            .Append(EnumStackallocCharCount).AppendLine("];");
        source.Append("        if (").Append(EnumName).AppendLine(".TryFormat(value, chars, out var charsWritten, format))");
        source.AppendLine("        {");
        source.AppendLine("            col.Set(chars.Slice(0, charsWritten));");
        source.AppendLine("            return;");
        source.AppendLine("        }");
        source.AppendLine("        SetEnumPooled(col, value, format);");
        source.AppendLine("    }");
        source.AppendLine();
        source.Append("    static void SetEnumPooled").AppendLine(signature);
        source.AppendLine(constraint);
        source.AppendLine("    {");
        source.Append("        for (var length = ").Append(EnumStackallocCharCount * 2).AppendLine("; ; length *= 2)");
        source.AppendLine("        {");
        source.Append("            var array = ").Append(ArrayPoolName).AppendLine(".Shared.Rent(length);");
        source.Append("            var formatted = ").Append(EnumName)
            .AppendLine(".TryFormat(value, array, out var charsWritten, format);");
        source.AppendLine("            if (formatted)");
        source.AppendLine("            {");
        source.Append("                col.Set(").Append(MemoryExtensionsName).AppendLine(".AsSpan(array, 0, charsWritten));");
        source.AppendLine("            }");
        source.Append("            ").Append(ArrayPoolName).AppendLine(".Shared.Return(array);");
        source.AppendLine("            if (formatted) { return; }");
        source.AppendLine("        }");
        source.AppendLine("    }");
    }

    static void EmitEnumerateAsync(StringBuilder source, Model model)
    {
        source.Append("    public static ").Append(AsyncEnumerableName).Append('<')
            .Append(model.ModelName).Append("> EnumerateAsync(").Append(SepReaderName).Append(" reader) => ")
            .Append(SepReaderExtensionsName).Append(".EnumerateAsync(reader, ")
            .Append(model.ModelName).AppendLine(".Parse);");
    }

    static void EmitFormat(StringBuilder source, Model model)
    {
        source.Append("    public static void Format(").Append(SepWriterRowName).Append(" row, ")
            .Append(model.ModelName).AppendLine(" value)");
        source.AppendLine("    {");
        foreach (var member in model.Members
            .OrderBy(member => model.UsesIndexes ? member.Index : member.Order)
            .ThenBy(static member => member.Order))
        {
            EmitFormatMember(source, member);
        }
        source.AppendLine("    }");
    }

    static void EmitFormatMember(StringBuilder source, Member member)
    {
        var value = "value.@" + member.Name;
        if (member.IsString)
        {
            source.Append("        ");
            AppendSetSpan(source, WriteColumnAccess(member), value);
            return;
        }

        // Enum and nullable members use the column more than once, so hoist it into a local to
        // only look it up once.
        var hoist = member.IsEnum || member.IsNullable;
        var column = hoist ? ColumnLocal(member.Order) : WriteColumnAccess(member);
        if (hoist)
        {
            source.Append("        var ").Append(column).Append(" = ")
                .Append(WriteColumnAccess(member)).AppendLine(";");
        }

        if (!member.IsNullable)
        {
            EmitFormatValue(source, member, column, value, "        ");
            return;
        }

        // GetValueOrDefault avoids the redundant HasValue check done by Value.
        var hasValue = member.IsNullableValue ? value + ".HasValue" : value + " is not null";
        var innerValue = member.IsNullableValue ? value + ".GetValueOrDefault()" : value;
        source.Append("        if (").Append(hasValue).AppendLine(")");
        source.AppendLine("        {");
        EmitFormatValue(source, member, column, innerValue, "            ");
        source.AppendLine("        }");
        source.AppendLine("        else");
        source.AppendLine("        {");
        source.Append("            ").Append(column).Append(".Set(").Append(ReadOnlySpanName).AppendLine("<char>.Empty);");
        source.AppendLine("        }");
    }

    static void EmitFormatValue(StringBuilder source, Member member, string column, string value, string indent)
    {
        if (member.IsEnum)
        {
            EmitFormatEnum(source, member, column, value, indent);
            return;
        }
        source.Append(indent).Append(column).Append(".Format(").Append(value);
        if (member.Format is not null)
        {
            source.Append(", ");
            AppendStringLiteral(source, member.Format);
        }
        source.AppendLine(");");
    }

    // Enums cannot use Col.Format like other ISpanFormattable types. ISpanFormattable is implemented
    // by System.Enum, not by each enum type, so the constrained call a generic
    // `Format<T>(T) where T : ISpanFormattable` compiles to has to box the value, which allocates
    // 24 bytes per value. Parsing has the same shape of problem: enums do not implement
    // ISpanParsable<TSelf> at all, so Col.Parse<T> cannot even be used. Knowing the concrete enum
    // type at generation time makes both allocation free.
    static void EmitFormatEnum(StringBuilder source, Member member, string column, string value, string indent)
    {
        var isGeneralFormat = member.Format is null || string.Equals(member.Format, "G", StringComparison.OrdinalIgnoreCase);
        if (!isGeneralFormat || member.EnumMemberNames.Length == 0)
        {
            source.Append(indent).Append("SetEnum(").Append(column).Append(", ").Append(value).Append(", ");
            if (member.Format is null)
            {
                source.Append("default");
            }
            else
            {
                AppendStringLiteral(source, member.Format);
            }
            source.AppendLine(");");
            return;
        }

        // Defined values map directly to constant names, which avoids all formatting, boxing and
        // allocation. Undefined values and flags combinations fall back to Enum.TryFormat.
        source.Append(indent).Append("switch (").Append(value).AppendLine(")");
        source.Append(indent).AppendLine("{");
        foreach (var name in member.EnumMemberNames)
        {
            source.Append(indent).Append("    case ").Append(member.ValueTypeName).Append(".@").Append(name).AppendLine(":");
            source.Append(indent).Append("        ");
            AppendSetSpan(source, column, SymbolDisplay.FormatLiteral(name, quote: true));
            source.Append(indent).AppendLine("        break;");
        }
        source.Append(indent).AppendLine("    default:");
        source.Append(indent).Append("        SetEnum(").Append(column).Append(", ").Append(value).AppendLine(", default);");
        source.Append(indent).AppendLine("        break;");
        source.Append(indent).AppendLine("}");
    }

    static void AppendSetSpan(StringBuilder source, string column, string value) =>
        source.Append(column).Append(".Set(").Append(MemoryExtensionsName).Append(".AsSpan(")
            .Append(value).AppendLine("));");

    static string WriteColumnAccess(Member member) =>
        "row[" + SymbolDisplay.FormatLiteral(member.ColumnName, quote: true) + "]";

    static void AppendConstruction(StringBuilder source, Model model, Action<int> appendValue)
    {
        source.Append("new ").Append(model.ModelName).Append('(');
        for (var parameterIndex = 0; parameterIndex < model.Construction.Parameters.Length; ++parameterIndex)
        {
            if (parameterIndex > 0)
            {
                source.Append(", ");
            }
            var parameter = model.Construction.Parameters[parameterIndex];
            source.Append(parameter.Name).Append(": ");
            appendValue(parameter.MemberIndex);
        }
        source.Append(')');
        if (model.Construction.Initializers.Length == 0)
        {
            return;
        }

        source.Append(" { ");
        for (var initializerIndex = 0; initializerIndex < model.Construction.Initializers.Length; ++initializerIndex)
        {
            if (initializerIndex > 0)
            {
                source.Append(", ");
            }
            var memberIndex = model.Construction.Initializers[initializerIndex];
            source.Append('@').Append(model.Members[memberIndex].Name).Append(" = ");
            appendValue(memberIndex);
        }
        source.Append(" }");
    }

    static void AppendStringLiteral(StringBuilder source, string value) =>
        source.Append(SymbolDisplay.FormatLiteral(value, quote: true));

    static void EmitWrite(StringBuilder source, Model model)
    {
        // A span overload avoids the enumerator allocation an IEnumerable<T> enumeration incurs.
        // params is not used since that requires C# 13 which consumers cannot be assumed to use.
        source.Append("    public static void Write(").Append(SepWriterName).Append(" writer, ")
            .Append(ReadOnlySpanName).Append('<').Append(model.ModelName).AppendLine("> values)");
        source.AppendLine("    {");
        source.Append("        ").Append(ArgumentNullExceptionName).AppendLine(".ThrowIfNull(writer);");
        source.AppendLine("        for (var index = 0; index < values.Length; ++index)");
        source.AppendLine("        {");
        source.AppendLine("            using var row = writer.NewRow();");
        source.Append("            ").Append(model.ModelName).AppendLine(".Format(row, values[index]);");
        source.AppendLine("        }");
        source.AppendLine("    }");
        source.AppendLine();
        // An array converts to both the span and the IEnumerable overload with neither being
        // better, which makes the most common call ambiguous. This exact match overload resolves
        // that and keeps arrays on the allocation free span path.
        source.Append("    public static void Write(").Append(SepWriterName).Append(" writer, ")
            .Append(model.ModelName).AppendLine("[] values)");
        source.AppendLine("    {");
        source.Append("        ").Append(ArgumentNullExceptionName).AppendLine(".ThrowIfNull(values);");
        source.Append("        Write(writer, new ").Append(ReadOnlySpanName).Append('<')
            .Append(model.ModelName).AppendLine(">(values));");
        source.AppendLine("    }");
        source.AppendLine();
        source.Append("    public static void Write(").Append(SepWriterName).Append(" writer, ")
            .Append(EnumerableName).Append('<').Append(model.ModelName).AppendLine("> values)");
        source.AppendLine("    {");
        source.Append("        ").Append(ArgumentNullExceptionName).AppendLine(".ThrowIfNull(writer);");
        source.Append("        ").Append(ArgumentNullExceptionName).AppendLine(".ThrowIfNull(values);");
        source.AppendLine("        foreach (var value in values)");
        source.AppendLine("        {");
        source.AppendLine("            using var row = writer.NewRow();");
        source.Append("            ").Append(model.ModelName).AppendLine(".Format(row, value);");
        source.AppendLine("        }");
        source.AppendLine("    }");
    }

    static void EmitWriteAsyncOverloads(StringBuilder source, Model model)
    {
        EmitWriteAsync(source, model, EnumerableName + "<" + model.ModelName + ">", asyncValues: false);
        source.AppendLine();
        EmitWriteAsync(source, model, AsyncEnumerableName + "<" + model.ModelName + ">", asyncValues: true);
    }

    static void EmitWriteAsync(StringBuilder source, Model model, string valuesType, bool asyncValues)
    {
        source.Append("    public static async ").Append(ValueTaskName).Append(" WriteAsync(")
            .Append(SepWriterName).Append(" writer, ").Append(valuesType).Append(" values, ")
            .Append(CancellationTokenName).AppendLine(" cancellationToken = default)");
        source.AppendLine("    {");
        source.Append("        ").Append(ArgumentNullExceptionName).AppendLine(".ThrowIfNull(writer);");
        source.Append("        ").Append(ArgumentNullExceptionName).AppendLine(".ThrowIfNull(values);");
        if (asyncValues)
        {
            source.Append("        await foreach (var value in ").Append(TaskAsyncEnumerableExtensionsName)
                .AppendLine(".WithCancellation(values, cancellationToken).ConfigureAwait(false))");
        }
        else
        {
            source.AppendLine("        foreach (var value in values)");
        }
        source.AppendLine("        {");
        source.AppendLine("            await using var row = writer.NewRow(cancellationToken);");
        source.Append("            ").Append(model.ModelName).AppendLine(".Format(row, value);");
        source.AppendLine("        }");
        source.AppendLine("    }");
    }

    static string HintName(Model model) =>
        model.AdapterName.TrimStart('@') + "_" +
        HashHintIdentity(model.AdapterIdentity + "\0" + model.ModelName) + ".Sep.g.cs";

    static string HashHintIdentity(string value)
    {
        using var hashAlgorithm = SHA256.Create();
        var hash = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
        // Only a short prefix is needed to disambiguate same named adapters in different namespaces.
        const int hintByteCount = 8;
        var builder = new StringBuilder(hintByteCount * 2);
        for (var index = 0; index < hintByteCount; ++index)
        {
            builder.Append(hash[index].ToString("x2", System.Globalization.CultureInfo.InvariantCulture));
        }
        return builder.ToString();
    }

    const string AttributeSource = """
        // <auto-generated/>
        #nullable enable
        namespace nietras.SeparatedValues;

        [global::System.AttributeUsage(global::System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
        [global::System.Diagnostics.Conditional("SEP_SOURCE_GENERATOR")]
        internal sealed class SepSourceGenerationAttribute : global::System.Attribute
        {
            public SepSourceGenerationAttribute(global::System.Type modelType)
            {
            }
        }

        [global::System.AttributeUsage(
            global::System.AttributeTargets.Property | global::System.AttributeTargets.Field,
            Inherited = false,
            AllowMultiple = false)]
        [global::System.Diagnostics.Conditional("SEP_SOURCE_GENERATOR")]
        internal sealed class SepColAttribute : global::System.Attribute
        {
            public SepColAttribute()
            {
            }

            public SepColAttribute(string name)
            {
                Name = name;
            }

            public SepColAttribute(int index)
            {
                Index = index;
            }

            public SepColAttribute(string name, int index)
            {
                Name = name;
                Index = index;
            }

            public string? Name { get; set; }
            public int Index { get; set; }
            public string? Format { get; set; }
        }
        """;

    internal sealed class Model : IEquatable<Model>
    {
        public Model(
            string? @namespace,
            string accessibility,
            string adapterName,
            string adapterIdentity,
            string modelName,
            ImmutableArray<Member> members,
            ConstructionPlan construction,
            bool usesIndexes)
        {
            Namespace = @namespace;
            Accessibility = accessibility;
            AdapterName = adapterName;
            AdapterIdentity = adapterIdentity;
            ModelName = modelName;
            Members = members;
            Construction = construction;
            UsesIndexes = usesIndexes;
        }

        Model(Issue issue)
        {
            Issue = issue;
            Members = ImmutableArray<Member>.Empty;
            Construction = ConstructionPlan.Empty;
        }

        public string? Namespace { get; }
        public string Accessibility { get; } = "";
        public string AdapterName { get; } = "";
        public string AdapterIdentity { get; } = "";
        public string ModelName { get; } = "";
        public ImmutableArray<Member> Members { get; }
        public ConstructionPlan Construction { get; }
        public bool UsesIndexes { get; }
        public Issue? Issue { get; }

        public static Model Invalid(Issue issue) => new(issue);

        public bool Equals(Model? other)
        {
            if (other is null ||
                !string.Equals(Namespace, other.Namespace, StringComparison.Ordinal) ||
                !string.Equals(Accessibility, other.Accessibility, StringComparison.Ordinal) ||
                !string.Equals(AdapterName, other.AdapterName, StringComparison.Ordinal) ||
                !string.Equals(AdapterIdentity, other.AdapterIdentity, StringComparison.Ordinal) ||
                !string.Equals(ModelName, other.ModelName, StringComparison.Ordinal) ||
                UsesIndexes != other.UsesIndexes ||
                !Equals(Issue, other.Issue) ||
                !Construction.Equals(other.Construction) ||
                Members.Length != other.Members.Length)
            {
                return false;
            }
            for (var index = 0; index < Members.Length; ++index)
            {
                if (!Members[index].Equals(other.Members[index]))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object? obj) => Equals(obj as Model);

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Namespace, StringComparer.Ordinal);
            hashCode.Add(Accessibility, StringComparer.Ordinal);
            hashCode.Add(AdapterName, StringComparer.Ordinal);
            hashCode.Add(AdapterIdentity, StringComparer.Ordinal);
            hashCode.Add(ModelName, StringComparer.Ordinal);
            hashCode.Add(UsesIndexes);
            hashCode.Add(Issue);
            hashCode.Add(Construction);
            foreach (var member in Members)
            {
                hashCode.Add(member);
            }
            return hashCode.ToHashCode();
        }
    }

    internal sealed class LocationInfo : IEquatable<LocationInfo>
    {
        LocationInfo(string filePath, TextSpan textSpan, LinePositionSpan lineSpan)
        {
            FilePath = filePath;
            TextSpan = textSpan;
            LineSpan = lineSpan;
        }

        public string FilePath { get; }
        public TextSpan TextSpan { get; }
        public LinePositionSpan LineSpan { get; }

        // A Location roots its syntax tree and thereby the compilation, which must never be kept
        // alive by incremental generator outputs.
        public static LocationInfo? Create(Location? location)
        {
            if (location is null || location.Kind is not (LocationKind.SourceFile or LocationKind.ExternalFile))
            {
                return null;
            }
            var lineSpan = location.GetLineSpan();
            return new LocationInfo(lineSpan.Path, location.SourceSpan, lineSpan.Span);
        }

        public Location ToLocation() => Location.Create(FilePath, TextSpan, LineSpan);

        public bool Equals(LocationInfo? other) =>
            other is not null &&
            string.Equals(FilePath, other.FilePath, StringComparison.Ordinal) &&
            TextSpan.Equals(other.TextSpan) &&
            LineSpan.Equals(other.LineSpan);

        public override bool Equals(object? obj) => Equals(obj as LocationInfo);

        public override int GetHashCode() => HashCode.Combine(FilePath, TextSpan, LineSpan);
    }

    internal sealed class Issue : IEquatable<Issue>
    {
        Issue(IssueId id, LocationInfo? location, ImmutableArray<string> arguments)
        {
            Id = id;
            Location = location;
            Arguments = arguments;
        }

        public IssueId Id { get; }
        public LocationInfo? Location { get; }
        public ImmutableArray<string> Arguments { get; }

        public static Issue Create(IssueId id, Location? location, params string[] arguments) =>
            new(id, LocationInfo.Create(location), arguments.ToImmutableArray());

        public bool Equals(Issue? other)
        {
            if (other is null ||
                Id != other.Id ||
                !Equals(Location, other.Location) ||
                Arguments.Length != other.Arguments.Length)
            {
                return false;
            }
            for (var index = 0; index < Arguments.Length; ++index)
            {
                if (!string.Equals(Arguments[index], other.Arguments[index], StringComparison.Ordinal))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object? obj) => Equals(obj as Issue);

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Id);
            hashCode.Add(Location);
            foreach (var argument in Arguments)
            {
                hashCode.Add(argument, StringComparer.Ordinal);
            }
            return hashCode.ToHashCode();
        }
    }

    internal readonly struct ColumnMapping
    {
        public ColumnMapping(string name, int? index, string? format)
        {
            Name = name;
            Index = index;
            Format = format;
        }

        public string Name { get; }
        public int? Index { get; }
        public string? Format { get; }
    }

    internal sealed class Member : IEquatable<Member>
    {
        public Member(
            string name,
            string typeName,
            string valueTypeName,
            bool isString,
            bool isEnum,
            bool isNullable,
            bool canWrite,
            bool isRequired,
            int order)
            : this(name, typeName, typeName, valueTypeName, isString, isEnum, isNullable, isNullable,
                ImmutableArray<string>.Empty, canWrite, true, isRequired, string.Empty, null, null, order)
        {
        }

        public Member(
            string name,
            string typeName,
            string typeIdentityName,
            string valueTypeName,
            bool isString,
            bool isEnum,
            bool isNullable,
            bool isNullableValue,
            ImmutableArray<string> enumMemberNames,
            bool canWrite,
            bool isProperty,
            bool isRequired,
            int order)
            : this(name, typeName, typeIdentityName, valueTypeName, isString, isEnum, isNullable, isNullableValue,
                enumMemberNames, canWrite, isProperty, isRequired, string.Empty, null, null, order)
        {
        }

        Member(
            string name,
            string typeName,
            string typeIdentityName,
            string valueTypeName,
            bool isString,
            bool isEnum,
            bool isNullable,
            bool isNullableValue,
            ImmutableArray<string> enumMemberNames,
            bool canWrite,
            bool isProperty,
            bool isRequired,
            string columnName,
            int? index,
            string? format,
            int order)
        {
            Name = name;
            TypeName = typeName;
            TypeIdentityName = typeIdentityName;
            ValueTypeName = valueTypeName;
            IsString = isString;
            IsEnum = isEnum;
            IsNullable = isNullable;
            IsNullableValue = isNullableValue;
            EnumMemberNames = enumMemberNames;
            CanWrite = canWrite;
            IsProperty = isProperty;
            IsRequired = isRequired;
            ColumnName = columnName;
            Index = index;
            Format = format;
            Order = order;
        }

        public string Name { get; }
        public string TypeName { get; }
        public string TypeIdentityName { get; }
        public string ValueTypeName { get; }
        public bool IsString { get; }
        public bool IsEnum { get; }
        public bool IsNullable { get; }
        public bool IsNullableValue { get; }
        public ImmutableArray<string> EnumMemberNames { get; }
        public bool CanWrite { get; }
        public bool IsProperty { get; }
        public bool IsRequired { get; }
        public string ColumnName { get; }
        public int? Index { get; }
        public string? Format { get; }
        public int Order { get; }

        public Member WithMapping(ColumnMapping mapping) =>
            new(Name, TypeName, TypeIdentityName, ValueTypeName, IsString, IsEnum, IsNullable, IsNullableValue,
                EnumMemberNames, CanWrite, IsProperty, IsRequired, mapping.Name, mapping.Index, mapping.Format, Order);

        public bool Equals(Member? other) =>
            other is not null &&
            string.Equals(Name, other.Name, StringComparison.Ordinal) &&
            string.Equals(TypeName, other.TypeName, StringComparison.Ordinal) &&
            string.Equals(TypeIdentityName, other.TypeIdentityName, StringComparison.Ordinal) &&
            string.Equals(ValueTypeName, other.ValueTypeName, StringComparison.Ordinal) &&
            IsString == other.IsString &&
            IsEnum == other.IsEnum &&
            IsNullable == other.IsNullable &&
            IsNullableValue == other.IsNullableValue &&
            EnumMemberNames.SequenceEqual(other.EnumMemberNames, StringComparer.Ordinal) &&
            CanWrite == other.CanWrite &&
            IsProperty == other.IsProperty &&
            IsRequired == other.IsRequired &&
            string.Equals(ColumnName, other.ColumnName, StringComparison.Ordinal) &&
            Index == other.Index &&
            string.Equals(Format, other.Format, StringComparison.Ordinal) &&
            Order == other.Order;

        public override bool Equals(object? obj) => Equals(obj as Member);

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Name, StringComparer.Ordinal);
            hashCode.Add(TypeName, StringComparer.Ordinal);
            hashCode.Add(TypeIdentityName, StringComparer.Ordinal);
            hashCode.Add(ValueTypeName, StringComparer.Ordinal);
            hashCode.Add(IsString);
            hashCode.Add(IsEnum);
            hashCode.Add(IsNullable);
            hashCode.Add(IsNullableValue);
            foreach (var enumMemberName in EnumMemberNames)
            {
                hashCode.Add(enumMemberName, StringComparer.Ordinal);
            }
            hashCode.Add(CanWrite);
            hashCode.Add(IsProperty);
            hashCode.Add(IsRequired);
            hashCode.Add(ColumnName, StringComparer.Ordinal);
            hashCode.Add(Index);
            hashCode.Add(Format, StringComparer.Ordinal);
            hashCode.Add(Order);
            return hashCode.ToHashCode();
        }
    }

    internal sealed class ConstructionPlan : IEquatable<ConstructionPlan>
    {
        public static readonly ConstructionPlan Empty = new(
            ImmutableArray<ConstructorParameter>.Empty,
            ImmutableArray<int>.Empty);

        public ConstructionPlan(ImmutableArray<ConstructorParameter> parameters, ImmutableArray<int> initializers)
        {
            Parameters = parameters;
            Initializers = initializers;
        }

        public ImmutableArray<ConstructorParameter> Parameters { get; }
        public ImmutableArray<int> Initializers { get; }

        public bool Equals(ConstructionPlan? other)
        {
            if (other is null || Parameters.Length != other.Parameters.Length || Initializers.Length != other.Initializers.Length)
            {
                return false;
            }
            for (var index = 0; index < Parameters.Length; ++index)
            {
                if (!Parameters[index].Equals(other.Parameters[index]))
                {
                    return false;
                }
            }
            for (var index = 0; index < Initializers.Length; ++index)
            {
                if (Initializers[index] != other.Initializers[index])
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object? obj) => Equals(obj as ConstructionPlan);

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            foreach (var parameter in Parameters)
            {
                hashCode.Add(parameter);
            }
            foreach (var initializer in Initializers)
            {
                hashCode.Add(initializer);
            }
            return hashCode.ToHashCode();
        }
    }

    internal readonly struct ConstructorParameter : IEquatable<ConstructorParameter>
    {
        public ConstructorParameter(string name, int memberIndex)
        {
            Name = name;
            MemberIndex = memberIndex;
        }

        public string Name { get; }
        public int MemberIndex { get; }

        public bool Equals(ConstructorParameter other) =>
            string.Equals(Name, other.Name, StringComparison.Ordinal) && MemberIndex == other.MemberIndex;

        public override bool Equals(object? obj) => obj is ConstructorParameter other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Name, MemberIndex);
    }
}

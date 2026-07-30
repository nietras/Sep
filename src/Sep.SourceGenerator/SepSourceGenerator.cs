using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace nietras.SeparatedValues.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class SepSourceGenerator : IIncrementalGenerator
{
    const string AttributeMetadataName = "nietras.SeparatedValues.SepSourceGenerationAttribute";
    const string ColumnAttributeMetadataName = "nietras.SeparatedValues.SepColAttribute";

    static readonly DiagnosticDescriptor s_invalidAdapter = CreateDescriptor(
        "SEPGEN001", "Invalid Sep source-generation adapter",
        "Adapter '{0}' must be a non-generic, non-file-local, top-level static partial class");
    static readonly DiagnosticDescriptor s_invalidModel = CreateDescriptor(
        "SEPGEN002", "Invalid Sep source-generation model",
        "Model '{0}' must be a non-abstract class or struct");
    static readonly DiagnosticDescriptor s_unsupportedMember = CreateDescriptor(
        "SEPGEN003", "Unsupported Sep source-generation member",
        "Member '{0}' {1}");
    static readonly DiagnosticDescriptor s_noMembers = CreateDescriptor(
        "SEPGEN004", "No Sep source-generation members",
        "Model '{0}' has no public instance fields or properties");
    static readonly DiagnosticDescriptor s_invalidColumn = CreateDescriptor(
        "SEPGEN005", "Invalid Sep column mapping",
        "Member '{0}' has an invalid Sep column mapping: {1}");
    static readonly DiagnosticDescriptor s_duplicateColumn = CreateDescriptor(
        "SEPGEN006", "Duplicate Sep column mapping",
        "Column {0} is mapped by both '{1}' and '{2}'");
    static readonly DiagnosticDescriptor s_indexLayout = CreateDescriptor(
        "SEPGEN007", "Invalid Sep physical column layout",
        "Model '{0}' has an invalid indexed layout: {1}");
    static readonly DiagnosticDescriptor s_noUsableConstructor = CreateDescriptor(
        "SEPGEN008", "No usable Sep source-generation constructor",
        "Model '{0}' has no accessible construction plan");
    static readonly DiagnosticDescriptor s_ambiguousConstructor = CreateDescriptor(
        "SEPGEN009", "Ambiguous Sep source-generation constructor",
        "Model '{0}' has multiple equally suitable accessible constructors");
    static readonly DiagnosticDescriptor s_unbindableConstructor = CreateDescriptor(
        "SEPGEN010", "Unbindable Sep source-generation constructor",
        "Model '{0}' cannot bind '{1}' to an accessible construction plan");
    static readonly DiagnosticDescriptor s_genericModel = CreateDescriptor(
        "SEPGEN011", "Unsupported generic Sep source-generation model",
        "Model '{0}' is generic; generic models are not supported");

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

    static DiagnosticDescriptor CreateDescriptor(string id, string title, string message) =>
        new(id, title, message, "Usage", DiagnosticSeverity.Error, isEnabledByDefault: true);

    static Model? CreateModel(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var adapter = (INamedTypeSymbol)context.TargetSymbol;
        if (!IsValidAdapter(adapter))
        {
            return Model.Invalid(Issue.Create("SEPGEN001", adapter.Locations.FirstOrDefault(), adapter.Name));
        }

        var model = context.Attributes[0].ConstructorArguments[0].Value as INamedTypeSymbol;
        if (model is null)
        {
            return Model.Invalid(Issue.Create("SEPGEN002", adapter.Locations.FirstOrDefault(), "<unknown>"));
        }
        if (IsGenericModel(model))
        {
            return Model.Invalid(Issue.Create("SEPGEN011", model.Locations.FirstOrDefault(), model.Name));
        }
        if (!IsValidModel(model))
        {
            return Model.Invalid(Issue.Create("SEPGEN002", model.Locations.FirstOrDefault(), model.Name));
        }

        var symbols = GetPublicInstanceMembers(model, cancellationToken);
        if (symbols.Length == 0)
        {
            return Model.Invalid(Issue.Create("SEPGEN004", model.Locations.FirstOrDefault(), model.Name));
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
                return Model.Invalid(Issue.Create("SEPGEN003", property.Locations.FirstOrDefault(), property.Name,
                    "is an indexer, which cannot be mapped"));
            }
            if (!TryCreateMember(symbol, order, out var member, out var error))
            {
                return Model.Invalid(Issue.Create("SEPGEN003", symbol.Locations.FirstOrDefault(), symbol.Name, error));
            }
            if (!TryGetColumnMapping(symbol, out var mapping, out error))
            {
                return Model.Invalid(Issue.Create("SEPGEN005", symbol.Locations.FirstOrDefault(), symbol.Name, error));
            }
            if (names.TryGetValue(mapping.Name, out var existingName))
            {
                return Model.Invalid(Issue.Create("SEPGEN006", symbol.Locations.FirstOrDefault(),
                    $"name '{mapping.Name}'", existingName, symbol.Name));
            }
            names.Add(mapping.Name, symbol.Name);
            if (mapping.Index is int index)
            {
                if (indices.TryGetValue(index, out var existingIndex))
                {
                    return Model.Invalid(Issue.Create("SEPGEN006", symbol.Locations.FirstOrDefault(),
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
            return Model.Invalid(Issue.Create("SEPGEN007", model.Locations.FirstOrDefault(), model.Name,
                "all mapped members must specify contiguous indexes starting at zero"));
        }

        if (!TryCreateConstructionPlan(model, immutableMembers, cancellationToken, out var construction, out var issue))
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
        out Member member,
        out string error)
    {
        var type = symbol is IPropertySymbol property ? property.Type : ((IFieldSymbol)symbol).Type;
        var canRead = symbol is IPropertySymbol readableProperty
            ? IsAccessible(readableProperty.GetMethod)
            : true;
        var canWrite = symbol is IPropertySymbol writableProperty
            ? IsAccessible(writableProperty.SetMethod)
            : !((IFieldSymbol)symbol).IsReadOnly;
        if (!canRead && !canWrite)
        {
            member = default!;
            error = "does not have an accessible getter, setter, or assignable field";
            return false;
        }

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
            canRead,
            canWrite,
            IsRequired(symbol),
            order);
        error = string.Empty;
        return true;
    }

    static bool IsAccessible(IMethodSymbol? method) =>
        method is not null &&
        method.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal or Accessibility.ProtectedOrInternal;

    static bool IsRequired(ISymbol symbol) =>
        symbol.GetAttributes().Any(static attribute =>
            attribute.AttributeClass?.ToDisplayString() == "System.Runtime.CompilerServices.RequiredMemberAttribute") ||
        symbol.DeclaringSyntaxReferences.Any(static syntaxReference =>
            syntaxReference.GetSyntax() switch
            {
                PropertyDeclarationSyntax property => property.Modifiers.Any(static modifier =>
                    modifier.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.RequiredKeyword)),
                VariableDeclaratorSyntax variable when variable.Parent?.Parent is FieldDeclarationSyntax field =>
                    field.Modifiers.Any(static modifier =>
                        modifier.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.RequiredKeyword)),
                _ => false,
            });

    static bool SupportsSpanConversion(ITypeSymbol type) =>
    type.AllInterfaces.Any(@interface =>
        @interface.OriginalDefinition.MetadataName == "ISpanParsable`1" &&
        @interface.OriginalDefinition.ContainingNamespace.ToDisplayString() == "System" &&
        @interface.TypeArguments.Length == 1 &&
        SymbolEqualityComparer.Default.Equals(@interface.TypeArguments[0], type)) &&
    type.AllInterfaces.Any(@interface =>
        @interface.MetadataName == "ISpanFormattable" &&
        @interface.ContainingNamespace.ToDisplayString() == "System");

    static bool TryGetColumnMapping(ISymbol symbol, out ColumnMapping mapping, out string error)
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
        CancellationToken cancellationToken,
        out ConstructionPlan? plan,
        out Issue? issue)
    {
        if (model.TypeKind == TypeKind.Struct && members.All(static member => member.CanWrite))
        {
            plan = ConstructionPlan.Empty;
            issue = null;
            return true;
        }

        var accessibleConstructors = model.InstanceConstructors
            .Where(static constructor => constructor.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal)
            .ToImmutableArray();
        if (accessibleConstructors.Length == 0)
        {
            plan = null;
            issue = Issue.Create("SEPGEN008", model.Locations.FirstOrDefault(), model.Name);
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
            issue = unassignable is not null
                ? Issue.Create("SEPGEN010", model.Locations.FirstOrDefault(), model.Name, unassignable.Name)
                : Issue.Create("SEPGEN010", model.Locations.FirstOrDefault(), model.Name, "constructor parameters");
            return false;
        }

        var bestBoundCount = candidates.Max(static candidate => candidate.Parameters.Length);
        var best = candidates.Where(candidate => candidate.Parameters.Length == bestBoundCount).ToArray();
        if (best.Length != 1)
        {
            plan = null;
            issue = Issue.Create("SEPGEN009", model.Locations.FirstOrDefault(), model.Name);
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

    static bool IsValidAdapter(INamedTypeSymbol adapter) =>
        adapter.IsStatic &&
        adapter.Arity == 0 &&
        adapter.ContainingType is null &&
        adapter.DeclaringSyntaxReferences.Length == 1 &&
        adapter.DeclaringSyntaxReferences[0].GetSyntax() is ClassDeclarationSyntax declaration &&
        !declaration.Modifiers.Any(static modifier => modifier.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.FileKeyword)) &&
        declaration.Modifiers.Any(static modifier => modifier.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword));

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
        EmitRead(source, model);
        source.AppendLine();
        EmitTryRead(source, model);
        source.AppendLine();
        EmitReadEnumerable(source, model);
        source.AppendLine();
        EmitReadAsyncEnumerable(source, model);
        source.AppendLine();
        EmitWrite(source, model);
        source.AppendLine();
        EmitWriteEnumerable(source, model);
        source.AppendLine("}");

        context.AddSource(HintName(model), SourceText.From(source.ToString(), Encoding.UTF8));
    }

    internal static Diagnostic CreateDiagnostic(Issue issue)
    {
        var descriptor = issue.Id switch
        {
            "SEPGEN001" => s_invalidAdapter,
            "SEPGEN002" => s_invalidModel,
            "SEPGEN003" => s_unsupportedMember,
            "SEPGEN004" => s_noMembers,
            "SEPGEN005" => s_invalidColumn,
            "SEPGEN006" => s_duplicateColumn,
            "SEPGEN007" => s_indexLayout,
            "SEPGEN008" => s_noUsableConstructor,
            "SEPGEN009" => s_ambiguousConstructor,
            "SEPGEN010" => s_unbindableConstructor,
            "SEPGEN011" => s_genericModel,
            _ => throw new InvalidOperationException(),
        };
        return Diagnostic.Create(descriptor, issue.Location, issue.Arguments.Cast<object>().ToArray());
    }

    static void EmitRead(StringBuilder source, Model model)
    {
        source.Append("    public static ").Append(model.ModelName)
            .Append(" Read(global::nietras.SeparatedValues.SepReader.Row row) => ");
        AppendConstruction(source, model, memberIndex => AppendReadExpression(source, model, model.Members[memberIndex]));
        source.AppendLine(";");
    }

    static void EmitTryRead(StringBuilder source, Model model)
    {
        source.Append("    public static bool TryRead(global::nietras.SeparatedValues.SepReader.Row row, out ")
            .Append(model.ModelName).AppendLine(" value)");
        source.AppendLine("    {");
        for (var memberIndex = 0; memberIndex < model.Members.Length; ++memberIndex)
        {
            EmitTryReadMember(source, model, model.Members[memberIndex], memberIndex);
        }
        source.AppendLine();
        source.Append("        value = ");
        AppendConstruction(source, model, memberIndex => source.Append("__sep").Append(memberIndex));
        source.AppendLine(";");
        source.AppendLine("        return true;");
        source.AppendLine("    }");
    }

    static void EmitTryReadMember(StringBuilder source, Model model, Member member, int memberIndex)
    {
        var local = "__sep" + memberIndex;
        if (member.IsString)
        {
            source.Append("        var ").Append(local).Append(" = ");
            if (member.IsNullable)
            {
                AppendReadColumn(source, model, member);
                source.Append(".Span.IsEmpty ? null : ");
            }
            AppendReadColumn(source, model, member);
            source.AppendLine(".ToString();");
            return;
        }

        if (member.IsNullable)
        {
            source.Append("        ").Append(member.TypeName);
            if (!member.IsNullableValue)
            {
                source.Append('?');
            }
            source.Append(' ').Append(local).AppendLine(";");
            source.Append("        if (");
            AppendReadColumn(source, model, member);
            source.AppendLine(".Span.IsEmpty)");
            source.AppendLine("        {");
            source.Append("            ").Append(local).AppendLine(" = null;");
            source.AppendLine("        }");
            source.AppendLine("        else");
            source.AppendLine("        {");
            EmitTryParse(source, model, member, local + "Value", "            ");
            source.Append("            ").Append(local).Append(" = ").Append(local).AppendLine("Value;");
            source.AppendLine("        }");
            return;
        }

        EmitTryParse(source, model, member, local, "        ");
    }

    static void EmitTryParse(StringBuilder source, Model model, Member member, string local, string indent)
    {
        source.Append(indent).Append("if (!");
        if (member.IsEnum)
        {
            source.Append("global::System.Enum.TryParse<").Append(member.ValueTypeName).Append(">(");
            AppendReadColumn(source, model, member);
            source.Append(".Span, out var ").Append(local).AppendLine("))");
        }
        else
        {
            AppendReadColumn(source, model, member);
            source.Append(".TryParse<").Append(member.ValueTypeName).Append(">(out var ").Append(local).AppendLine("))");
        }
        source.Append(indent).AppendLine("{");
        source.Append(indent).AppendLine("    value = default!;");
        source.Append(indent).AppendLine("    return false;");
        source.Append(indent).AppendLine("}");
    }

    static void EmitReadEnumerable(StringBuilder source, Model model)
    {
        source.Append("    public static global::System.Collections.Generic.IEnumerable<")
            .Append(model.ModelName).Append("> Read(global::nietras.SeparatedValues.SepReader reader) => ")
            .Append("global::nietras.SeparatedValues.SepReaderExtensions.Enumerate(reader, Read);").AppendLine();
    }

    static void EmitReadAsyncEnumerable(StringBuilder source, Model model)
    {
        source.Append("    public static global::System.Collections.Generic.IAsyncEnumerable<")
            .Append(model.ModelName).Append("> ReadAsync(global::nietras.SeparatedValues.SepReader reader) => ")
            .Append("global::nietras.SeparatedValues.SepReaderExtensions.EnumerateAsync(reader, Read);").AppendLine();
    }

    static void EmitWrite(StringBuilder source, Model model)
    {
        source.Append("    public static void Write(global::nietras.SeparatedValues.SepWriter.Row row, ")
            .Append(model.ModelName).AppendLine(" value)");
        source.AppendLine("    {");
        foreach (var member in model.Members
            .Where(static member => member.CanRead)
            .OrderBy(member => model.UsesIndexes ? member.Index : member.Order)
            .ThenBy(static member => member.Order))
        {
            EmitWriteMember(source, member);
        }
        source.AppendLine("    }");
    }

    static void EmitWriteMember(StringBuilder source, Member member)
    {
        var value = "value.@" + member.Name;
        if (member.IsString)
        {
            source.Append("        ");
            AppendWriteColumn(source, member);
            source.Append(".Set(global::System.MemoryExtensions.AsSpan(").Append(value).AppendLine("));");
            return;
        }

        if (member.IsNullable)
        {
            if (!member.IsNullableValue)
            {
                source.Append("        if (").Append(value).AppendLine(" is null)");
                source.AppendLine("        {");
                source.Append("            ");
                AppendWriteColumn(source, member);
                source.AppendLine(".Set(global::System.ReadOnlySpan<char>.Empty);");
                source.AppendLine("        }");
                source.AppendLine("        else");
                source.AppendLine("        {");
                source.Append("            ");
                AppendWriteColumn(source, member);
                source.Append(".Format(").Append(value);
                if (member.Format is not null)
                {
                    source.Append(", ");
                    AppendStringLiteral(source, member.Format);
                }
                source.AppendLine(");");
                source.AppendLine("        }");
                return;
            }

            source.Append("        if (").Append(value).AppendLine(".HasValue)");
            source.AppendLine("        {");
            source.Append("            ");
            AppendWriteColumn(source, member);
            if (member.IsEnum)
            {
                source.Append(".Set(").Append(value).Append(".Value.ToString(");
                AppendStringLiteral(source, member.Format ?? "G");
                source.AppendLine("));");
            }
            else
            {
                source.Append(".Format(").Append(value).Append(".Value");
                if (member.Format is not null)
                {
                    source.Append(", ");
                    AppendStringLiteral(source, member.Format);
                }
                source.AppendLine(");");
            }
            source.AppendLine("        }");
            source.AppendLine("        else");
            source.AppendLine("        {");
            source.Append("            ");
            AppendWriteColumn(source, member);
            source.AppendLine(".Set(global::System.ReadOnlySpan<char>.Empty);");
            source.AppendLine("        }");
            return;
        }

        if (member.IsEnum)
        {
            source.Append("        ");
            AppendWriteColumn(source, member);
            source.Append(".Set(").Append(value).Append(".ToString(");
            AppendStringLiteral(source, member.Format ?? "G");
            source.AppendLine("));");
            return;
        }

        source.Append("        ");
        AppendWriteColumn(source, member);
        source.Append(".Format(").Append(value);
        if (member.Format is not null)
        {
            source.Append(", ");
            AppendStringLiteral(source, member.Format);
        }
        source.AppendLine(");");
    }

    static void AppendWriteColumn(StringBuilder source, Member member)
    {
        source.Append("row[");
        AppendStringLiteral(source, member.ColumnName);
        source.Append(']');
    }

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

    static void AppendReadExpression(StringBuilder source, Model model, Member member)
    {
        if (member.IsNullable)
        {
            AppendReadColumn(source, model, member);
            source.Append(".Span.IsEmpty ? null : ");
        }
        if (member.IsString)
        {
            AppendReadColumn(source, model, member);
            source.Append(".ToString()");
        }
        else if (member.IsEnum)
        {
            source.Append("global::System.Enum.Parse<").Append(member.ValueTypeName).Append(">(");
            AppendReadColumn(source, model, member);
            source.Append(".Span)");
        }
        else
        {
            AppendReadColumn(source, model, member);
            source.Append(".Parse<").Append(member.ValueTypeName).Append(">()");
        }
    }

    static void AppendReadColumn(StringBuilder source, Model model, Member member)
    {
        source.Append("row[");
        if (model.UsesIndexes)
        {
            source.Append(member.Index!.Value);
        }
        else
        {
            AppendStringLiteral(source, member.ColumnName);
        }
        source.Append(']');
    }

    static void AppendStringLiteral(StringBuilder source, string value) =>
        source.Append(Microsoft.CodeAnalysis.CSharp.SymbolDisplay.FormatLiteral(value, quote: true));

    static void EmitWriteEnumerable(StringBuilder source, Model model)
    {
        source.Append("    public static void Write(global::nietras.SeparatedValues.SepWriter writer, global::System.Collections.Generic.IEnumerable<")
            .Append(model.ModelName).AppendLine("> values)");
        source.AppendLine("    {");
        source.AppendLine("        global::System.ArgumentNullException.ThrowIfNull(writer);");
        source.AppendLine("        global::System.ArgumentNullException.ThrowIfNull(values);");
        source.AppendLine("        foreach (var value in values)");
        source.AppendLine("        {");
        source.AppendLine("            using var row = writer.NewRow();");
        source.AppendLine("            Write(row, value);");
        source.AppendLine("        }");
        source.AppendLine("    }");
    }

    static string HintName(Model model) =>
        "Sep_" + HashHintIdentity(model.AdapterIdentity + "\0" + model.ModelName) + ".Sep.g.cs";

    static string HashHintIdentity(string value)
    {
        using var hashAlgorithm = SHA256.Create();
        var hash = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
        var builder = new StringBuilder(hash.Length * 2);
        foreach (var valueByte in hash)
        {
            builder.Append(valueByte.ToString("x2", System.Globalization.CultureInfo.InvariantCulture));
        }
        return builder.ToString();
    }

    const string AttributeSource = """
        // <auto-generated/>
        #nullable enable
        namespace nietras.SeparatedValues;

        [global::System.AttributeUsage(global::System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
        [global::System.Diagnostics.Conditional("SEP_SOURCE_GENERATOR")]
        public sealed class SepSourceGenerationAttribute : global::System.Attribute
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
        public sealed class SepColAttribute : global::System.Attribute
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
            public int Index { get; set; } = -1;
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
            var hashCode = StringComparer.Ordinal.GetHashCode(Namespace ?? string.Empty);
            hashCode = (hashCode * 397) ^ StringComparer.Ordinal.GetHashCode(Accessibility);
            hashCode = (hashCode * 397) ^ StringComparer.Ordinal.GetHashCode(AdapterName);
            hashCode = (hashCode * 397) ^ StringComparer.Ordinal.GetHashCode(AdapterIdentity);
            hashCode = (hashCode * 397) ^ StringComparer.Ordinal.GetHashCode(ModelName);
            hashCode = (hashCode * 397) ^ UsesIndexes.GetHashCode();
            hashCode = (hashCode * 397) ^ (Issue?.GetHashCode() ?? 0);
            hashCode = (hashCode * 397) ^ Construction.GetHashCode();
            foreach (var member in Members)
            {
                hashCode = (hashCode * 397) ^ member.GetHashCode();
            }
            return hashCode;
        }
    }

    internal sealed class Issue : IEquatable<Issue>
    {
        Issue(string id, Location location, ImmutableArray<string> arguments)
        {
            Id = id;
            Location = location;
            Arguments = arguments;
        }

        public string Id { get; }
        public Location Location { get; }
        public ImmutableArray<string> Arguments { get; }

        public static Issue Create(string id, Location? location, params string[] arguments) =>
            new(id, location ?? Location.None, arguments.ToImmutableArray());

        public bool Equals(Issue? other)
        {
            if (other is null ||
                !string.Equals(Id, other.Id, StringComparison.Ordinal) ||
                !string.Equals(Location.SourceTree?.FilePath, other.Location.SourceTree?.FilePath, StringComparison.Ordinal) ||
                !Location.SourceSpan.Equals(other.Location.SourceSpan) ||
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
            var hashCode = StringComparer.Ordinal.GetHashCode(Id);
            hashCode = (hashCode * 397) ^ StringComparer.Ordinal.GetHashCode(Location.SourceTree?.FilePath ?? string.Empty);
            hashCode = (hashCode * 397) ^ Location.SourceSpan.GetHashCode();
            foreach (var argument in Arguments)
            {
                hashCode = (hashCode * 397) ^ StringComparer.Ordinal.GetHashCode(argument);
            }
            return hashCode;
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
            bool canRead,
            bool canWrite,
            bool isRequired,
            int order)
            : this(name, typeName, typeName, valueTypeName, isString, isEnum, isNullable, isNullable, canRead, canWrite,
                isRequired, string.Empty, null, null, order)
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
            bool canRead,
            bool canWrite,
            bool isRequired,
            int order)
            : this(name, typeName, typeIdentityName, valueTypeName, isString, isEnum, isNullable, isNullableValue, canRead, canWrite,
                isRequired, string.Empty, null, null, order)
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
            bool canRead,
            bool canWrite,
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
            CanRead = canRead;
            CanWrite = canWrite;
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
        public bool CanRead { get; }
        public bool CanWrite { get; }
        public bool IsRequired { get; }
        public string ColumnName { get; }
        public int? Index { get; }
        public string? Format { get; }
        public int Order { get; }

        public Member WithMapping(ColumnMapping mapping) =>
            new(Name, TypeName, TypeIdentityName, ValueTypeName, IsString, IsEnum, IsNullable, IsNullableValue, CanRead, CanWrite, IsRequired,
                mapping.Name, mapping.Index, mapping.Format, Order);

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
            CanRead == other.CanRead &&
            CanWrite == other.CanWrite &&
            IsRequired == other.IsRequired &&
            string.Equals(ColumnName, other.ColumnName, StringComparison.Ordinal) &&
            Index == other.Index &&
            string.Equals(Format, other.Format, StringComparison.Ordinal) &&
            Order == other.Order;

        public override bool Equals(object? obj) => Equals(obj as Member);

        public override int GetHashCode()
        {
            var hashCode = StringComparer.Ordinal.GetHashCode(Name);
            hashCode = (hashCode * 397) ^ StringComparer.Ordinal.GetHashCode(TypeName);
            hashCode = (hashCode * 397) ^ StringComparer.Ordinal.GetHashCode(TypeIdentityName);
            hashCode = (hashCode * 397) ^ StringComparer.Ordinal.GetHashCode(ValueTypeName);
            hashCode = (hashCode * 397) ^ IsString.GetHashCode();
            hashCode = (hashCode * 397) ^ IsEnum.GetHashCode();
            hashCode = (hashCode * 397) ^ IsNullable.GetHashCode();
            hashCode = (hashCode * 397) ^ IsNullableValue.GetHashCode();
            hashCode = (hashCode * 397) ^ CanRead.GetHashCode();
            hashCode = (hashCode * 397) ^ CanWrite.GetHashCode();
            hashCode = (hashCode * 397) ^ IsRequired.GetHashCode();
            hashCode = (hashCode * 397) ^ StringComparer.Ordinal.GetHashCode(ColumnName);
            hashCode = (hashCode * 397) ^ Index.GetHashCode();
            hashCode = (hashCode * 397) ^ StringComparer.Ordinal.GetHashCode(Format ?? string.Empty);
            return (hashCode * 397) ^ Order;
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
            var hashCode = 0;
            foreach (var parameter in Parameters)
            {
                hashCode = (hashCode * 397) ^ parameter.GetHashCode();
            }
            foreach (var initializer in Initializers)
            {
                hashCode = (hashCode * 397) ^ initializer;
            }
            return hashCode;
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

        public override int GetHashCode() => (StringComparer.Ordinal.GetHashCode(Name) * 397) ^ MemberIndex;
    }
}

using System;
using System.Threading;
using H.Generators.Extensions;
using Kava.Generators.Extensions;

namespace Kava.Generators.Sources;

[Generator]
public sealed class PropertyGenerator : IIncrementalGenerator
{
    private const string Id = "PG";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context
            .SyntaxProvider.CreateSyntaxProvider(
                IsSyntaxTarget,
                (syntaxContext, _) => syntaxContext
            )
            .Combine(context.CompilationProvider)
            .SelectAndReportExceptions(tuple => Generate(tuple.Left, tuple.Right), context, Id)
            .AddSource(context);
    }

    private static bool IsSyntaxTarget(SyntaxNode node, CancellationToken ct)
    {
        if (node is BaseTypeDeclarationSyntax baseTypeDeclarationSyntax)
        {
            return baseTypeDeclarationSyntax.Identifier.Text.EndsWith("ViewModel");
        }

        return false;
    }

    private static FileWithName Generate(GeneratorSyntaxContext context, Compilation compilation)
    {
        var viewModelSymbol = context.SemanticModel.GetDeclaredSymbol(context.Node);

        if (viewModelSymbol is null)
        {
            return FileWithName.Empty;
        }

        var viewSymbol = GetView(compilation, viewModelSymbol);
        if (viewSymbol is null)
            return FileWithName.Empty;

        var source = new SourceStringBuilder(viewSymbol);

        source.Line("#nullable enable");
        source.PartialTypeBlockBrace(() =>
        {
            source.Line($"public {viewModelSymbol.ToFullDisplayString()} ViewModel");
            source.BlockBrace(() =>
            {
                source.Line(
                    $"get => DataContext as {viewModelSymbol.ToFullDisplayString()} ?? throw new global::System.ArgumentNullException(nameof(DataContext));"
                );
                source.Line("set => DataContext = value;");
            });
        });

        return new FileWithName($"{viewSymbol.ToDisplayString()}.Property.g.cs", source.ToString());
    }

    private static INamedTypeSymbol? GetView(Compilation compilation, ISymbol? symbol)
    {
        if (symbol is null)
        {
            return null;
        }

        var viewName = symbol.ToDisplayString().Replace("ViewModel", "View");
        var viewSymbol = compilation.GetTypeByMetadataName(viewName);

        if (viewSymbol is not null)
            return viewSymbol;

        viewName = symbol.ToDisplayString().Replace(".ViewModels.", ".Views.");
        viewName = viewName.Remove(viewName.IndexOf("ViewModel", StringComparison.Ordinal));
        return compilation.GetTypeByMetadataName(viewName);
    }
}

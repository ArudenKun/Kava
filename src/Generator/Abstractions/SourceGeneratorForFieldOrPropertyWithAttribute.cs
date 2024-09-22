using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Generator.Abstractions;

public abstract class SourceGeneratorForFieldOrPropertyWithAttribute<TAttribute>
    : SourceGeneratorForMemberWithAttribute<TAttribute, SyntaxNode>
    where TAttribute : Attribute
{
    protected virtual string GenerateCode(
        Compilation compilation,
        SyntaxNode node,
        VariableSymbol symbol,
        TAttribute attribute,
        AnalyzerConfigOptions options
    ) => string.Empty;

    protected virtual string GenerateCode(
        Compilation compilation,
        SyntaxNode node,
        VariableSymbol symbol,
        ImmutableArray<TAttribute> attributes,
        AnalyzerConfigOptions options
    ) => string.Empty;

    protected sealed override string GenerateCode(
        Compilation compilation,
        SyntaxNode node,
        ISymbol symbol,
        TAttribute attribute,
        AnalyzerConfigOptions options
    )
    {
        var (castedNode, variableSymbol) = Process(node, symbol);
        return GenerateCode(compilation, castedNode, variableSymbol, attribute, options);
    }

    protected sealed override string GenerateCode(
        Compilation compilation,
        SyntaxNode node,
        ISymbol symbol,
        ImmutableArray<TAttribute> attributes,
        AnalyzerConfigOptions options
    )
    {
        var (castedNode, variableSymbol) = Process(node, symbol);
        return GenerateCode(compilation, castedNode, variableSymbol, attributes, options);
    }

    private static (SyntaxNode, VariableSymbol) Process(SyntaxNode node, ISymbol symbol)
    {
        node = node switch
        {
            VariableDeclaratorSyntax
            {
                Parent: VariableDeclarationSyntax
                {
                    Parent: FieldDeclarationSyntax fieldDeclarationSyntax
                }
            } => fieldDeclarationSyntax,
            PropertyDeclarationSyntax propertyDeclarationSyntax => propertyDeclarationSyntax,
            _ => throw new InvalidCastException(
                $"Unexpected syntax node type: {node.GetType().FullName}"
            ),
        };

        ITypeSymbol typeSymbol;
        ISymbol selfTypeSymbol;
        switch (symbol)
        {
            case IFieldSymbol fieldSymbol:
                typeSymbol = fieldSymbol.Type;
                selfTypeSymbol = fieldSymbol;
                break;
            case IPropertySymbol propertySymbol:
                typeSymbol = propertySymbol.Type;
                selfTypeSymbol = propertySymbol;
                break;
            default:
                throw new InvalidCastException(
                    $"Unexpected symbol type: {symbol.GetType().FullName}"
                );
        }

        return (
            node,
            new VariableSymbol(symbol.Name, typeSymbol, symbol.ContainingType, selfTypeSymbol)
        );
    }

    protected readonly record struct VariableSymbol(
        string Name,
        ITypeSymbol Type,
        INamedTypeSymbol ContainingType,
        ISymbol SelfType
    );

    protected sealed override bool IsSyntaxTarget(SyntaxNode node, CancellationToken _) =>
        node
            is VariableDeclaratorSyntax
                {
                    Parent: VariableDeclarationSyntax
                    {
                        Parent: FieldDeclarationSyntax { AttributeLists.Count: > 0 }
                    }
                }
                or PropertyDeclarationSyntax { AttributeLists.Count: > 0 };
}

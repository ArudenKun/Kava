﻿using System;
using System.Threading;
using Kava.Generators.Models;
using Microsoft.CodeAnalysis.Diagnostics;
using OneOf;

namespace Kava.Generators.Abstractions;

public abstract class SourceGeneratorForFieldOrPropertyWithAttribute<TAttribute>
    : SourceGeneratorForMemberWithAttribute<TAttribute, SyntaxNode>
    where TAttribute : Attribute
{
    protected virtual string GenerateCode(
        Compilation compilation,
        OneOf<PropertyDeclarationSyntax, FieldDeclarationSyntax> node,
        PropertyOrFieldSymbol symbol,
        TAttribute attribute,
        AnalyzerConfigOptions options
    ) => string.Empty;

    protected virtual string GenerateCode(
        Compilation compilation,
        OneOf<PropertyDeclarationSyntax, FieldDeclarationSyntax> node,
        PropertyOrFieldSymbol symbol,
        ImmutableArray<TAttribute> attributes,
        AnalyzerConfigOptions options
    ) => string.Empty;

    protected sealed override string GenerateCode(
        Compilation compilation,
        SyntaxNode node,
        ISymbol symbol,
        TAttribute attribute,
        AnalyzerConfigOptions options
    ) => GenerateCode(compilation, ProcessNode(node), ProcessSymbol(symbol), attribute, options);

    protected sealed override string GenerateCode(
        Compilation compilation,
        SyntaxNode node,
        ISymbol symbol,
        ImmutableArray<TAttribute> attributes,
        AnalyzerConfigOptions options
    ) => GenerateCode(compilation, ProcessNode(node), ProcessSymbol(symbol), attributes, options);

    private static OneOf<PropertyDeclarationSyntax, FieldDeclarationSyntax> ProcessNode(
        SyntaxNode node
    ) =>
        node switch
        {
            PropertyDeclarationSyntax propertyDeclarationSyntax => propertyDeclarationSyntax,
            VariableDeclaratorSyntax
            {
                Parent: VariableDeclarationSyntax
                {
                    Parent: FieldDeclarationSyntax fieldDeclarationSyntax
                }
            } => fieldDeclarationSyntax,
            _ => throw new InvalidCastException(
                $"Unexpected syntax node type: {node.GetType().FullName}"
            ),
        };

    private static PropertyOrFieldSymbol ProcessSymbol(ISymbol symbol) =>
        symbol switch
        {
            IPropertySymbol propertySymbol => new PropertyOrFieldSymbol.Property(propertySymbol),
            IFieldSymbol fieldSymbol => new PropertyOrFieldSymbol.Field(fieldSymbol),
            _ => throw new InvalidCastException(
                $"Unexpected symbol type: {symbol.GetType().FullName}"
            ),
        };

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

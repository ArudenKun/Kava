using System;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Kava.Generators.Abstractions;

public abstract class SourceGeneratorForMethodWithAttribute<TAttribute>
    : SourceGeneratorForMemberWithAttribute<TAttribute, MethodDeclarationSyntax>
    where TAttribute : Attribute
{
    protected abstract string GenerateCode(
        Compilation compilation,
        MethodDeclarationSyntax node,
        IMethodSymbol symbol,
        TAttribute attribute,
        AnalyzerConfigOptions options
    );

    protected sealed override string GenerateCode(
        Compilation compilation,
        MethodDeclarationSyntax node,
        ISymbol symbol,
        TAttribute attribute,
        AnalyzerConfigOptions options
    ) => GenerateCode(compilation, node, (IMethodSymbol)symbol, attribute, options);
}

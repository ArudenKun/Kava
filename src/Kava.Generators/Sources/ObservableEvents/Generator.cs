// Copyright (c) 2019-2023 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace Kava.Generators.Sources.ObservableEvents;

/// <summary>
/// Roslyn 40 generator for geneating events.
/// </summary>
[Generator(LanguageNames.CSharp)]
public class Generator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource("TestExtensions.SourceGenerated.cs", ClassConstants.ExtensionMethodText)
        );

        var candidateInvocations = context.SyntaxProvider.CreateSyntaxProvider(
            (syntax, _) => syntax.IsCandidate(),
            (syntax, _) => (InvocationExpressionSyntax)syntax.Node
        );

        var inputs = candidateInvocations
            .Collect()
            .Combine(context.CompilationProvider)
            .Select((combined, _) => (Candidates: combined.Left, Compilation: combined.Right));

        context.RegisterSourceOutput(
            inputs,
            (generateContext, collectedValues) =>
                EventGenerator.Generate(
                    collectedValues.Compilation,
                    collectedValues.Candidates,
                    generateContext.AddSource,
                    generateContext.ReportDiagnostic,
                    generateContext.CancellationToken
                )
        );
    }
}

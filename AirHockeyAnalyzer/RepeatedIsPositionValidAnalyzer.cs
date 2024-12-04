using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RepeatedIsPositionValidAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        id: "ISVALID001",
        title: "Repeated Call to IsPositionValid",
        messageFormat: "Call to 'IsPositionValid' is repeated in different cases of the switch statement",
        category: "CodeSmell",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeSwitchStatement, SyntaxKind.SwitchStatement);
    }

    private void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
    {
        var switchStatement = (SwitchStatementSyntax)context.Node;

        var isPositionValidCalls = new Dictionary<string, Location>();

        foreach (var section in switchStatement.Sections)
        {
            foreach (var statement in section.Statements)
            {
                var invocation = statement.DescendantNodes().OfType<InvocationExpressionSyntax>()
                    .FirstOrDefault(expr =>
                        expr.Expression is IdentifierNameSyntax identifier &&
                        identifier.Identifier.Text == "IsPositionValid");

                if (invocation != null)
                {
                    var argumentPattern = string.Join(", ", invocation.ArgumentList.Arguments.Select(arg => arg.ToString()));

                    if (isPositionValidCalls.ContainsKey(argumentPattern))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, invocation.GetLocation()));
                    }
                    else
                    {
                        isPositionValidCalls[argumentPattern] = invocation.GetLocation();
                    }
                }
            }
        }
    }
}
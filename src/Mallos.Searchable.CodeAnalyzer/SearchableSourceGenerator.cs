using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mallos.Searchable.CodeAnalyzer
{
    [Generator]
    public class SearchableSourceGenerator : ISourceGenerator
    {
        private readonly List<string> filterNames = new List<string>();

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SearchableSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            filterNames.Clear();

            SearchableSyntaxReceiver syntaxReceiver = (SearchableSyntaxReceiver)context.SyntaxReceiver;
            Generate(context, syntaxReceiver);
        }

        private void Generate(GeneratorExecutionContext context, SearchableSyntaxReceiver syntaxReceiver)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using Mallos.Searchable;");
            sb.AppendLine("using Mallos.Searchable.Attributes;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");

            foreach (var ns in syntaxReceiver.Generators.Select(x => x.ClassNamespace).Distinct())
            {
                sb.AppendLine($"using {ns};");
            }

            foreach (var node in syntaxReceiver.Generators)
            {
                foreach (var member in node.Members)
                {
                    if (member.Member is PropertyDeclarationSyntax property)
                    {
                        GenerateClass(sb, node.TargetClassName, node.ClassName, property.Identifier.ValueText, member);
                    }

                    if (member.Member is FieldDeclarationSyntax field)
                    {
                        GenerateClass(sb, node.TargetClassName, node.ClassName, field.Declaration.Type.ToString(), member);
                    }
                }

                sb.AppendLine($@"
public class {node.TargetClassName} : Searchable<{node.ClassName}> 
{{
    public {node.TargetClassName}()
    {{
{string.Join("\n", filterNames.Select(x => $"        Filters.Add(new {x}());"))}
    }}

    protected override IEnumerable<{node.ClassName}> FreeTextFilter(
        IEnumerable<{node.ClassName}> values, bool negative, string text)
    {{
        return base.FreeTextFilter(values, negative, text);
    }}
}}");
            }

            string finalSource = sb.ToString();

            SourceText sourceText = SourceText.From(finalSource, Encoding.UTF8);

            context.AddSource("GeneratedSearchables.cs", sourceText);
        }

        private void GenerateClass(
            StringBuilder sb,
            string generatorName,
            string targetName,
            string identifierName,
            GeneratorNodeAttributeGroup member)
        {
            foreach (var attr in member.Attributes)
            {
                var keyName = attr.GetKey() ?? identifierName;
                var filterClassName = $"{generatorName}Filter_{attr.Rule.Name}_{keyName}";
                filterNames.Add(filterClassName);

                var arguments = string.Join(",\n            ", attr.Rule.ArgumentList.Arguments);
                var argumentsText = arguments.Length > 0
                    ? $",\n            {arguments}"
                    : string.Empty;

                sb.AppendLine($@"
public class {filterClassName} : SimpleFilter<{targetName}> 
{{
    public override string Key => ""{keyName}"";

    protected override bool Check({targetName} item, string value)
    {{
        return {attr.RuleType}.Execute(
            item.{identifierName},
            value{argumentsText}
        );
    }}
}}");
            }
        }
    }
}

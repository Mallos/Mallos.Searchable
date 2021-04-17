namespace Mallos.Searchable.CodeAnalyzer
{
    using Mallos.Searchable.Attributes;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    class GeneratorNodeAttribute
    {
        public AttributeSyntax Key { get; }

        public AttributeSyntax Rule { get; }

        public Type RuleType { get; }

        public GeneratorNodeAttribute(AttributeSyntax key, AttributeSyntax rule, Type ruleType)
        {
            this.Key = key;
            this.Rule = rule;
            this.RuleType = ruleType;
        }

        public string GetKey()
        {
            if (this.Rule == null)
                return null;

            return this.Key.ArgumentList.Arguments[0].ToString().Replace("\"", "");
        }
    }

    class GeneratorNodeAttributeGroup
    {
        public MemberDeclarationSyntax Member { get; }

        public List<GeneratorNodeAttribute> Attributes { get; }

        public GeneratorNodeAttributeGroup(MemberDeclarationSyntax member, List<GeneratorNodeAttribute> attributes)
        {
            this.Member = member;
            this.Attributes = attributes;
        }
    }

    class GeneratorNode
    {
        public AttributeSyntax FilterGenerator { get; }

        public TypeDeclarationSyntax ObjectSyntax { get; }

        public GeneratorNodeAttributeGroup[] Members { get; }

        public string ClassNamespace => "Mallos.Searchable.Test";

        public string ClassName => this.ObjectSyntax.Identifier.ValueText;

        public string TargetClassName { get; }

        public GeneratorNode(
            AttributeSyntax filterGenerator,
            TypeDeclarationSyntax objectSyntax,
            GeneratorNodeAttributeGroup[] members)
        {
            this.FilterGenerator = filterGenerator;
            this.ObjectSyntax = objectSyntax;
            this.Members = members;

            // Process Attribute
            var filterGeneratorAttribute = this.FilterGenerator.ArgumentList.Arguments[0];
            this.TargetClassName = filterGeneratorAttribute.ToString().Replace("\"", "");
        }
    }

    class SearchableSyntaxReceiver : ISyntaxReceiver
    {
        public List<GeneratorNode> Generators { get; private set; } = new List<GeneratorNode>();

        private readonly List<Type> filterAttributes;

        public SearchableSyntaxReceiver()
        {
            this.filterAttributes = ReflectionHelper.AllOfType<FilterBaseAttribute>().ToList();
        }

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax
             || syntaxNode is StructDeclarationSyntax
             || syntaxNode is RecordDeclarationSyntax)
            {
                var syntax = (TypeDeclarationSyntax)syntaxNode;

                var attr = GetAttribute(syntax, "FilterGenerator");
                if (attr != null)
                {
                    var members = syntax.Members
                        .Where(x => x.IsKind(SyntaxKind.PropertyDeclaration) || x.IsKind(SyntaxKind.FieldDeclaration))
                        .Select(x => new GeneratorNodeAttributeGroup(x, GetFilterAttributes(x)))
                        .ToArray();

                    Generators.Add(new GeneratorNode(attr, syntax, members));
                }
            }
        }

        private List<GeneratorNodeAttribute> GetFilterAttributes(MemberDeclarationSyntax syntax)
        {
            var attrs = new List<GeneratorNodeAttribute>();
            if (syntax.AttributeLists != null)
            {
                foreach (var list in syntax.AttributeLists)
                {
                    AttributeSyntax keyAttr = null;
                    AttributeSyntax filterAttr = null;
                    Type filterAttrType = null;
                    foreach (var attr in list.Attributes)
                    {
                        if (attr.Name.ToString().Contains("FilterKey"))
                            keyAttr = attr;

                        var foundFilterType = filterAttributes.Find(x => x.Name.Contains(attr.Name.ToString()));
                        if (foundFilterType != null)
                        {
                            filterAttr = attr;
                            filterAttrType = foundFilterType;
                        }
                    }
                    attrs.Add(new GeneratorNodeAttribute(keyAttr, filterAttr, filterAttrType));
                }
            }
            return attrs;
        }

        private static AttributeSyntax GetAttribute(TypeDeclarationSyntax syntax, string name)
        {
            if (syntax.AttributeLists != null)
            {
                foreach (var list in syntax.AttributeLists)
                    foreach (var attr in list.Attributes)
                        if (attr.Name.ToString() == name)
                            return attr;
            }
            return null;
        }
    }
}

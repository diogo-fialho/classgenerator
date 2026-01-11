using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classgenerator
{
    public class ClassGenerator
    {
        private const string defaultFile = "TestClass.cs";
        private const string defaultTemplate = "classtemplate.txt";
        private const string defaultSuffix = "Generated";
        private const string defaultNamespacePrefix = "ClassGenerator";
        private const string defaultOutputSuffix = ".cs";
        private const string defaultParameterSuffix = "data";

        public readonly string SourceFile;
        public readonly string TemplateFile;
        public readonly string? NewClassName;
        public readonly string? NewNamespace;
        public readonly string? OutputFile;

        public ClassGenerator(
            string? sourceFile = null,
            string? templateFile = null,
            string? newClassName = null,
            string? newNamespace = null,
            string? outputFile = null)
        {
            this.SourceFile = sourceFile ?? defaultFile;
            this.TemplateFile = templateFile ?? defaultTemplate;
            this.NewClassName = newClassName;
            this.NewNamespace = newNamespace;
            this.OutputFile = outputFile;
        }

        public void Generate()
        {
            var code = File.ReadAllText(SourceFile);

            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetCompilationUnitRoot();

            var properties = root
                .DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .Select(p => new
                {
                    Name = p.Identifier.Text,
                    Type = p.Type.ToString(),
                    Modifiers = p.Modifiers.ToString()
                });
            var classes = root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>();

            var template = File.ReadAllText(TemplateFile);

            var props = properties
                .Select(prop => $"public {prop.Type} {prop.Name} {{ get; set; }}");

            var fromprops = properties
                .Select(prop => $"{prop.Name} = {defaultParameterSuffix}.{prop.Name};");

            var toprops = properties
                .Select(prop => $"{prop.Name} = {prop.Name},");

            string classname = NewClassName ?? classes.First().Identifier.Text + defaultSuffix;
            string namespacedesc = NewNamespace ?? $"{defaultNamespacePrefix}.{defaultSuffix}";
            template = template
                .Replace("{@Namespace}", namespacedesc)
                .Replace("{@ClassName}", classname)
                .Replace("{@OriginalClass}", classes.First().Identifier.Text)
                .Replace("{@PROPS}", string.Join("\n", props))
                .Replace("{@MAPPINGFROM}", string.Join("\n", fromprops))
                .Replace("{@MAPPINGTO}", string.Join("\n", toprops));

            string output = OutputFile ?? ($"{classname}{defaultOutputSuffix}");
            var fileInfo = new FileInfo(output);
            File.WriteAllText(fileInfo.FullName, template + "\n");
        }
    }
}

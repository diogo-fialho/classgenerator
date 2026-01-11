using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddCommandLine(args)
    .Build();

string fileCSLocation = config["file"] ?? "TestClass.cs";
var code = File.ReadAllText(fileCSLocation);

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

string templateCSLocation = config["template"] ?? "classtemplate.txt";
var template = File.ReadAllText(templateCSLocation);

var props = properties
    .Select(prop => $"public {prop.Type} {prop.Name} {{ get; set; }}");

var fromprops = properties
    .Select(prop => $"{prop.Name} = data.{prop.Name};");

var toprops = properties
    .Select(prop => $"{prop.Name} = {prop.Name},");

string classname = config["class"] ?? "New" + classes.First().Identifier.Text;
string namespacedesc = config["namespace"] ?? "BL.DTO";
template = template.Replace("{@Namespace}", namespacedesc);
template = template.Replace("{@ClassName}", classname);
template = template.Replace("{@OriginalClass}", classes.First().Identifier.Text);
template = template.Replace("{@PROPS}", string.Join("\n", props));
template = template.Replace("{@MAPPINGFROM}", string.Join("\n", fromprops));
template = template.Replace("{@MAPPINGTO}", string.Join("\n", toprops));

string output = config["out"] ?? "";
string filename = (config["filename"] ?? $"{classname}") + ".cs";
string fulloutput = $"{Path.Combine(output, filename)}";
File.WriteAllText(fulloutput, template + "\n");

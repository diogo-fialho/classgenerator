using Classgenerator;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddCommandLine(args)
    .Build();

new ClassGenerator(
    config["file"],
    config["template"],
    config["class"],
    config["namespace"],
    config["out"]
    )
    .Generate();

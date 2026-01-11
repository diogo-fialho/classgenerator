using Classgenerator;

namespace ClassGeneratorTest
{
    public class ClassGeneratorTests
    {
        private const string SourceClassCode = @"
        namespace TestNamespace
        {
            public class TestClass
            {
                public int Id { get; set; }
                public string Name { get; set; }
            }
        }
    ";

        private const string Template = @"
        namespace {@Namespace}
        {
            public class {@ClassName}
            {
                // Properties
                {@PROPS}

                public {@ClassName}({@OriginalClass} data)
                {
                    {@MAPPINGFROM}
                }

                public {@OriginalClass} ToOriginal()
                {
                    return new {@OriginalClass}
                    {
                        {@MAPPINGTO}
                    };
                }
            }
        }
    ";

        [Fact]
        public void Constructor_UsesDefaults_WhenParametersAreNull()
        {
            var generator = new ClassGenerator();

            Assert.Equal("TestClass.cs", generator.SourceFile);
            Assert.Equal("classtemplate.txt", generator.TemplateFile);
        }

        [Fact]
        public void Constructor_UsesProvidedParameters()
        {
            var generator = new ClassGenerator("src.cs", "tmpl.txt", "MyClass", "My.Namespace", "out.cs");

            Assert.Equal("src.cs", generator.SourceFile);
            Assert.Equal("tmpl.txt", generator.TemplateFile);
            Assert.Equal("MyClass", generator.NewClassName);
            Assert.Equal("My.Namespace", generator.NewNamespace);
            Assert.Equal("out.cs", generator.OutputFile);
        }

        [Fact]
        public void Generate_CreatesFile_WithExpectedContent()
        {
            // Arrange
            var sourcePath = Path.GetTempFileName();
            var templatePath = Path.GetTempFileName();
            var outputPath = Path.Combine(Path.GetTempPath(), "TestClassGenerated.cs");

            File.WriteAllText(sourcePath, SourceClassCode);
            File.WriteAllText(templatePath, Template);

            var generator = new ClassGenerator(
                sourceFile: sourcePath,
                templateFile: templatePath,
                newClassName: "MyGeneratedClass",
                newNamespace: "My.Generated.Namespace",
                outputFile: outputPath);

            // The expected output after template replacement
        var ExpectedOutput = @"
        namespace My.Generated.Namespace
        {
            public class MyGeneratedClass
            {
                // Properties
                public int Id { get; set; }
public string Name { get; set; }

                public MyGeneratedClass(TestClass data)
                {
                    Id = data.Id;
Name = data.Name;
                }

                public TestClass ToOriginal()
                {
                    return new TestClass
                    {
                        Id = Id,
Name = Name,
                    };
                }
            }
        }
    ".Replace("\r\n", "\n").Trim();

            // Act
            generator.Generate();

            // Assert
            Assert.True(File.Exists(outputPath));
            var output = File.ReadAllText(outputPath);

            Assert.Equal(ExpectedOutput, output.Replace("\r\n", "\n").Trim());

            // Cleanup
            File.Delete(sourcePath);
            File.Delete(templatePath);
            File.Delete(outputPath);
        }

        [Fact]
        public void Generate_UsesDefaults_WhenParametersAreNotProvided()
        {
            // Arrange
            var sourcePath = Path.GetTempFileName();
            var templatePath = Path.GetTempFileName();

            File.WriteAllText(sourcePath, SourceClassCode);
            File.WriteAllText(templatePath, Template);

            var generator = new ClassGenerator(
                sourceFile: sourcePath,
                templateFile: templatePath);

            // Act
            generator.Generate();

            // The expected output after template replacement
            var ExpectedOutput = @"
        namespace ClassGenerator.Generated
        {
            public class TestClassGenerated
            {
                // Properties
                public int Id { get; set; }
public string Name { get; set; }

                public TestClassGenerated(TestClass data)
                {
                    Id = data.Id;
Name = data.Name;
                }

                public TestClass ToOriginal()
                {
                    return new TestClass
                    {
                        Id = Id,
Name = Name,
                    };
                }
            }
        }
    ".Replace("\r\n", "\n").Trim();

            // Assert
            var expectedOutput = "TestClassGenerated.cs";
            Assert.True(File.Exists(expectedOutput));
            var output = File.ReadAllText(expectedOutput);

            Assert.Equal(ExpectedOutput, output.Replace("\r\n", "\n").Trim());

            // Cleanup
            File.Delete(sourcePath);
            File.Delete(templatePath);
            File.Delete(expectedOutput);
        }
    }
}
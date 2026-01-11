## Contents

- [Download] - Exe download location;
- [Template properties](#template-properties) - List of properties that can be used with templates
- [Commands](#commands) - Commands that can be used with .exe file

## Download exe
You can download exe from [here](https://github.com/diogo-fialho/classgenerator/releases/tag/v0.0.1)

## Template properties
These are properties that can be used together with templates when generating a new class, you can find examples in [examples](examples)


| Property | Notes |
|---|---|
| ClassName | Name of new class |
| OriginalClass | Name of the class inside base file |
| Namespace | Namespace |
| PROPS | properties read from file with c# pattern "public type PropertyName { get; set; }" |
| MAPPINGFROM | this is a more custom property, works a bit like props but uses pattern "PropertyName = data.PropertyName;" |
| MAPPINGTO | this is a more custom property, works a bit like props but uses pattern "PropertyName = PropertyName" |

## Commands
Contains list of commands that can be used with `.exe`, here is an example:
```
.\Classgenerator.exe --file=TestClass.cs --class=TestClss --namespace=Classgenerator --template=classtemplate.txt
```

#### Table
| Command | Description | Template property | Default value (*) |
|---|---|---|---|
| file | source file, should include extension | OriginalClass (does not include the .cs) | TestClass.cs |
| template | source of template file | - | classtemplate.txt |
| class | new class name | ClassName | OriginalClass.cs |
| namespace | namespace used in new file | Namespace | BL.DTO |
| out | output folder | - | where .exe is |
| filename | new file name | - | ClassName.cs |

*-Most of the default values like templatename is expected to be in same folder as exe

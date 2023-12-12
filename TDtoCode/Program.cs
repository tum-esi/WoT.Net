// See https://aka.ms/new-console-template for more information
using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.CSharp;
using NJsonSchema.Validation;
using System.Text;

string schemaString = File.ReadAllText("tdSchema.json", Encoding.UTF8);
var schema = await JsonSchema.FromJsonAsync(schemaString);
var settings = new CSharpGeneratorSettings();
var generator = new CSharpGenerator(schema, settings);
var code = generator.GenerateFile();

File.WriteAllText("TD.cs", code);
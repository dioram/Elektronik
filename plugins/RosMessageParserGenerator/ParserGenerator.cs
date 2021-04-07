using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Elektronik.RosMessageParserGenerator
{
    [Generator]
    public class ParserGenerator : ISourceGenerator
    {
        private static string GetSourceFileDir([CallerFilePath] string? callerFilePath = null)
            => Path.GetDirectoryName(callerFilePath) ?? "";

        private static readonly string[] ExcludedTypes =
        {
            "Header", "String", "Double", "Bool", "SByte", "Byte",
            "Int16", "UInt16", "Int32", "UInt32", "Int64", "UInt64",
        };
        
        private static readonly Dictionary<string, string> FuncNames = new();

        private static Assembly? _ros;
        private static Type? _baseType;

        public void Initialize(GeneratorInitializationContext context)
        { 
// #if DEBUG
//             if (!Debugger.IsAttached) 
//             {
//                 Debugger.Launch();
//             } 
// #endif
            _ros = Assembly.LoadFile(Path.Combine(GetSourceFileDir(), "RosBridgeClient.dll"));
            _baseType = _ros!.ExportedTypes.First(t => t.Name == "Message");
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var sourceBuilder = new StringBuilder("using System.IO;\n" +
                                                  "using RosSharp.RosBridgeClient.MessageTypes.Sensor;\n" +
                                                  "namespace Elektronik.RosPlugin.Common.RosMessages {\n" +
                                                  "public static partial class MessageParser {\n");
            // Static Constructor will be inserted here
            var insertIndex = sourceBuilder.Length;
            
            foreach (var type in _ros!.ExportedTypes.Where(t => t.IsSubclassOf(_baseType)
                                                                   && !t.Name.Contains("`")
                                                                   && !t.Name.Contains("Action")
                                                                   && !t.Name.Contains("Request")
                                                                   && !t.Name.Contains("Response")
                                                                   && !t.Namespace!.Contains("Moveit")
                                                                   && !ExcludedTypes.Contains(t.Name)))
            {
                sourceBuilder.Append(GenerateParseMethod(type));
            }
            
            // Creating static constructor
            var constructorBuilder = new StringBuilder("static MessageParser(){\n");
            foreach (var pair in FuncNames)
            {
                constructorBuilder.Append($"Parsers.Add(\"{pair.Key}\", {pair.Value});\n");
            }
            constructorBuilder.Append("}\n\n");
            sourceBuilder.Insert(insertIndex, constructorBuilder);

            sourceBuilder.Append("}\n}\n");
            context.AddSource("MessageParser.generated.cs", sourceBuilder.ToString());
        }

        private StringBuilder GenerateParseMethod(Type parsingType)
        {
            var parameters = parsingType
                    .GetConstructors()
                    .FirstOrDefault(c => c.GetParameters().Length > 0)
                    ?.GetParameters();
            if (parameters == null) return new StringBuilder();
            var sourceBuilder = new StringBuilder(
                $"public static {parsingType.Namespace}.{parsingType.Name} Parse{parsingType.Name}(Stream data, bool cdr)\n" +
                $" => new(");

            var messageName = (string) parsingType.GetField("RosMessageName").GetValue(null);
            FuncNames.Add(messageName, $"Parse{parsingType.Name}");
            var tmp = messageName.Split('/').ToList();
            tmp.Insert(1, "msg");
            messageName = string.Join("/", tmp);
            FuncNames.Add(messageName, $"Parse{parsingType.Name}");
            
            foreach (var param in parameters)
            {
                switch (param.ParameterType.IsArray)
                {
                case true when param.ParameterType.GetElementType() != typeof(byte):
                {
                    object sample = Activator.CreateInstance(parsingType);
                    int count = ((Array) sample.GetType().GetProperty(param.Name)!.GetValue(sample)).Length;
                    if (count == 0)
                    {
                        sourceBuilder.Append($"ParseArray(data, " +
                                             $"Parse{param.ParameterType.GetElementType()!.Name}, cdr)");
                    }
                    else
                    {
                        sourceBuilder.Append($"ParseArray(data, {count}, " +
                                             $"Parse{param.ParameterType.GetElementType()!.Name}, cdr)");
                    }

                    break;
                }
                case true when param.ParameterType.GetElementType() == typeof(byte):
                    sourceBuilder.Append($"ParseByteArray(data, cdr)");
                    break;
                default:
                    sourceBuilder.Append($"Parse{param.ParameterType.Name}(data, cdr)");
                    break;
                }

                if (param != parameters.Last()) sourceBuilder.Append(", ");
            }

            sourceBuilder.Append(");\n\n");
            return sourceBuilder;
        }
    }
}
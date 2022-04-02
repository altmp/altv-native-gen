using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AltV.NativesDb.Reader.Extensions;
using AltV.NativesDb.Reader.Models.NativeDb;
using Durty.AltV.NativesTypingsGenerator.Converters;
using Durty.AltV.NativesTypingsGenerator.Models.Typing;

namespace Durty.AltV.NativesTypingsGenerator.TypingDef
{
    public class TypeDefCSharpFileGenerator
    {
        private readonly TypeDef _typeDefFile;
        private readonly bool _generateDocumentation;

        public TypeDefCSharpFileGenerator(
            TypeDef typeDefFile,
            bool generateDocumentation = true)
        {
            _typeDefFile = typeDefFile;
            _generateDocumentation = generateDocumentation;
        }

        public string Generate(bool generateHeader = true, List<string> customHeaderLines = null)
        {
            StringBuilder fileContent = new StringBuilder(string.Empty);
            if (generateHeader)
            {
                if (customHeaderLines != null)
                {
                    foreach (var customHeaderLine in customHeaderLines)
                    {
                        fileContent.Append($"//{customHeaderLine}\n");
                    }
                }
                fileContent.Append("\n");
            }

            foreach (TypeDefModule typeDefModule in _typeDefFile.Modules)
            {
                fileContent.Append(GenerateModule(typeDefModule));
                fileContent.Append("\n");
            }

            return fileContent.ToString();
        }

        private StringBuilder GenerateModule(TypeDefModule typeDefModule)
        {
            StringBuilder result = new StringBuilder(string.Empty);
            result.Append($"using System.Numerics;\n");
            result.Append($"namespace AltV.Net.Client\n{{\n");
            result.Append($"\tpublic class Natives\n\t{{\n");
            result.Append($"\t\tprivate IntPtr handle;\n");
            foreach (var typeDefFunction in typeDefModule.Functions)
            {
                result.Append($"\t\tprivate {GetUnmanagedDelegateType(typeDefFunction)} {GetFixedTypeDefFunctionName(typeDefFunction.Name)};\n");
            }
            result.Append($"\n");
            result.Append($"\t\tpublic Natives(string dllName)\n\t\t{{\n");
            result.Append($"\t\t\tconst DllImportSearchPath dllImportSearchPath = DllImportSearchPath.LegacyBehavior | DllImportSearchPath.AssemblyDirectory | DllImportSearchPath.SafeDirectories | DllImportSearchPath.System32 | DllImportSearchPath.UserDirectories | DllImportSearchPath.ApplicationDirectory | DllImportSearchPath.UseDllDirectoryForDependencies;\n");
            result.Append($"\t\t\thandle = NativeLibrary.Load(dllName, Assembly.GetExecutingAssembly(), dllImportSearchPath);\n");
            result.Append($"\t\t}}\n\n");
            
            result = typeDefModule.Functions.Aggregate(result, (current, typeDefFunction) => current.Append($"{GenerateFunction(typeDefFunction)}\n"));
            result.Append("\t}\n");
            result.Append("}");
            return result;
        }

        private string GetFixedTypeDefFunctionName(string name)
        {
            return name.StartsWith("_") ? name : "_" + name;
        }
        
        private string GetUnmanagedDelegateType(TypeDefFunction function)
        {
            var converter = new NativeTypeToCSharpTypingConverter();
            return $"delegate* unmanaged[Cdecl]<{string.Join("", function.Parameters.Select(p => $"{converter.Convert(null, p.NativeType, p.IsReference)}, "))}{converter.Convert(null, function.ReturnType.NativeType[0], false)}>";
        }

        private string GetFixedTypeDefParameterName(string name)
        {
            return IsParameterNameReservedCSharpKeyWord(name) ? "@" + name : name;
        }

        private StringBuilder GenerateFunction(TypeDefFunction typeDefFunction)
        {
            StringBuilder result = new StringBuilder(string.Empty);
            if (_generateDocumentation)
            {
                result.Append(GenerateFunctionDocumentation(typeDefFunction));
            }
            var fixedTypeDefName = GetFixedTypeDefFunctionName(typeDefFunction.Name);
            var cSharpReturnType = new NativeTypeToCSharpTypingConverter().Convert(null, typeDefFunction.ReturnType.NativeType[0], false);
            result.Append($"\t\tpublic {cSharpReturnType} {typeDefFunction.Name.FirstCharToUpper()}(");
            foreach (var parameter in typeDefFunction.Parameters)
            {
                result.Append($"{(parameter.IsReference ? "ref " : "")}{new NativeTypeToCSharpTypingConverter().Convert(null, parameter.NativeType, false)} {GetFixedTypeDefParameterName(parameter.Name)}");
                if (typeDefFunction.Parameters.Last() != parameter)
                {
                    result.Append(", ");
                }
            }
            result.Append($")\n\t\t{{\n");
            result.Append($"\t\t\tif ({fixedTypeDefName} == null) {fixedTypeDefName} = ({GetUnmanagedDelegateType(typeDefFunction)}) NativeLibrary.GetExport(handle, \"Native_{typeDefFunction.Name}\");\n");
            var hasReturn = typeDefFunction.ReturnType.NativeType[0] != NativeType.Void;
            result.Append($"\t\t\t{(hasReturn ? "return " : "")}{fixedTypeDefName}(");
            GenerateCallParameters(result, typeDefFunction);
            
            result.Append($"\t\t}}\n");

            return result;
        }

        private void GenerateCallParameters(StringBuilder result, TypeDefFunction typeDefFunction, bool closeFunction = true)
        {
            foreach (var parameter in typeDefFunction.Parameters)
            {
                result.Append($"{GetFixedTypeDefParameterName(parameter.Name)}{(parameter.IsReference ? "&" : "")}");
                if (typeDefFunction.Parameters.Last() != parameter)
                {
                    result.Append(", ");
                }
            }

            result.Append(closeFunction ? $");\n" : ")");
        }

        private StringBuilder GenerateFunctionDocumentation(TypeDefFunction typeDefFunction)
        {
            //When no docs exist
            if (typeDefFunction.ReturnType.NativeType.Count <= 1 &&
                string.IsNullOrEmpty(typeDefFunction.Description) &&
                typeDefFunction.Parameters.All(p => string.IsNullOrEmpty(p.Description) && string.IsNullOrEmpty(typeDefFunction.ReturnType.Description)))
                return new StringBuilder(string.Empty);

            StringBuilder result = new StringBuilder($"\t\t/// <summary>\n");
            if (!string.IsNullOrEmpty(typeDefFunction.Description))
            {
                string[] descriptionLines = typeDefFunction.Description.Split("\n");
                foreach (string descriptionLine in descriptionLines)
                {
                    string sanitizedDescriptionLine = descriptionLine.Replace("/*", string.Empty).Replace("*/", string.Empty).Trim();
                    result.Append($"\t\t/// {sanitizedDescriptionLine}\n");
                }
            }
            result.Append("\t\t/// </summary>\n");

            //Add @remarks in the future?
            foreach (var parameter in typeDefFunction.Parameters)
            {
                if (!string.IsNullOrEmpty(parameter.Description))
                {
                    result.Append($"\t\t/// <param name=\"{GetFixedTypeDefParameterName(parameter.Name)}\">{parameter.Description}</param>\n");
                }
            }
            
            if (!string.IsNullOrEmpty(typeDefFunction.ReturnType.Description))
            {
                result.Append($"\t\t/// <returns>{typeDefFunction.ReturnType.Description}</returns>\n");
            }
            return result;
        }

        private bool IsParameterNameReservedCSharpKeyWord(string parameterName)
        {
            var reservedKeywords = new List<string>()
            {
                "abstract",
                "as",
                "base",
                "bool",
                "break",
                "byte",
                "case",
                "catch",
                "char",
                "checked",
                "class",
                "const",
                "continue",
                "decimal",
                "default",
                "delegate",
                "do",
                "double",
                "else",
                "enum",
                "event",
                "explicit",
                "extern",
                "false",
                "finally",
                "fixed",
                "float",
                "for",
                "foreach",
                "goto",
                "if",
                "implicit",
                "in",
                "int",
                "interface",
                "internal",
                "is",
                "lock",
                "long",
                "namespace",
                "new",
                "null",
                "object",
                "operator",
                "out",
                "override",
                "params",
                "private",
                "protected",
                "public",
                "readonly",
                "ref",
                "return",
                "sbyte",
                "sealed",
                "short",
                "sizeof",
                "stackalloc",
                "static",
                "string",
                "struct",
                "switch",
                "this",
                "throw",
                "true",
                "try",
                "typeof",
                "uint",
                "ulong",
                "unchecked",
                "unsafe",
                "ushort",
                "using",
                "static",
                "virtual",
                "void",
                "volatile",
                "while"
            };
            return reservedKeywords.Any(k => parameterName.ToLower() == k);
        }
    }
}

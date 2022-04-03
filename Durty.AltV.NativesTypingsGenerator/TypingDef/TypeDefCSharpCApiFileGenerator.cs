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
    public class TypeDefCSharpCApiFileGenerator
    {
        private readonly TypeDef _typeDefFile;
        private readonly bool _generateDocumentation;

        public TypeDefCSharpCApiFileGenerator(
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
            result.Append($"alt::Ref<alt::INative::Context> ctx;\n\n");
            result.Append($"void InitNatives(alt::ICore* core) {{\n");
            result.Append($"\tctx = core->CreateNativesContext();\n");
            result.Append($"}}\n\n");
            
            foreach (var typeDefFunction in typeDefModule.Functions)
            {
                result.Append(GenerateFunction(typeDefFunction));
                result.Append("\n");
            }
            return result;
        }
        
        private StringBuilder GenerateFunction(TypeDefFunction typeDefFunction)
        {
            StringBuilder result = new StringBuilder(string.Empty);
            if (string.IsNullOrWhiteSpace(typeDefFunction.LatestHash))
            {
                Console.WriteLine("Warning: Function without hash: " + typeDefFunction.Name);
                return result;
            }
            StringBuilder resultEnd = new StringBuilder(string.Empty);

            var converter = new NativeTypeToCSharpCApiTypingConverter();
            result.Append($"EXPORT {converter.Convert(null, typeDefFunction.ReturnType.NativeType[0], false)} Native_{typeDefFunction.Name}(");
            foreach (var parameter in typeDefFunction.Parameters)
            {
                result.Append($"{converter.Convert(null, parameter.NativeType, parameter.IsReference)} _{parameter.Name}");
                if (typeDefFunction.Parameters.Last() != parameter)
                {
                    result.Append(", ");
                }
            }
            
            result.Append($") {{\n");
            result.Append($"\tstatic auto native = alt::ICore::Instance().GetNativeByHash({typeDefFunction.LatestHash});\n");
            result.Append($"\tctx->Reset();\n");
            foreach (var parameter in typeDefFunction.Parameters)
            {
                if (parameter.NativeType != NativeType.Vector3)
                    result.Append($"\tctx->Push(_{parameter.Name});\n");
                else
                {
                    result.Append($"\talt::INative::Vector3 converted_{parameter.Name} {{ _{parameter.Name}.x, 0, _{parameter.Name}.y, 0, _{parameter.Name}.z }};\n");
                    result.Append($"\tctx->Push(converted_{parameter.Name});\n");
                    resultEnd.Append($"\t_{parameter.Name}.x = converted_{parameter.Name}.x;\n");
                    resultEnd.Append($"\t_{parameter.Name}.y = converted_{parameter.Name}.y;\n");
                    resultEnd.Append($"\t_{parameter.Name}.z = converted_{parameter.Name}.z;\n");
                }
            }
            result.Append($"\tnative->Invoke(ctx);\n");
            
            var resultConverter = new NativeReturnTypeToCSharpCApiTypingConverter();
            if (typeDefFunction.ReturnType.NativeType[0] != NativeType.Void)
            {
                if (typeDefFunction.ReturnType.NativeType[0] == NativeType.Vector3)
                {
                    result.Append($"\tauto resultVec = ctx->{resultConverter.Convert(null, typeDefFunction.ReturnType.NativeType[0])}();\n");
                    result.Append($"\treturn {{ resultVec.x, resultVec.y, resultVec.z }};\n");
                }
                else
                {
                    result.Append($"\treturn ctx->{resultConverter.Convert(null, typeDefFunction.ReturnType.NativeType[0])}();\n");
                }
            }
            result.Append(resultEnd.ToString());
            result.Append($"}}\n");

            return result;
        }
    }
}

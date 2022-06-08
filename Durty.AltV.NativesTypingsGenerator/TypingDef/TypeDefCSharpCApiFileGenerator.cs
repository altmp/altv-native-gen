﻿using System;
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
            result.Append("// THIS IS AN AUTOGENERATED FILE. DO NOT EDIT THIS FILE DIRECTLY.\n\n");
            result.Append($"#include \"natives.h\"\n");
            result.Append($"#include \"../../c-api/data/types.h\"\n");
            result.Append($"#include \"Log.h\"\n");
            result.Append($"#include <cstring>\n");
            result.Append($"#include <stdlib.h>\n\n");
            result.Append($"alt::Ref<alt::INative::Context> ctx;\n\n");
            result.Append($"void InitNatives() {{\n");
            result.Append($"\tctx = alt::ICore::Instance().CreateNativesContext();\n");
            result.Append($"}}\n\n");
            result.Append($"const char* AllocateString(const char* str) {{\n");
            result.Append($"\tsize_t stringSize = strlen(str);\n");
            result.Append($"\tchar* writable = new char[stringSize + 1];\n");
            result.Append($"\tstd::memcpy(writable, str, stringSize);\n");
            result.Append($"\twritable[stringSize] = '\\0';\n");
            result.Append($"\treturn writable;\n");
            result.Append($"}}\n\n");
            result.Append($"static char* SaveString(const char* str) {{\n");
            result.Append($"\tif (str == nullptr) return nullptr;\n");
            result.Append($"\tstatic char* stringValues[256] = {{ 0 }};\n");
            result.Append($"\tstatic int nextString = 0;\n");
            result.Append($"\tif (stringValues[nextString]) free(stringValues[nextString]);\n");
            result.Append($"\tchar* _str = _strdup(str);\n");
            result.Append($"\tstringValues[nextString] = _str;\n");
            result.Append($"\tnextString = (nextString + 1) % 256;\n");
            result.Append($"\treturn _str;\n");
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
            StringBuilder resultEnd = new StringBuilder(string.Empty);

            var converter = new NativeTypeToCSharpCApiTypingConverter();
            result.Append($"EXPORT {converter.Convert(null, typeDefFunction.ReturnType.NativeType[0], false)} Native_{typeDefFunction.Name}(bool& success");
            foreach (var parameter in typeDefFunction.Parameters)
            {
                result.Append($", {converter.Convert(null, parameter.NativeType, parameter.IsReference)} _{parameter.Name}");
            }
            
            result.Append($") {{\n");
            result.Append($"\tstatic auto native = alt::ICore::Instance().GetNativeByHash({typeDefFunction.BaseHash});\n");
            result.Append($"\tctx->Reset();\n");
            foreach (var parameter in typeDefFunction.Parameters)
            {
                if (parameter.NativeType == NativeType.Vector3)
                {
                    result.Append($"\talt::INative::Vector3 converted_{parameter.Name} {{ _{parameter.Name}.x, 0, _{parameter.Name}.y, 0, _{parameter.Name}.z }};\n");
                    result.Append($"\tctx->Push(&converted_{parameter.Name});\n");
                    resultEnd.Append($"\t_{parameter.Name}.x = converted_{parameter.Name}.x;\n");
                    resultEnd.Append($"\t_{parameter.Name}.y = converted_{parameter.Name}.y;\n");
                    resultEnd.Append($"\t_{parameter.Name}.z = converted_{parameter.Name}.z;\n");
                }
                else if(parameter.IsReference && parameter.NativeType == NativeType.String)
                {
                    result.Append($"\tauto ptr_{parameter.Name} = SaveString(_{parameter.Name});\n");   
                    result.Append($"\tctx->Push(&ptr_{parameter.Name});\n");
                    resultEnd.Append($"\t_{parameter.Name} = ptr_{parameter.Name};\n");
                }
                else if(parameter.NativeType == NativeType.String)
                {   
                    result.Append($"\tctx->Push(SaveString(_{parameter.Name}));\n");
                }
                else if(parameter.IsReference && parameter.NativeType == NativeType.Boolean)
                {
                    result.Append($"\tauto ptr_{parameter.Name} = (int32_t) _{parameter.Name};\n");   
                    result.Append($"\tctx->Push(&ptr_{parameter.Name});\n");
                    resultEnd.Append($"\t_{parameter.Name} = (bool) ptr_{parameter.Name};\n");
                }
                else if(parameter.NativeType == NativeType.Boolean)
                {   
                    result.Append($"\tctx->Push((int32_t) _{parameter.Name});\n");
                }
                else if(parameter.IsReference)
                {
                    result.Append($"\tauto ptr_{parameter.Name} = _{parameter.Name};\n");   
                    result.Append($"\tctx->Push(&ptr_{parameter.Name});\n");
                    resultEnd.Append($"\t_{parameter.Name} = ptr_{parameter.Name};\n");
                }
                else
                {
                    result.Append($"\tctx->Push(_{parameter.Name});\n");
                }
            }
            result.Append($"\tif (!native->Invoke(ctx)) {{\n");
            result.Append($"\t\tsuccess = false;\n");
            result.Append($"\t\treturn {(new NativeTypeToCSharpCApiDefaultValueConverter().Convert(null, typeDefFunction.ReturnType.NativeType[0]))};\n");
            result.Append($"\t}}\n");
            result.Append($"\tsuccess = true;\n");
            
            result.Append(resultEnd.ToString());
            var resultConverter = new NativeReturnTypeToCSharpCApiTypingConverter();
            if (typeDefFunction.ReturnType.NativeType[0] != NativeType.Void)
            {
                if (typeDefFunction.ReturnType.NativeType[0] == NativeType.Vector3)
                {
                    result.Append($"\tauto resultVec = ctx->{resultConverter.Convert(null, typeDefFunction.ReturnType.NativeType[0])}();\n");
                    result.Append($"\treturn {{ resultVec.x, resultVec.y, resultVec.z }};\n");
                }
                else if (typeDefFunction.ReturnType.NativeType[0] == NativeType.String)
                {
                    result.Append($"\treturn AllocateString(ctx->{resultConverter.Convert(null, typeDefFunction.ReturnType.NativeType[0])}());\n");
                }
                else
                {
                    result.Append($"\treturn ctx->{resultConverter.Convert(null, typeDefFunction.ReturnType.NativeType[0])}();\n");
                }
            }
            result.Append($"}}\n");

            return result;
        }
    }
}

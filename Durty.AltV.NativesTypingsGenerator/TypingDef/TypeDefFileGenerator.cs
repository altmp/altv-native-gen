﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Durty.AltV.NativesTypingsGenerator.Models.Typing;

namespace Durty.AltV.NativesTypingsGenerator.TypingDef
{
    public class TypeDefFileGenerator
    {
        private readonly TypeDef _typeDefFile;
        private readonly bool _generateDocumentation;
        private readonly string _indent;

        public TypeDefFileGenerator(
            TypeDef typeDefFile,
            bool generateDocumentation = true,
            string indent = null)
        {
            _typeDefFile = typeDefFile;
            _generateDocumentation = generateDocumentation;
            _indent = indent ?? "\t";
        }

        public void Generate(out string str)
        {
            StringBuilder fileContent = new StringBuilder(string.Empty);
            fileContent.Append("// THIS IS AN AUTOGENERATED FILE. DO NOT EDIT THIS FILE DIRECTLY.\n\n");

            foreach (TypeDefModule typeDefModule in _typeDefFile.Modules)
            {
                fileContent.Append(GenerateModule(typeDefModule));
                fileContent.Append("\n");
            }

            str = fileContent.ToString();
        }

        private string GenerateInterface(TypeDefInterface typeDefInterface)
        {
            StringBuilder result = new StringBuilder($"{_indent}interface {typeDefInterface.Name} {{\n");
            result = typeDefInterface.Properties.Aggregate(result, (current, property) => current.Append($"{_indent}{_indent}{property.Name}: {property.Type};\n"));
            result.Append($"{_indent}}}");
            return result.ToString();
        }

        private string GenerateType(TypeDefType typeDefType)
        {
            return $"{_indent}type {typeDefType.Name} = {typeDefType.TypeDefinition};";
        }

        private StringBuilder GenerateModule(TypeDefModule typeDefModule)
        {
            StringBuilder result = new StringBuilder();
            result.Append("/// <reference types=\"@altv/types-client\"/>");
            result.Append("\n");
            result.Append("/**\n * @module natives\n */");
            result.Append("\n");
            result.Append($"declare module \"{typeDefModule.Name}\" {{");
            result.Append("\n");
            result.Append($"{_indent}import {{ Vector3, Entity, Vehicle, Player }} from \"alt-client\";");
            result.Append("\n");
            typeDefModule.Interfaces?.Aggregate(result, (current, typeDef) => current.Append($"{GenerateInterface(typeDef)}\n"));
            result.Append("\n");
            typeDefModule.Types?.Aggregate(result, (current, typeDef) => current.Append($"{GenerateType(typeDef)}\n"));
            result.Append("\n");
            typeDefModule.Functions?.Aggregate(result, (current, typeDef) => current.Append($"{GenerateFunction(typeDef)}\n"));
            result.Length--;
            result.Append("}");
            return result;
        }

        private StringBuilder GenerateFunction(TypeDefFunction typeDefFunction)
        {
            StringBuilder result = new StringBuilder(string.Empty);
            if (_generateDocumentation)
            {
                result.Append(GenerateFunctionDocumentation(typeDefFunction));
            }
            result.Append($"{_indent}export function {typeDefFunction.Name}(");
            foreach (var parameter in typeDefFunction.Parameters)
            {
                result.Append($"{parameter.Name}{(typeDefFunction.Parameters.Count > 1 && parameter.IsReference && parameter.IsLastReference ? "?" : "")}: {parameter.Type}");
                if (typeDefFunction.Parameters.Last() != parameter)
                {
                    result.Append(", ");
                }
            }
            result.Append($"): {typeDefFunction.ReturnType.Name};\n");

            return result;
        }

        private StringBuilder GenerateFunctionDocumentation(TypeDefFunction typeDefFunction)
        {
            //When no docs exist
            if (string.IsNullOrEmpty(typeDefFunction.Description) && typeDefFunction.Parameters.All(p => string.IsNullOrEmpty(p.Description) && string.IsNullOrEmpty(typeDefFunction.ReturnType.Description)))
                return new StringBuilder(string.Empty);

            StringBuilder result = new StringBuilder($"{_indent}/**\n");
            if (!string.IsNullOrEmpty(typeDefFunction.Description))
            {
                string[] descriptionLines = typeDefFunction.Description.Split("\n");
                foreach (string descriptionLine in descriptionLines)
                {
                    string sanitizedDescriptionLine = descriptionLine.Replace("/*", string.Empty).Replace("*/", string.Empty).Trim();
                    result.Append($"{_indent}* {sanitizedDescriptionLine}\n");
                }
            }
            //Add @remarks in the future?
            foreach (var parameter in typeDefFunction.Parameters)
            {
                if (!string.IsNullOrEmpty(parameter.Description))
                {
                    result.Append($"{_indent}* @param {parameter.Name} {parameter.Description}\n");
                }
            }
            if (!string.IsNullOrEmpty(typeDefFunction.ReturnType.Description))
            {
                result.Append($"{_indent}* @returns {typeDefFunction.ReturnType.Description}\n");
            }
            result.Append($"{_indent}*/\n");
            return result;
        }
    }
}

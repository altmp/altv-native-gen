using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Durty.AltV.NativesTypingsGenerator.Models.Typing
{
    /// <summary>
    /// export function getPlayerPed(player: number): number;
    /// </summary>
    [DebuggerDisplay("{Name}(): {ReturnType};")]
    public class TypeDefFunction
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<TypeDefFunctionParameter> Parameters { get; set; }

        public TypeDefFunctionReturnType ReturnType { get; set; }

        public override string ToString()
        {
            string result = string.Empty;
            result += $"{GetDocumentation()}";
            result += $"\texport function {Name}(";
            foreach (var parameter in Parameters)
            {
                result += $"{parameter.Name}: {parameter.Type}";
                if (Parameters.Last() != parameter)
                {
                    result += ", ";
                }
            }
            result += $"): {ReturnType.Name};";

            return result;
        }

        //TODO: outsource docs generation to own class, to allow disabling it
        private string GetDocumentation()
        {
            //When no docs exist
            if (string.IsNullOrEmpty(Description) && Parameters.All(p => string.IsNullOrEmpty(p.Description) && string.IsNullOrEmpty(ReturnType.Description)))
                return string.Empty;

            string result = $"\t/**\n";
            if (!string.IsNullOrEmpty(Description))
            {
                string[] descriptionLines = Description.Split("\n");
                foreach (string descriptionLine in descriptionLines.Take(10)) //Docs summary can only show max 10 lines, will be cut afterwards anyway
                {
                    string sanitizedDescriptionLine = descriptionLine.Replace("/*", string.Empty).Replace("*/", string.Empty);
                    result += $"\t* {sanitizedDescriptionLine}\n";
                }
            }
            //TODO: add reference link (nativedb url)
            //Add @remarks in the future?
            //TODO: Add intelligent parameter description resolving (pattern matchin paraname - description)
            foreach (var parameter in Parameters)
            {
                if (!string.IsNullOrEmpty(parameter.Description))
                {
                    result += $"\t* @param {parameter.Name} {parameter.Description}\n";
                }
            }
            if (!string.IsNullOrEmpty(ReturnType.Description))
            {
                result += $"\t* {ReturnType.Description}\n";
            }
            result += "\t*/\n";
            return result;
        }
    }
}

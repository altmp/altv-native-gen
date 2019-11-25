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

        public List<TypeDefFunctionParameter> Parameters { get; set; }

        public string ReturnType { get; set; }

        public override string ToString()
        {
            string result = $"export function {Name}(";
            foreach (var parameter in Parameters)
            {
                result += $"{parameter.Name}: {parameter.Type}";
                if (Parameters.Last() != parameter)
                {
                    result += ", ";
                }
            }
            result += $"): {ReturnType};";

            return result;
        }
    }

    public class TypeDefFunctionParameter
    {
        public string Name { get; set; }

        public string Type { get; set; }
    }
}

using System.Collections.Generic;
using System.Diagnostics;

namespace Durty.AltV.NativesTypingsGenerator.Models.Typing
{
    /// <summary>
    /// export function getPlayerPed(player: number): number;
    /// </summary>
    [DebuggerDisplay("{Name}(): {ReturnType};")]
    public class TypeDefFunction
    {
        public string LatestHash { get; set; }
        
        public string Name { get; set; }

        public string Description { get; set; }

        public List<TypeDefFunctionParameter> Parameters { get; set; }

        public TypeDefFunctionReturnType ReturnType { get; set; }
    }
}

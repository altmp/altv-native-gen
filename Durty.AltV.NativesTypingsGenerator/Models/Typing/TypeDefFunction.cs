using System.Collections.Generic;
using System.Diagnostics;

namespace Durty.AltV.NativesTypingsGenerator.Models.Typing
{
    [DebuggerDisplay("{Name}(): {ReturnType};")]
    public class TypeDefFunction
    {
        public string Name { get; set; }

        public List<TypeDefFunctionParameter> Parameters { get; set; }

        public string ReturnType { get; set; }
    }

    public class TypeDefFunctionParameter
    {
        public string Name { get; set; }

        public string Type { get; set; }
    }
}

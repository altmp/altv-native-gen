using System.Collections.Generic;

namespace Durty.AltV.NativesTypingsGenerator.Models.Typing
{
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

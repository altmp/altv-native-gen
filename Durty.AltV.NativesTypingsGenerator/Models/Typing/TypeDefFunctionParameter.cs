using Durty.AltV.NativesTypingsGenerator.Models.NativeDb;

namespace Durty.AltV.NativesTypingsGenerator.Models.Typing
{
    public class TypeDefFunctionParameter
    {
        public string Name { get; set; }

        public NativeType NativeType { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }
    }
}
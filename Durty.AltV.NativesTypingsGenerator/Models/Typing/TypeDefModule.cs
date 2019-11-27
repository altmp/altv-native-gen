using System.Collections.Generic;

namespace Durty.AltV.NativesTypingsGenerator.Models.Typing
{
    public class TypeDefModule
    {
        public string Name { get; set; }

        public List<TypeDefFunction> Functions { get; set; }
    }
}

using System.Collections.Generic;

namespace Durty.AltV.NativesTypingsGenerator.Models.Typing
{
    public class TypeDefFile
    {
        public List<TypeDefInterface> Interfaces { get; set; }

        public List<TypeDefType> Types { get; set; }

        public List<TypeDefFunction> Functions { get; set; }
    }
}

using System.Collections.Generic;

namespace Durty.AltV.NativesTypingsGenerator.Models.Typing
{
    public class TypeDef
    {
        public List<TypeDefInterface> Interfaces { get; set; }

        public List<TypeDefType> Types { get; set; }

        public List<TypeDefModule> Modules { get; set; }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace Durty.AltV.NativesTypingsGenerator.Models.Typing
{
    /// <summary>
    /// interface Vector3 {
    /// x: number;
    /// y: number;
    /// z: number;
    /// }
    /// </summary>
    public class TypeDefInterface
    {
        public string Name { get; set; }

        public List<TypeDefInterfaceProperty> Properties { get; set; }
    }
}

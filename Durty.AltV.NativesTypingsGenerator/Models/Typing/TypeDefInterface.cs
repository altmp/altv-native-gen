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

        public override string ToString()
        {
            string result = $"interface {Name} {{\n";
            result = Properties.Aggregate(result, (current, property) => current + $"  {property.Name}: {property.Type};\n");
            result += "}";
            return result;
        }
    }

    public class TypeDefInterfaceProperty
    {
        public string Name { get; set; }

        public string Type { get; set; }
    }
}

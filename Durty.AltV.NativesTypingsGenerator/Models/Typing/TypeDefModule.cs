using System.Collections.Generic;
using System.Linq;

namespace Durty.AltV.NativesTypingsGenerator.Models.Typing
{
    public class TypeDefModule
    {
        public string Name { get; set; }

        public List<TypeDefFunction> Functions { get; set; }

        public override string ToString()
        {
            string result = "";
            result += $"declare module \"{Name}\" {{\n";
            result = Functions.Aggregate(result, (current, typeDefFunction) => current + $"{typeDefFunction}\n");
            result += "}";
            return result;
        }
    }
}

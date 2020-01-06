using System.Text;

namespace Durty.AltV.NativesTypingsGenerator.Models.Typing
{
    /// <summary>
    /// type vectorPtr = Vector3;
    /// </summary>
    public class TypeDefType
    {
        public string Name { get; set; }

        public string TypeDefinition { get; set; }

        public TypeDefType()
        { }

        public TypeDefType(string name, string definition)
        {
            Name = name;
            TypeDefinition = definition;
        }

        /// <summary>
        /// Example output: type Month = 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | 11;
        /// </summary>
        /// <param name="name">Name of type</param>
        /// <param name="rangeStart">Start for range of allowed numbers</param>
        /// <param name="rangeEnd">End for range of allowed numbers</param>
        public TypeDefType(string name, int rangeStart, int rangeEnd)
        {
            Name = name;
            TypeDefinition = GenerateRangeTypeDefintion(rangeStart, rangeEnd);
        }

        private string GenerateRangeTypeDefintion(int start, int end)
        {
            StringBuilder definition = new StringBuilder(string.Empty);
            for (int i = start; i <= end; i++)
            {
                definition.Append(i);
                if (i != end)
                {
                    definition.Append(" | ");
                }
            }

            return definition.ToString();
        }
    }
}

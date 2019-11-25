using System.Linq;
using Durty.AltV.NativesTypingsGenerator.Models.Typing;

namespace Durty.AltV.NativesTypingsGenerator
{
    public class TypeDefFileGenerator
    {
        private readonly TypeDefFile _typeDefFile;

        public TypeDefFileGenerator(TypeDefFile typeDefFile)
        {
            _typeDefFile = typeDefFile;
        }

        public string Generate()
        {
            string fileContent = _typeDefFile.Interfaces.Aggregate("", (current, typeDefInterface) => current + typeDefInterface + "\n");
            fileContent += "\n";

            fileContent += _typeDefFile.Types.Aggregate("", (current, typeDefType) => current + typeDefType + "\n");
            fileContent += "\n";

            foreach (TypeDefModule typeDefModule in _typeDefFile.Modules)
            {
                fileContent += typeDefModule.ToString();
                fileContent += "\n";
            }

            return fileContent;
        }
    }
}

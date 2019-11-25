using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Durty.AltV.NativesTypingsGenerator.Models.Typing;

namespace Durty.AltV.NativesTypingsGenerator
{
    public class TypeDefFileWriter
    {
        private readonly TypeDefFile _typeDefFile;

        public TypeDefFileWriter(TypeDefFile typeDefFile)
        {
            _typeDefFile = typeDefFile;
        }

        public void Write(string filePath)
        {
            string fileContent = _typeDefFile.Interfaces.Aggregate("", (current, typeDefInterface) => current + typeDefInterface + "\n");
            fileContent += "\n";

            fileContent += _typeDefFile.Types.Aggregate("", (current, typeDefType) => current + typeDefType + "\n");
            fileContent += "\n";

            fileContent += "declare module \"natives\" {\n";
            fileContent = _typeDefFile.Functions.Aggregate(fileContent, (current, typeDefFunction) => current + $"	{typeDefFunction}\n");
            fileContent += "}";
            File.WriteAllText(filePath, fileContent);
        }
    }
}

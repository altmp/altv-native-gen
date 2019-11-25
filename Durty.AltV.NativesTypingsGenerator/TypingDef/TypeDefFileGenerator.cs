﻿using System;
using System.Linq;
using Durty.AltV.NativesTypingsGenerator.Models.Typing;

namespace Durty.AltV.NativesTypingsGenerator.TypingDef
{
    public class TypeDefFileGenerator
    {
        private readonly TypeDefFile _typeDefFile;

        public TypeDefFileGenerator(TypeDefFile typeDefFile)
        {
            _typeDefFile = typeDefFile;
        }

        public string Generate(bool generateHeader = true)
        {
            string fileContent = "";
            if (generateHeader)
            {
                // THIS FILE IS AUTOGENERATED
                // Generated on "11/15/2019, 10:14:03 PM"
                fileContent += $"// THIS FILE IS AUTOGENERATED by Durty AltV NativeDB Typings Generator\n// Generated {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}\n\n";
            }
            fileContent += _typeDefFile.Interfaces.Aggregate("", (current, typeDefInterface) => current + typeDefInterface + "\n");
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
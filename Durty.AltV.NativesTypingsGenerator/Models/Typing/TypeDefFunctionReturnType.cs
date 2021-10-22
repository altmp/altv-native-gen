using System.Collections.Generic;
using AltV.NativesDb.Reader.Models.NativeDb;

namespace Durty.AltV.NativesTypingsGenerator.Models.Typing
{
    public class TypeDefFunctionReturnType
    {
        public List<NativeType> NativeType { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
using System.Collections.Generic;
using System.Linq;
using AltV.NativesDb.Reader.Models.NativeDb;

namespace Durty.AltV.NativesTypingsGenerator.Converters
{
    public class NativeReturnTypeToCSharpTypingConverter
    {
        public string Convert(Native native, List<NativeType> nativeReturnTypes)
        {
            NativeTypeToCSharpTypingConverter nativeTypeToTypingConverter = new NativeTypeToCSharpTypingConverter();

            if (nativeReturnTypes.Count == 1)
            {
                return nativeTypeToTypingConverter.Convert(native, nativeReturnTypes.First(), false);
            }
            else if (nativeReturnTypes.Count > 1) 
            {
                var returnTypeForTyping = "(";
                for (var i = 0; i < nativeReturnTypes.Count; i++)
                {
                    var tupleReturnType = nativeTypeToTypingConverter.Convert(native, nativeReturnTypes[i], false);
                    if (tupleReturnType == "void")
                    {
                        returnTypeForTyping += "object";
                    }
                    else
                    {
                        returnTypeForTyping += tupleReturnType;
                    }
                    if (i != nativeReturnTypes.Count - 1)
                    {
                        returnTypeForTyping += ", ";
                    }
                }
                returnTypeForTyping += ")";
                return returnTypeForTyping;
            }
            return "object";
        }
    }
}

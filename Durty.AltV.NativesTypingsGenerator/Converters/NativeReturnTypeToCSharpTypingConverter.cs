using System.Collections.Generic;
using System.Linq;
using Durty.AltV.NativesTypingsGenerator.Models.NativeDb;

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
                string returnTypeForTyping = "(";
                for (int i = 0; i < nativeReturnTypes.Count; i++)
                {
                    returnTypeForTyping += nativeTypeToTypingConverter.Convert(native, nativeReturnTypes[i], false);
                    if (i != nativeReturnTypes.Count - 1)
                    {
                        returnTypeForTyping += ", ";
                    }
                }
                returnTypeForTyping += ")";
                retrun returnTypeForTyping;
            }
            return "object";
        }
    }
}

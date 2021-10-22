using System.Collections.Generic;
using System.Linq;
using AltV.NativesDb.Reader.Models.NativeDb;

namespace Durty.AltV.NativesTypingsGenerator.Converters
{
    public class NativeReturnTypeToTypingConverter
    {
        public string Convert(Native native, List<NativeType> nativeReturnTypes)
        {
            NativeTypeToTypingConverter nativeTypeToTypingConverter = new NativeTypeToTypingConverter();

            if (nativeReturnTypes.Count == 1)
            {
                return nativeTypeToTypingConverter.Convert(native, nativeReturnTypes.First(), false, true);
            }

            string returnTypeForTyping = "[";
            for (int i = 0; i < nativeReturnTypes.Count; i++)
            {
                returnTypeForTyping += nativeTypeToTypingConverter.Convert(native, nativeReturnTypes[i], false, true);
                if (i != nativeReturnTypes.Count - 1)
                {
                    returnTypeForTyping += ", ";
                }
            }
            returnTypeForTyping += "]";
            return returnTypeForTyping;
        }
    }
}

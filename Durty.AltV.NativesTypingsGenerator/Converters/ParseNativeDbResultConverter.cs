using System;
using System.Collections.Generic;
using Durty.AltV.NativesTypingsGenerator.Extensions;
using Durty.AltV.NativesTypingsGenerator.Models.NativeDb;
using Newtonsoft.Json;

namespace Durty.AltV.NativesTypingsGenerator.Converters
{
    internal class ParseNativeDbResultConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(string);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) 
                return null;
            string value = serializer.Deserialize<string>(reader);

            List<string> rawReturnTypes = new List<string>();
            if (value.Contains("[") && value.Contains("]"))
            {
                value = value.Replace("[", "").Replace("]", "");
                string[] types = value.Split(",");
                foreach (string type in types)
                {
                    rawReturnTypes.Add(type.Trim());
                }
            }
            else
            {
                rawReturnTypes.Add(value);
            }

            List<NativeType> returnTypes = new List<NativeType>();
            foreach (string rawReturnType in rawReturnTypes)
            {
                if (Enum.TryParse(rawReturnType.FirstCharToUpper(), out NativeType returnType))
                {
                    returnTypes.Add(returnType);
                }
                else
                {
                    throw new Exception($"Can not map raw return type '{rawReturnType}' to enum");
                }
            }

            return returnTypes;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

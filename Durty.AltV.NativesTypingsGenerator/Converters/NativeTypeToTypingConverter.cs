using System;
using Durty.AltV.NativesTypingsGenerator.Models.NativeDb;

namespace Durty.AltV.NativesTypingsGenerator.Converters
{
    public class NativeParamTypeToTypingConverter
    {
        public string Convert(Native native, NativeParamType nativeParamType)
        {
            switch (nativeParamType)
            {
                case NativeParamType.Any:
                    return "any";
                case NativeParamType.Boolean:
                    return "boolean";
                case NativeParamType.Float:
                    return "number";
                case NativeParamType.Int:
                    return "number";
                case NativeParamType.String:
                    return "string";
                case NativeParamType.Vector3:
                    return "Vector3";
                case NativeParamType.Void:
                    return "void";
                case NativeParamType.ScrHandle:
                    return "intPtr";
                case NativeParamType.MemoryBuffer:
                    return "intPtr";
                case NativeParamType.Interior:
                    return "intPtr"; //Not sure about this
                case NativeParamType.Object:
                    return "intPtr"; //Not sure about this
                case NativeParamType.Hash:
                    return "string"; //Not sure about this
                case NativeParamType.Entity:
                    return "number"; //handle / script id
                case NativeParamType.Ped:
                    return "number"; //handle / script id
                case NativeParamType.Vehicle:
                    return "number"; //handle / script id
                case NativeParamType.Cam:
                    return "number"; //handle / script id
                case NativeParamType.FireId:
                    return "number"; //handle / script id
                case NativeParamType.Blip:
                    return "number"; //handle / script id
                case NativeParamType.Pickup:
                    return "number"; //handle / script id
                case NativeParamType.Player:
                    return "number"; //handle / script id
                default:
                    throw new ArgumentOutOfRangeException(nameof(nativeParamType), nativeParamType, null);
            }
        }
    }
}

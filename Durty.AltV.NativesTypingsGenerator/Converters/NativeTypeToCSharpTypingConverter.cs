using System;
using AltV.NativesDb.Reader.Models.NativeDb;

namespace Durty.AltV.NativesTypingsGenerator.Converters
{
    public class NativeTypeToCSharpTypingConverter
    {
        public string Convert(Native native, NativeType nativeType, bool isReference)
        {
            string referenceType = isReference ? "*" : "";
            return nativeType switch
            {
                NativeType.Any => "int" + referenceType,
                NativeType.Boolean => "bool" + referenceType,
                NativeType.Float => "double" + referenceType,
                NativeType.Int => "int" + referenceType,
                NativeType.String => "string" + referenceType,
                NativeType.Vector3 => "Vector3" + referenceType,
                NativeType.Void => "void" + referenceType,
                NativeType.ScrHandle => "int" + referenceType,
                NativeType.MemoryBuffer => "object" + referenceType,
                NativeType.Interior => "int" + referenceType,
                NativeType.Object => "int" + referenceType,
                NativeType.Hash => "int" + referenceType,
                NativeType.Entity => "int" + referenceType,
                NativeType.Ped => "int" + referenceType,
                NativeType.Vehicle => "int" + referenceType,
                NativeType.Cam => "int" + referenceType,
                NativeType.FireId => "int" + referenceType,
                NativeType.Blip => "int" + referenceType,
                NativeType.Pickup => "int" + referenceType,
                NativeType.Player => "int" + referenceType,
                NativeType.CarGenerator => "int" + referenceType,
                NativeType.Group => "int" + referenceType,
                NativeType.Train => "int" + referenceType,
                NativeType.Weapon => "int" + referenceType,
                NativeType.Texture => "int" + referenceType,
                NativeType.TextureDict => "int" + referenceType,
                NativeType.CoverPoint => "int" + referenceType,
                NativeType.Camera => "int" + referenceType,
                NativeType.TaskSequence => "int" + referenceType,
                NativeType.ColourIndex => "int" + referenceType,
                NativeType.Sphere => "int" + referenceType,
                _ => throw new ArgumentOutOfRangeException(nameof(nativeType), nativeType, null)
            };
        }
    }
}

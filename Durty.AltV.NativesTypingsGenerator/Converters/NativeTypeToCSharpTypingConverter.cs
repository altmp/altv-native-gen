using System;
using AltV.NativesDb.Reader.Models.NativeDb;

namespace Durty.AltV.NativesTypingsGenerator.Converters
{
    public class NativeTypeToCSharpTypingConverter
    {
        public string Convert(Native native, NativeType nativeType, bool isReference, bool isUnmanagedDelegate = false)
        {
            var value = nativeType switch
            {
                NativeType.Any => "int",
                NativeType.Boolean => isUnmanagedDelegate ? "byte" : "bool",
                NativeType.Float => "float",
                NativeType.Int => "int",
                NativeType.String => isUnmanagedDelegate ? "nint" : "string",
                NativeType.Vector3 => "Vector3",
                NativeType.Void => "void",
                NativeType.ScrHandle => "int",
                NativeType.MemoryBuffer => "object",
                NativeType.Interior => "int",
                NativeType.Object => "int",
                NativeType.Hash => "uint",
                NativeType.Entity => "int",
                NativeType.Ped => "int",
                NativeType.Vehicle => "int",
                NativeType.Cam => "int",
                NativeType.FireId => "int",
                NativeType.Blip => "int",
                NativeType.Pickup => "int",
                NativeType.Player => "int",
                NativeType.CarGenerator => "int",
                NativeType.Group => "int",
                NativeType.Train => "int",
                NativeType.Weapon => "int",
                NativeType.Texture => "int",
                NativeType.TextureDict => "int",
                NativeType.CoverPoint => "int",
                NativeType.Camera => "int",
                NativeType.TaskSequence => "int",
                NativeType.ColourIndex => "int",
                NativeType.Sphere => "int",
                _ => throw new ArgumentOutOfRangeException(nameof(nativeType), nativeType, null)
            };
            
            if (isReference)
                if (isUnmanagedDelegate)
                    value = value + "*";
                else value = "ref " + value;

            return value;
        }
    }
}

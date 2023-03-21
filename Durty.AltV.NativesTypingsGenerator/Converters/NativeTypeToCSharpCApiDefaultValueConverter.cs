using System;
using AltV.NativesDb.Reader.Models.NativeDb;

namespace Durty.AltV.NativesTypingsGenerator.Converters
{
    public class NativeTypeToCSharpCApiDefaultValueConverter
    {
        public string Convert(Native native, NativeType nativeType)
        {
            return nativeType switch
            {
                NativeType.Any => "0",
                NativeType.Boolean => "0",
                NativeType.Float => "0.f",
                NativeType.Int => "0",
                NativeType.String => "nullptr",
                NativeType.Vector3 => "{ 0, 0, 0 }",
                NativeType.Void => "",
                NativeType.ScrHandle => "0",
                NativeType.MemoryBuffer => "0",
                NativeType.Interior => "0",
                NativeType.Object => "0",
                NativeType.Hash => "0",
                NativeType.Entity => "0",
                NativeType.Ped => "0",
                NativeType.Vehicle => "0",
                NativeType.Cam => "0",
                NativeType.FireId => "0",
                NativeType.Blip => "0",
                NativeType.Pickup => "0",
                NativeType.Player => "0",
                NativeType.CarGenerator => "0",
                NativeType.Group => "0",
                NativeType.Train => "0",
                NativeType.Weapon => "0",
                NativeType.Texture => "0",
                NativeType.TextureDict => "0",
                NativeType.CoverPoint => "0",
                NativeType.Camera => "0",
                NativeType.TaskSequence => "0",
                NativeType.ColourIndex => "0",
                NativeType.Sphere => "0",
                _ => throw new ArgumentOutOfRangeException(nameof(nativeType), nativeType, null)
            };
        }
    }
}

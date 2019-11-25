using System;
using Durty.AltV.NativesTypingsGenerator.Models.NativeDb;

namespace Durty.AltV.NativesTypingsGenerator.Converters
{
    public class NativeTypeToTypingConverter
    {
        /* see mapping js func by Tuxick
         function getJSType(type) {
           return type
           .replace(/int\s*\* /, 'number')
           .replace(/BOOL\s*\* /, 'boolean')
           .replace('Vector3', 'Vector3')
           .replace(/const char\s*\* /, 'string')
           .replace(/char\s*\* /, 'string')
           .replace(/float\s*\* /, 'number')
           .replace(/float/, 'number')
           .replace(/int/, 'number')
           .replace(/(Player|Hash|Any|Entity|FireId|Ped|Vehicle|Cam|CarGenerator|Group|Train|Pickup|Object|Weapon|Interior|Blip|ScrHandle)\s*\* /, 'number')
           .replace(/Vector3\s*\* /, 'vectorPtr')
           .replace('Entity', 'number')
           .replace('Hash', 'number')
           .replace('Player', 'number')
           .replace('FireId', 'number')
           .replace('Ped', 'number')
           .replace('Vehicle', 'number')
           .replace('Cam', 'number')
           .replace('CarGenerator', 'number')
           .replace('Group', 'number')
           .replace('Train', 'number')
           .replace('Pickup', 'number')
           .replace('Object', 'number')
           .replace('Weapon', 'number')
           .replace('Interior', 'number')
           .replace('Blip', 'number')
           .replace('Texture', 'number')
           .replace('TextureDict', 'number')
           .replace('CoverPoint', 'number')
           .replace('Camera', 'number')
           .replace('TaskSequence', 'number')
           .replace('ColourIndex', 'number')
           .replace('Sphere', 'number')
           .replace('ScrHandle', 'number')
           .replace('Any', 'number')
           .replace('BOOL', 'boolean')
           .replace('void*', 'MemoryBuffer');
           }
         */

        public string Convert(Native native, NativeType nativeType)
        {
            return nativeType switch
            {
                NativeType.Any => "number",
                NativeType.Boolean => "boolean",
                NativeType.Float => "number",
                NativeType.Int => "number",
                NativeType.String => "string",
                NativeType.Vector3 => "Vector3",
                NativeType.Void => "void",
                NativeType.ScrHandle => "number",
                NativeType.MemoryBuffer => "MemoryBuffer",
                NativeType.Interior => "number",
                NativeType.Object => "number",
                NativeType.Hash => "number",
                NativeType.Entity => "number",
                NativeType.Ped => "number",
                NativeType.Vehicle => "number",
                NativeType.Cam => "number",
                NativeType.FireId => "number",
                NativeType.Blip => "number",
                NativeType.Pickup => "number",
                NativeType.Player => "number",
                NativeType.CarGenerator => "number",
                NativeType.Group => "number",
                NativeType.Train => "number",
                NativeType.Weapon => "number",
                NativeType.Texture => "number",
                NativeType.TextureDict => "number",
                NativeType.CoverPoint => "number",
                NativeType.Camera => "number",
                NativeType.TaskSequence => "number",
                NativeType.ColourIndex => "number",
                NativeType.Sphere => "number",
                _ => throw new ArgumentOutOfRangeException(nameof(nativeType), nativeType, null)
            };
        }
    }
}

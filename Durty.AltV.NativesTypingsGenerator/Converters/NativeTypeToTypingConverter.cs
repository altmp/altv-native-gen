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

        public string Convert(Native native, NativeType nativeType, bool isReference)
        {
            string referenceType = isReference ? " | null" : "";
            return nativeType switch
            {
                NativeType.Any => "any" + referenceType,
                NativeType.Boolean => "boolean" + referenceType,
                NativeType.Float => "number" + referenceType,
                NativeType.Int => "number" + referenceType,
                NativeType.String => "string" + referenceType,
                NativeType.Vector3 => "Vector3" + referenceType,
                NativeType.Void => "void" + referenceType,
                NativeType.ScrHandle => "number" + referenceType,
                NativeType.MemoryBuffer => "MemoryBuffer" + referenceType,
                NativeType.Interior => "number" + referenceType,
                NativeType.Object => "number" + referenceType,
                NativeType.Hash => "number" + referenceType,
                NativeType.Entity => "number" + referenceType,
                NativeType.Ped => "number" + referenceType,
                NativeType.Vehicle => "number" + referenceType,
                NativeType.Cam => "number" + referenceType,
                NativeType.FireId => "number" + referenceType,
                NativeType.Blip => "number" + referenceType,
                NativeType.Pickup => "number" + referenceType,
                NativeType.Player => "number" + referenceType,
                NativeType.CarGenerator => "number" + referenceType,
                NativeType.Group => "number" + referenceType,
                NativeType.Train => "number" + referenceType,
                NativeType.Weapon => "number" + referenceType,
                NativeType.Texture => "number" + referenceType,
                NativeType.TextureDict => "number" + referenceType,
                NativeType.CoverPoint => "number" + referenceType,
                NativeType.Camera => "number" + referenceType,
                NativeType.TaskSequence => "number" + referenceType,
                NativeType.ColourIndex => "number" + referenceType,
                NativeType.Sphere => "number" + referenceType,
                _ => throw new ArgumentOutOfRangeException(nameof(nativeType), nativeType, null)
            };
        }
    }
}

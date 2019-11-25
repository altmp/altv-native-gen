using Newtonsoft.Json;

namespace Durty.AltV.NativesTypingsGenerator.Models.NativeDb
{
    public enum NativeParamType
    {
        Any,
        Blip,
        Boolean,
        Cam,
        Entity,
        FireId,
        Float,
        Hash,
        Int,
        Interior,
        MemoryBuffer,
        Object,
        Ped,
        Pickup,
        Player,
        ScrHandle,
        String,
        Vector3,
        Vehicle,
        Void,
        VoidAny,
        VoidVector3Hash
    };

    public class NativeParam
    {
        [JsonProperty("type")]
        public NativeParamType NativeParamType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}

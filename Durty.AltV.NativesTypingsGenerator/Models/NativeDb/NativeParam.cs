using Newtonsoft.Json;

namespace Durty.AltV.NativesTypingsGenerator.Models.NativeDb
{
    public class NativeParam
    {
        [JsonProperty("type")]
        public NativeType NativeParamType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("ref")]
        public bool IsReference { get; set; }
    }
}

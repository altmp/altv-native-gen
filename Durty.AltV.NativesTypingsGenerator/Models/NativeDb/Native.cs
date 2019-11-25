using System.Collections.Generic;
using Durty.AltV.NativesTypingsGenerator.Converters;
using Newtonsoft.Json;

namespace Durty.AltV.NativesTypingsGenerator.Models.NativeDb
{
    public class Native
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("jhash")]
        public string Jhash { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("params")]
        public NativeParam[] Params { get; set; }

        [JsonProperty("build")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Build { get; set; }

        [JsonProperty("results")]
        public string Results { get; set; }

        [JsonProperty("forceNetwork")]
        public bool ForceNetwork { get; set; }

        [JsonProperty("hashes")]
        public Dictionary<string, string> Hashes { get; set; }

        [JsonProperty("altName")]
        public string AltName { get; set; }

        [JsonProperty("unused", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Unused { get; set; }
    }
}

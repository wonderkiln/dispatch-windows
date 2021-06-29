using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Dispatch.Service.Models
{
    public class Update
    {
        [JsonProperty("createdAt")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("version")]
        public Version Version { get; set; }

        public string Notes { get; set; }

        [JsonProperty("url")]
        public Uri Link { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }
    }
}

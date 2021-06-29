using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Dispatch.Service.Models
{
    public class License
    {
        [JsonProperty("createdAt")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("expiresAt")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime ExpiresAt { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("majorVersion")]
        public int MajorVersion { get; set; }

        [JsonProperty("numberOfComputers")]
        public int NumberOfComputers { get; set; }

        [JsonProperty("isExpired")]
        public bool IsExpired { get; set; }
    }
}

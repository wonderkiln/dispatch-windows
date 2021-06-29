using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Dispatch.Service.Models
{
    public class Device
    {
        [JsonProperty("createdAt")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("platform")]
        public string Platform { get; set; }

        [JsonProperty("license")]
        public License License { get; set; }

        [JsonProperty("isTrial")]
        public bool IsTrial { get; set; }

        [JsonProperty("expiresAt")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime ExpiresAt { get; set; }

        [JsonProperty("isExpired")]
        public bool IsExpired { get; set; }
    }
}

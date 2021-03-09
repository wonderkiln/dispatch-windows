using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Dispatch.Service.Models
{
    public class DeviceStatus
    {
        public enum LicenseStatus
        {
            LicenseExpiredWrongVersion = 22,
            LicenseExpired = 21,
            License = 20,
            TrialExpired = 11,
            Trial = 10,
            None = 0,
        }

        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter), converterParameters: typeof(SnakeCaseNamingStrategy))]
        public LicenseStatus Status { get; set; }

        [JsonProperty("device")]
        public Device Device { get; set; }
    }
}

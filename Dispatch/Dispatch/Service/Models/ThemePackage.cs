using Newtonsoft.Json;

namespace Dispatch.Service.Models
{
    public class ThemePackage
    {
        public class FileIcon
        {
            [JsonProperty("extensions")]
            public string[] Extensions { get; set; }

            [JsonProperty("source")]
            public string Source { get; set; }
        }

        public string Path { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fileIcons")]
        public FileIcon[] FileIcons { get; set; }
    }
}

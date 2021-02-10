using Dispatch.Helpers;
using Dispatch.Service.Models;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Dispatch.Service.Updater
{
    public class UpdateProvider : IUpdateProvider
    {
        private class Release
        {
            public string url { get; set; }
            public long fileSize { get; set; }
            public string versionCode { get; set; }
            public string releaseDate { get; set; }
        }

        private string BaseUrl
        {
            get
            {
                switch (Constants.CHANNEL)
                {
                    case Constants.Channel.Nightly:
                        return "https://api.dispatch.wonderkiln.com/release/nightly";
                    case Constants.Channel.Beta:
                        return "https://api.dispatch.wonderkiln.com/release/beta";
                    case Constants.Channel.Stable:
                        return "https://api.dispatch.wonderkiln.com/release/stable";
                    default:
                        throw new Exception($"Unhandled channel: {Constants.CHANNEL}");
                }
            }
        }

        public async Task<Update> GetLatestUpdate()
        {
            var client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, Constants.APP_NAME);
            client.Headers.Add(HttpRequestHeader.Accept, "application/json");

            var data = await client.DownloadStringTaskAsync(BaseUrl);
            var json = JsonConvert.DeserializeObject<Release>(data);

            return new Update(new Version(json.versionCode), "", json.fileSize, new Uri(json.url));
        }

    }
}

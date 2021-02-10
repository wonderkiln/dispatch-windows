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
            public long size { get; set; }
            public string version { get; set; }
            public string createdAt { get; set; }
        }

        private string BaseUrl
        {
            get
            {
                switch (Constants.CHANNEL)
                {
                    case Constants.Channel.Nightly:
                        return "https://api.dispatch.wonderkiln.com/api/release/nightly";
                    case Constants.Channel.Beta:
                        return "https://api.dispatch.wonderkiln.com/api/release/beta";
                    case Constants.Channel.Stable:
                        return "https://api.dispatch.wonderkiln.com/api/release/stable";
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

            return new Update(new Version(json.version), "", json.size, new Uri(json.url));
        }

    }
}

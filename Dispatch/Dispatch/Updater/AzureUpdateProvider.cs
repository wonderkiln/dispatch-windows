using Dispatch.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Updater
{
    public class AzureUpdateProvider : IUpdateProvider
    {
        class LatestRelease
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
                        return "https://dispatch-api-dev.herokuapp.com/release/nighly";
                    case Constants.Channel.Beta:
                        return "https://dispatch-api-dev.herokuapp.com/release/beta";
                    case Constants.Channel.Stable:
                        return "https://dispatch-api-dev.herokuapp.com/release/stable";
                    default:
                        throw new Exception("Unhandled channel");
                }
            }
        }

        public event EventHandler<double> DownloadProgressChanged;
        private WebClient GetWebClient()
        {
            var client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, "Dispatch");

            return client;
        }

        public async Task<UpdateInfo> GetLatestUpdate()
        {
            var client = GetWebClient();
            client.Headers.Add(HttpRequestHeader.Accept, "application/json");

            var data = await client.DownloadStringTaskAsync(BaseUrl);
            var json = JsonConvert.DeserializeObject<LatestRelease>(data);

            return new UpdateInfo()
            {
                Version = new Version(json.versionCode),
                ReleaseNotes = "",
                DownloadSize = json.fileSize,
                DownloadUrl = json.url
            };
        }

        public async Task<string> DownloadUpdate(UpdateInfo info)
        {
            var client = GetWebClient();
            client.Headers.Add(HttpRequestHeader.Accept, "application/octet-stream");
            client.DownloadProgressChanged += Client_DownloadProgressChanged;

            var path = Path.GetTempFileName();

            try
            {
                await client.DownloadFileTaskAsync(info.DownloadUrl, path);
            }
            finally
            {
                client.DownloadProgressChanged -= Client_DownloadProgressChanged;
            }

            var newPath = Path.ChangeExtension(path, "exe");
            File.Move(path, newPath);

            return newPath;
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressChanged?.Invoke(this, e.ProgressPercentage);
        }
    }
}

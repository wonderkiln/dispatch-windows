using Dispatch.Helpers;
using Dispatch.Service.Model;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Dispatch.Service.Updater
{
    public class UpdateProvider : IUpdateProvider
    {
        class Release
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
                        return "https://dispatch-api-dev.herokuapp.com/release/nightly";
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
            var json = JsonConvert.DeserializeObject<Release>(data);

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

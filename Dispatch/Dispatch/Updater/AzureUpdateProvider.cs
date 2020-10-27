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
            public string name { get; set; }
            public string version { get; set; }
        }

        private static readonly string BASE_URL = "https://stdispatchdev.blob.core.windows.net/downloads/";

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

            var data = await client.DownloadStringTaskAsync(new Uri(BASE_URL + "latest.json"));
            var json = JsonConvert.DeserializeObject<LatestRelease>(data);

            return new UpdateInfo()
            {
                Version = new Version(json.version),
                ReleaseNotes = "",
                DownloadSize = 0,
                DownloadUrl = BASE_URL + json.name
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

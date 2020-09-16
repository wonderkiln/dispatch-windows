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
    public class GithubUpdateProvider : IUpdateProvider
    {
        class Release
        {
            public class Asset
            {
                public string url { get; set; }
                public long size { get; set; }
                public string browser_download_url { get; set; }
            }

            public string tag_name { get; set; }
            public string body { get; set; }
            public Asset[] assets { get; set; }
        }

        private string url;
        private string token;

        public GithubUpdateProvider(string url, string token)
        {
            this.url = url;
            this.token = token;
        }

        public event EventHandler<double> DownloadProgressChanged;

        private WebClient GetWebClient()
        {
            var client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, "Dispatch");
            client.Headers.Add(HttpRequestHeader.Authorization, $"token {token}");

            return client;
        }

        public async Task<UpdateInfo> GetLatestUpdate()
        {
            var client = GetWebClient();

            var data = await client.DownloadStringTaskAsync(new Uri(url));
            var json = JsonConvert.DeserializeObject<Release>(data);

            var asset = json.assets.First(e => e.browser_download_url.Contains(".exe"));

            return new UpdateInfo()
            {
                Version = new Version(json.tag_name),
                ReleaseNotes = json.body,
                DownloadSize = asset.size,
                DownloadUrl = asset.url
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

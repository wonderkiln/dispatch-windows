using Dispatch.Helpers;
using Dispatch.Service.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace Dispatch.Service.Updater
{
    public class ApplicationUpdater
    {
        private readonly IUpdateProvider updater;

        private readonly IProgress<double> progress;

        public ApplicationUpdater(IUpdateProvider updater, IProgress<double> progress = null)
        {
            this.updater = updater;
            this.progress = progress;
        }

        public async Task<Update> CheckForUpdate()
        {
            var update = await updater.GetLatestUpdate();
            if (update.Version > Constants.VERSION) return update;
            return null;
        }

        public async void DownloadAndInstall()
        {
            var client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, Constants.APP_NAME);
            client.Headers.Add(HttpRequestHeader.Accept, "application/octet-stream");
            client.DownloadProgressChanged += Client_DownloadProgressChanged;

            try
            {
                var path = Path.GetTempFileName();
                var update = await updater.GetLatestUpdate();

                await client.DownloadFileTaskAsync(update.Link, path);

                var newPath = Path.ChangeExtension(path, "exe");
                File.Move(path, newPath);

                Process.Start(newPath, "/VERYSILENT");
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                client.DownloadProgressChanged -= Client_DownloadProgressChanged;
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progress.Report(e.ProgressPercentage);
        }
    }
}

using ByteSizeLib;
using Dispatch.Helpers;
using Dispatch.Service.Model;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace Dispatch.Service.Updater
{
    public class ApplicationUpdater
    {
        private readonly IUpdateProvider updater;

        public event EventHandler<double> DownloadProgressChanged;

        public UpdateInfo LatestUpdate { get; set; }

        public bool HasUpdate
        {
            get
            {
                if (LatestUpdate == null) return false;
                return LatestUpdate.Version > Constants.VERSION;
            }
        }

        public ApplicationUpdater(IUpdateProvider updater)
        {
            this.updater = updater;
            this.updater.DownloadProgressChanged += Updater_DownloadProgressChanged;
        }

        private void Updater_DownloadProgressChanged(object sender, double e)
        {
            DownloadProgressChanged?.Invoke(this, e);
        }

        public async Task CheckForUpdate(bool silent = false)
        {
#if DEBUG
#else
            try
            {
                LatestUpdate = await updater.GetLatestUpdate();

                if (HasUpdate)
                {
                    if (!silent && MessageBox.Show(
                        $"Do you want to download and install it now ({ByteSize.FromBytes(LatestUpdate.DownloadSize)})?",
                        $"Update available from {Constants.VERSION} to {LatestUpdate.Version} ({Constants.CHANNEL})",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question,
                        MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    {
                        DownloadAndInstall();
                    }
                }
                else if (!silent)
                {
                    MessageBox.Show(
                        $"You already have the latest version ({Constants.VERSION})",
                        $"No update available ({Constants.CHANNEL})",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.Error.WriteLine(ex);
            }
#endif
        }

        public async void DownloadAndInstall()
        {
            try
            {
                var path = await updater.DownloadUpdate(LatestUpdate);

                Process.Start(path, "/VERYSILENT");
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

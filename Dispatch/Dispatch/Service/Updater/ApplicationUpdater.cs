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

        public UpdateInfo LatestUpdate { get; set; }

        public ApplicationUpdater(IUpdateProvider updater)
        {
            this.updater = updater;
        }

        public async Task CheckForUpdate()
        {
            try
            {
                LatestUpdate = await updater.GetLatestUpdate();

                if (LatestUpdate.Version > Constants.VERSION)
                {
                    if (MessageBox.Show(
                        $"Do you want to download and install it now ({ByteSize.FromBytes(LatestUpdate.DownloadSize)})?",
                        $"Update available from {Constants.VERSION} to {LatestUpdate.Version} ({Constants.CHANNEL})",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question,
                        MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    {
                        DownloadUpdate();
                    }
                }
                else
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
        }

        private async void DownloadUpdate()
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

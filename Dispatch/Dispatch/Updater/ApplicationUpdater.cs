using ByteSizeLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dispatch.Updater
{
    public class ApplicationUpdater
    {
        public static Version CurrentVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        private IUpdateProvider updater;
        private UpdateInfo info;

        public ApplicationUpdater(IUpdateProvider updater)
        {
            this.updater = updater;
            this.updater.DownloadProgressChanged += Updater_DownloadProgressChanged;
        }

        private void Updater_DownloadProgressChanged(object sender, double e)
        {
            Console.WriteLine($"Downloading... {e}%");
        }

        public async void CheckForUpdate(bool silent = true)
        {
            try
            {
                info = await updater.GetLatestUpdate();

                if (info.Version > CurrentVersion)
                {
                    if (MessageBox.Show($"Release notes:\n{info.ReleaseNotes}\n\nDo you want to download and install it now ({ByteSize.FromBytes(info.DownloadSize)})?", $"Update available from {CurrentVersion} to {info.Version}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    {
                        DownloadUpdate();
                    }
                }
                else
                {
                    if (!silent)
                    {
                        MessageBox.Show("You already have the latest version", "No update available", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);

                if (!silent)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public async void DownloadUpdate()
        {
            try
            {
                var path = await updater.DownloadUpdate(info);

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
